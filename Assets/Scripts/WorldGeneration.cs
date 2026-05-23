using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGeneration : MonoBehaviour
{
    private int[,] world;
    public int worldX, worldY;
    public int minRoom, maxRoom;
    public int countCenterPointRoom;
    private int actualRoomCount;

    [HideInInspector]
    public int netWeightRoom;

    [Header("Tilemaps")]
    public Tilemap tilemapWall;
    public Tilemap tilemapWallRL;
    public Tilemap tilemapWallUD;

    [Header("Tiles")]
    public Tile wall;
    public Tile wallU;
    public Tile wallD;
    public Tile wallL;
    public Tile wallR;
    public Tile ground;

    [Header("Parameters")]
    public int tunnelWidth;
    private GameObject player;
    public GameObject portal;
    public GameObject portalOnStart;
    public int lvl;

    public List<Enemy> enemy;
    public List<Decorates> decorates;

    public int countEnemies;
    public int[,] centers;
    public Transform parentForEnemy;
    public Transform parentForDecorates;

    private List<RuntimeEnemy> runtimeEnemies = new List<RuntimeEnemy>();
    private List<int>[] roomGraph;

   
    private int[] roomSizes;

    [Serializable]
    public class Enemy
    {
        public float costSpawn;
        public GameObject enemy;
        public int weight;
    }

    private class RuntimeEnemy
    {
        public GameObject enemyPrefab;
        public int countSpawn;
        public int weight;
    }

    [Serializable]
    public class Decorates
    {
        public int idObj;
        public GameObject objSpawn;
        public int radius;
        public int minCount, maxCount;
        public int chanceSpawn;
    }

    private void Start()
    {
        DataBase.SetWeapons();
        player = DataBase.GetPlayer(XmlSaver.Read().idPlayer);
        if(player.GetComponent<PlayerStats>().GetLvl() > 10)
        {
            lvl = player.GetComponent<PlayerStats>().GetLvl() * 2;
        }
        else
        {
            lvl = 15;
        }
            InitializeRuntimeEnemies();
        SetEnemiesCount();
        CreateWorld();

        
        GetComponent<MozgGeneration>().player =
            CreateObjectsOnCoord(player, centers[0, 0], centers[0, 1]);

        int portalsToSpawn = Mathf.Min(actualRoomCount, actualRoomCount - 1);
        for (int i = 0; i < portalsToSpawn; i++)
        {
            CreateObjectsOnCoord(portalOnStart, centers[i, 0], centers[i, 1]);
        }

        GetComponent<MozgGeneration>().portalOld =
            CreateObjectsOnCoord(portal,
                centers[actualRoomCount - 1, 0],
                centers[actualRoomCount - 1, 1]).transform;
    }

    void InitializeRuntimeEnemies()
    {
        runtimeEnemies.Clear();
        foreach (var e in enemy)
        {
            runtimeEnemies.Add(new RuntimeEnemy
            {
                enemyPrefab = e.enemy,
                weight = e.weight,
                countSpawn = Mathf.FloorToInt(lvl * e.costSpawn)
            });
        }
    }

    void SetEnemiesCount()
    {
        int allWeight = 0;
        for (int i = 0; i < runtimeEnemies.Count; i++)
        {
            allWeight += runtimeEnemies[i].countSpawn * runtimeEnemies[i].weight;
        }
        netWeightRoom = Mathf.FloorToInt((float)allWeight / countCenterPointRoom) - 5;
    }

    void CreateWorld()
    {
        int offsetX = 100;
        int offsetY = 100;

        world = new int[worldX + 200, worldY + 200];

        for (int x = 0; x < world.GetLength(0); x++)
            for (int y = 0; y < world.GetLength(1); y++)
                world[x, y] = -1;

        int targetRoomCount = countCenterPointRoom;

        roomSizes = new int[targetRoomCount];
        centers = new int[targetRoomCount, 2];

        actualRoomCount = PlaceRooms(offsetX, offsetY, targetRoomCount);

        BuildRoomGraph(actualRoomCount);
        DigTunnelsByGraph(offsetX, offsetY, actualRoomCount);

        tilemapWall.ClearAllTiles();

        for (int x = 0; x < world.GetLength(0); x++)
        {
            for (int y = 0; y < world.GetLength(1); y++)
            {
                int logicX = x - offsetX;
                int logicY = y - offsetY;
                Vector3Int tilePos = new Vector3Int(logicX, logicY, 0);

                if (world[x, y] == -1)
                    tilemapWall.SetTile(tilePos, wall);
            }
        }

        Check(offsetX, offsetY);
    }

    int PlaceRooms(int offsetX, int offsetY, int targetRoomCount)
    {
        int mapCenterX = worldX / 2;
        int mapCenterY = worldY / 2;

        float radius = worldX / 3f;
        float angleStep = 360f / targetRoomCount;

        Debug.Log($"PlaceRooms START: target={targetRoomCount} radius={radius}");

        for (int i = 0; i < targetRoomCount; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;

            int size = UnityEngine.Random.Range(minRoom, maxRoom);

            int posX = mapCenterX + Mathf.RoundToInt(Mathf.Cos(angle) * radius);
            int posY = mapCenterY + Mathf.RoundToInt(Mathf.Sin(angle) * radius);

            centers[i, 0] = posX;
            centers[i, 1] = posY;
            roomSizes[i] = size;

            Debug.Log($"Комната {i}: pos=({posX},{posY}) size={size}");

            DrawRoom(posX + offsetX, posY + offsetY, size);

            if (i != 0)
            {
                CreateEnemies(posX, posY,
                    -size, size,
                    -size, size);

                SetRoomsDecos(posX + offsetX, posY + offsetY,
                    -size, size,
                    -size, size);
            }
        }

        Debug.Log($"PlaceRooms DONE: создано {targetRoomCount} комнат");

        return targetRoomCount;
    }

    void DrawRoom(int centerX, int centerY, int roomSize)
    {
        for (int x = -roomSize; x <= roomSize; x++)
        {
            for (int y = -roomSize; y <= roomSize; y++)
            {
                int tx = centerX + x;
                int ty = centerY + y;
                if (tx >= 0 && tx < world.GetLength(0) &&
                    ty >= 0 && ty < world.GetLength(1))
                {
                    world[tx, ty] = 0;
                }
            }
        }
    }

    void BuildRoomGraph(int roomCount)
    {
        roomGraph = new List<int>[roomCount];
        for (int i = 0; i < roomCount; i++)
            roomGraph[i] = new List<int>();

        
        for (int i = 0; i < roomCount - 1; i++)
        {
            roomGraph[i].Add(i + 1);
            roomGraph[i + 1].Add(i);
        }

      
    }




    void DigTunnelsByGraph(int offsetX, int offsetY, int roomCount)
    {
        HashSet<string> dugPairs = new HashSet<string>();

        for (int i = 0; i < roomCount; i++)
        {
            foreach (int j in roomGraph[i])
            {
                int a = Mathf.Min(i, j);
                int b = Mathf.Max(i, j);
                string key = a + "_" + b;

                if (dugPairs.Contains(key)) continue;
                dugPairs.Add(key);

                int startX = centers[a, 0] + offsetX;
                int startY = centers[a, 1] + offsetY;
                int endX = centers[b, 0] + offsetX;
                int endY = centers[b, 1] + offsetY;

                DigTunnelX(startX, endX, startY);
                DigTunnelY(startY, endY, endX);
            }
        }
    }

    void DigTunnelX(int startX, int endX, int y)
    {
        int min = Mathf.Min(startX, endX);
        int max = Mathf.Max(startX, endX);
        int halfWidth = tunnelWidth / 2;

        for (int x = min; x <= max; x++)
        {
            for (int w = -halfWidth; w <= halfWidth; w++)
            {
                int ty = y + w;
                if (ty >= 0 && ty < world.GetLength(1))
                    world[x, ty] = 0;
            }
        }
    }

    void DigTunnelY(int startY, int endY, int x)
    {
        int min = Mathf.Min(startY, endY);
        int max = Mathf.Max(startY, endY);
        int halfWidth = tunnelWidth / 2;

        for (int y = min; y <= max; y++)
        {
            for (int w = -halfWidth; w <= halfWidth; w++)
            {
                int tx = x + w;
                if (tx >= 0 && tx < world.GetLength(0))
                    world[tx, y] = 0;
            }
        }
    }

    void SetRoomsDecos(int centerRoomX, int centerRoomY, int xMin, int xMax, int yMin, int yMax)
    {
        for (int i = 0; i < decorates.Count; i++)
        {
            if (decorates[i].chanceSpawn <= UnityEngine.Random.Range(0, 100)) continue;

            int countOfDeco = UnityEngine.Random.Range(decorates[i].minCount, decorates[i].maxCount);
            int rad = Mathf.Max(1, decorates[i].radius);

            int minXRange = centerRoomX + xMin + rad;
            int maxXRange = centerRoomX + xMax - rad;
            int minYRange = centerRoomY + yMin + rad;
            int maxYRange = centerRoomY + yMax - rad;

            if (minXRange >= maxXRange || minYRange >= maxYRange) continue;

            int attempts = 0;
            while (countOfDeco > 0 && attempts < 100)
            {
                attempts++;
                int randomX = UnityEngine.Random.Range(minXRange, maxXRange);
                int randomY = UnityEngine.Random.Range(minYRange, maxYRange);

                bool canPlace = true;
                for (int z = 0; z < rad && canPlace; z++)
                {
                    for (int w = 0; w < rad && canPlace; w++)
                    {
                        int tx = randomX + z;
                        int ty = randomY + w;
                        if (tx < 0 || tx >= world.GetLength(0) ||
                            ty < 0 || ty >= world.GetLength(1) ||
                            world[tx, ty] != 0)
                            canPlace = false;
                    }
                }

                if (canPlace)
                {
                    for (int z = 0; z < rad; z++)
                        for (int w = 0; w < rad; w++)
                            world[randomX + z, randomY + w] = 10 + i;

                    countOfDeco--;
                }
            }
        }
    }

    void CreateEnemies(int centerRoomX, int centerRoomY, int xMin, int xMax, int yMin, int yMax)
    {
        if (xMax - xMin < 4 || yMax - yMin < 4) return;

        int netWeight = netWeightRoom;

        for (int u = runtimeEnemies.Count - 1; u >= 0; u--)
        {
            if (runtimeEnemies[u].countSpawn <= 0) continue;

            int maxPossible = Mathf.CeilToInt(runtimeEnemies[u].countSpawn / 5f);
            int count = UnityEngine.Random.Range(1, Mathf.Max(2, maxPossible));
            if (runtimeEnemies[u].countSpawn - count < 0)
                count = runtimeEnemies[u].countSpawn;

            if (count > 0)
            {
                SpawnEnemyGroup(u, count, centerRoomX, centerRoomY, xMin, xMax, yMin, yMax);
                netWeight -= count * runtimeEnemies[u].weight;
                runtimeEnemies[u].countSpawn -= count;
                countEnemies += count;
            }
        }

        if (netWeight > 0)
        {
            for (int u = 0; u < runtimeEnemies.Count; u++)
            {
                if (netWeight <= 0 || runtimeEnemies[u].countSpawn <= 0) continue;

                int maxPossible = Mathf.CeilToInt(runtimeEnemies[u].countSpawn / 5f);
                int count = UnityEngine.Random.Range(1, Mathf.Max(2, maxPossible));
                if (runtimeEnemies[u].countSpawn - count < 0)
                    count = runtimeEnemies[u].countSpawn;

                if (count > 0)
                {
                    SpawnEnemyGroup(u, count, centerRoomX, centerRoomY, xMin, xMax, yMin, yMax);
                    netWeight -= count * runtimeEnemies[u].weight;
                    runtimeEnemies[u].countSpawn -= count;
                    countEnemies += count;
                }
            }
        }
    }

    void SpawnEnemyGroup(int enemyIndex, int count, int centerX, int centerY,
                          int xMin, int xMax, int yMin, int yMax)
    {
        int rangeXMin = xMin + 2;
        int rangeXMax = xMax - 1;
        if (rangeXMin >= rangeXMax) { rangeXMin = xMin; rangeXMax = xMin + 1; }

        int rangeYMin = yMin + 2;
        int rangeYMax = yMax - 1;
        if (rangeYMin >= rangeYMax) { rangeYMin = yMin; rangeYMax = yMin + 1; }

        for (int o = 0; o < count; o++)
        {
            float gridX = centerX + UnityEngine.Random.Range(rangeXMin, rangeXMax);
            float gridY = centerY + UnityEngine.Random.Range(rangeYMin, rangeYMax);

            float jitterX = UnityEngine.Random.Range(-0.15f, 0.15f);
            float jitterY = UnityEngine.Random.Range(-0.15f, 0.15f);

            CreateObjectsOnCoord(runtimeEnemies[enemyIndex].enemyPrefab,
                gridX + jitterX, gridY + jitterY, parentForEnemy);
        }
    }

    void Check(int offsetX, int offsetY)
    {
        tilemapWallRL.ClearAllTiles();
        tilemapWallUD.ClearAllTiles();

        for (int x = 1; x < world.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < world.GetLength(1) - 1; y++)
            {
                int logicX = x - offsetX;
                int logicY = y - offsetY;
                Vector3Int checkPos = new Vector3Int(logicX, logicY, 0);

                if (world[x, y] != -1)
                {
                    if (world[x - 1, y] == -1) tilemapWallRL.SetTile(checkPos, wallL);
                    if (world[x + 1, y] == -1) tilemapWallRL.SetTile(checkPos, wallR);
                    if (world[x, y - 1] == -1) tilemapWallUD.SetTile(checkPos, wallD);
                    if (world[x, y + 1] == -1) tilemapWallUD.SetTile(checkPos, wallU);
                }
            }
        }
    }

    GameObject CreateObjectsOnCoord(GameObject obj, float x, float y)
    {
        return Instantiate(obj, new Vector3(x, y, 0), Quaternion.identity);
    }

    GameObject CreateObjectsOnCoord(GameObject obj, float x, float y, Transform parent)
    {
        GameObject t = Instantiate(obj, new Vector3(x, y, 0), Quaternion.identity);
        t.transform.parent = parent;
        return t;
    }
}