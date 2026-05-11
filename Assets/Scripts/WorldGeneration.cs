using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Net;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGeneration : MonoBehaviour
{

    int[,] world;
    public int worldX, worldY;
    public int minRoom, maxRoom;
    public int countCenterPointRoom;
    public int netWeightRoom;
    public Tilemap tilemapWall;
    public Tilemap tilemapWallRL;
    public Tilemap tilemapWallUD;
    public Tile wall;
    public Tile wallU;
    public Tile wallD;
    public Tile wallL;
    public Tile wallR;
    public Tile ground;
    public int tunnelWidth;
    GameObject player;
    public GameObject portal;
    public GameObject portalOnStart;
    public int lvl;
    public List<Enemy> enemy;
    public List<Decorates> decorates;
    public int countEnemies;
    public int[,] centers;
    public Transform parentForEnemy;
    public Transform parentForDecorates;

    [Serializable]
    public class Enemy
    {
        public float costSpawn;
        public GameObject enemy;
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
        print(XmlSaver.Read().idPlayer);
        GetComponent<MozgGeneration>().player = CreateObjectsOnCoord(player, -6.5f, -6.5f);
        SetEnemiesCount();
        CreateWorld();
        CreateObjectsOnCoord(portalOnStart, centers[0, 0], centers[0, 1]);
        CreateObjectsOnCoord(portalOnStart, centers[1, 0], centers[1, 1]);
        CreateObjectsOnCoord(portalOnStart, centers[2, 0], centers[2, 1]);
        CreateObjectsOnCoord(portalOnStart, centers[3, 0], centers[3, 1]);
        CreateObjectsOnCoord(portalOnStart, centers[4, 0], centers[4, 1]);
        CreateObjectsOnCoord(portalOnStart, centers[5, 0], centers[5, 1]);
        CreateObjectsOnCoord(portalOnStart, centers[6, 0], centers[6, 1]);
        GetComponent<MozgGeneration>().portalOld = CreateObjectsOnCoord(portal, centers[countCenterPointRoom - 1, 0], centers[countCenterPointRoom - 1, 1]).transform;

    }

    void CreateWorld()
    {
        print("Creating World");
        world = new int[worldX + 100, worldY + 100];
        for (int x = 0; x < worldX + 100; x++)
        {
            for (int y = 0; y < worldY + 100; y++)
            {
                world[x, y] = -1;
            }
        }
        //centerRoom
        int zeros = countCenterPointRoom;
        int[,] posXY = new int[countCenterPointRoom, 2];
        while (zeros > 0)
        {
            int _x = UnityEngine.Random.Range(1, worldX);
            int _y = UnityEngine.Random.Range(1, worldY);
            if (world[_x, _y] != 0)
            {
                world[_x, _y] = zeros;
                zeros--;
                posXY[zeros, 0] = _x;
                posXY[zeros, 1] = _y;
            }
        }
        centers = posXY;
        //RoomCreate
        print("Looking rooms");
        for (int i = 0; i < countCenterPointRoom; i++)
        {
            int x = centers[i, 0];
            int y = centers[i, 1];
            if (world[x, y] > 0)
            {
                int roomRange = UnityEngine.Random.Range(minRoom, maxRoom);
                int minX = 0;
                int minY = 0;
                int maxX = 0;
                int maxY = 0;
                for (int _x = -roomRange; _x < roomRange; _x++)
                {
                    for (int _y = -roomRange; _y < roomRange; _y++)
                    {
                        try
                        {
                            print(1);
                            world[x + _x, y + _y] = 0;
                            print(2);
                            minX = _x < minX ? _x : minX;
                            minY = _y < minY ? _y : minY;
                            maxX = _x > maxX ? _x : maxX;
                            maxY = _x > maxX ? _x : maxX;
                        }
                        catch
                        {

                        }
                    }
                }
                print(minX + " " + maxX + " " + minY + " " + maxY);
                SetRoomsDecos(centers[i, 0], centers[i, 1], minX, maxX, minY, maxY);
                CreateEnemies(centers[i, 0], centers[i, 1], minX, maxX, minY, maxY);
            }
        }
        //tunnels
        print("Creating tunnels");
        for (int i = 0; i < countCenterPointRoom - 1; i += 1)
        {
            int xPosEnd = 0;
            for (int x = posXY[i, 0]; x < posXY[i + 1, 0]; x++)
            {
                xPosEnd = CreateTunnelX(x, posXY[i, 1]);
            }
            for (int x = posXY[i, 0]; x > posXY[i + 1, 0]; x--)
            {
                xPosEnd = CreateTunnelX(x, posXY[i, 1]);
            }
            for (int y = posXY[i, 1]; y < posXY[i + 1, 1]; y++)
            {
                CreateTunnelY(xPosEnd, y);
            }
            for (int y = posXY[i, 1]; y > posXY[i + 1, 1]; y--)
            {
                CreateTunnelY(xPosEnd, y);
            }
        }

        int CreateTunnelX(int x, int y)
        {
            int xPosEnd = 0;
            for (int u = (int)0 - tunnelWidth / 2; u < (int)tunnelWidth - tunnelWidth / 2; u++)
            {
                try
                {
                    world[x, y + u] = 0;
                    xPosEnd = x;
                }
                catch
                {

                }
            }
            return xPosEnd;
        }
        void CreateTunnelY(int x, int y)
        {
            for (int u = (int)0 - tunnelWidth / 2; u < (int)tunnelWidth - tunnelWidth / 2; u++)
            {
                try
                {
                    world[x + u, y] = 0;
                }
                catch
                {

                }
            }

        }

        //tileCreate
        print("CreatingTiles");
        for (int x = 0; x < worldX; x++)
        {
            for (int y = 0; y < worldY; y++)
            {
                if (world[x, y] == -1)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), wall);
                }
                if (world[x, y] >= 10)
                {
                    CreateObjectsOnCoord(decorates[world[x, y] - 10].objSpawn, x, y, parentForDecorates).GetComponent<SpriteRenderer>().sortingOrder = -y;
                }
            }
        }
        for (int x = 0; x < worldX + 10; x++)
        {
            for (int i = 0; i < 50; i++)
            {
                tilemapWall.SetTile(new Vector3Int(x, -1 - i, 0), wall);
                tilemapWall.SetTile(new Vector3Int(x, worldY + i, 0), wall);
            }
        }
        for (int y = -50; y < worldY + 50; y++)
        {
            for (int i = 0; i < 50; i++)
            {
                tilemapWall.SetTile(new Vector3Int(-1 - i, y, 0), wall);
                tilemapWall.SetTile(new Vector3Int(worldX + i, y, 0), wall);
            }
        }
        for (int x = -10; x < -4; x++)
        {
            for (int y = -10; y < -4; y++)
            {
                tilemapWall.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
        int _xPosEnd = 0;
        for (int x = -7; x < posXY[0, 0]; x++)
        {
            for (int u = (int)-5 - tunnelWidth / 2; u < (int)-6 + tunnelWidth - tunnelWidth / 2; u++)
            {
                tilemapWall.SetTile(new Vector3Int(x, u, 0), null);
                _xPosEnd = x;
                if (x >= 0 && u >= 0)
                {
                    world[x, u] = 0;
                }
            }
        }
        for (int y = -6; y < posXY[0, 1]; y++)
        {
            for (int u = (int)-1 - tunnelWidth / 2; u < (int)-1 + tunnelWidth - tunnelWidth / 2; u++)
            {
                tilemapWall.SetTile(new Vector3Int(_xPosEnd + u, y, 0), null);
                if (_xPosEnd + u >= 0 && y >= 0)
                {
                    world[_xPosEnd + u, y] = 0;
                }
            }

        }
        for (int x = 0; x < worldX + 99; x++)
        {
            for (int y = 0; y < worldY + 99; y++)
            {
                if (world[x, y] == -1)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), wall);
                }
            }
        }
        for (int x = 0; x < worldX + 99; x++)
        {
            for (int y = 0; y < worldY + 99; y++)
            {
                if (world[x, y] == 0)
                {
                    tilemapWall.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
        Check();
    }

    void SetRoomsDecos(int _centerRoomX, int _centerRoomY, int _xMin, int _xMax, int _yMin, int _yMax)
    {
        for (int i = 0; i < decorates.Count; i++)
        {
            int countOfDeco = UnityEngine.Random.Range(decorates[i].minCount, decorates[i].maxCount);
            if (decorates[i].chanceSpawn > UnityEngine.Random.Range(0, 100))
            {
                int minXPos = UnityEngine.Random.Range(_centerRoomX + (_xMin + decorates[i].radius), _centerRoomX + (_xMax - decorates[i].radius));
                int minYPos = UnityEngine.Random.Range(_centerRoomY + (_yMin + decorates[i].radius), _centerRoomY + (_yMax - decorates[i].radius));
                for (int z = 0; z < decorates[i].radius; z++)
                {
                    for (int w = 0; w < decorates[i].radius; w++)
                    {
                        try
                        {
                            if (countOfDeco > 0)
                            {
                                world[UnityEngine.Random.Range(minXPos, minXPos + decorates[i].radius), UnityEngine.Random.Range(minYPos, minYPos + decorates[i].radius)] = decorates[i].idObj;
                                countOfDeco -= 1;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }
    }

    void Check()
    {
        for (int _x = -100; _x < worldX + 100; _x++)
        {
            for (int _y = -100; _y < worldY + 100; _y++)
            {

                if (tilemapWall.GetTile(new Vector3Int(_x, _y)) == null)
                {

                    if (tilemapWall.GetTile(new Vector3Int(_x - 1, _y)) != null)
                    {
                        tilemapWallRL.SetTile(new Vector3Int(_x, _y, 0), wallL);
                    }

                    if (tilemapWall.GetTile(new Vector3Int(_x + 1, _y)) != null)
                    {
                        tilemapWallRL.SetTile(new Vector3Int(_x, _y, 0), wallR);
                    }

                    if (tilemapWall.GetTile(new Vector3Int(_x, _y - 1)) != null)
                    {
                        tilemapWallUD.SetTile(new Vector3Int(_x, _y, 0), wallU);
                    }

                    if (tilemapWall.GetTile(new Vector3Int(_x, _y + 1)) != null)
                    {
                        tilemapWallUD.SetTile(new Vector3Int(_x, _y, 0), wallD);
                    }


                }
            }

        }
    }

    void SetEnemiesCount()
    {
        int allWeight = 0;
        for (int i = 0; i < enemy.Count; i++)
        {
            enemy[i].countSpawn = Mathf.FloorToInt(lvl * enemy[i].costSpawn);
            allWeight += enemy[i].countSpawn * enemy[i].weight;
        }
        netWeightRoom = Mathf.FloorToInt(allWeight / countCenterPointRoom) - 5;
    }

    void CreateEnemies(int _centerRoomX, int _centerRoomY, int _xMin, int _xMax, int _yMin, int _yMax)
    {

        int netWeight = netWeightRoom;

        for (int u = enemy.Count - 1; u >= 0; u--)
        {
            int count = UnityEngine.Random.Range(1, Mathf.CeilToInt(enemy[u].countSpawn / 5));
            if (enemy[u].countSpawn - count >= 0)
            {

            }
            else
            {
                count = enemy[u].countSpawn;
            }
            if (count > 0)
            {
                for (int o = 0; o < count; o++)
                {
                    CreateObjectsOnCoord(enemy[u].enemy, _centerRoomX + UnityEngine.Random.Range(_xMin + 2, _xMax - 2), _centerRoomY + UnityEngine.Random.Range(_yMin + 2, _yMax - 2), parentForEnemy);
                }
                netWeight -= count * enemy[u].weight;
                enemy[u].countSpawn -= count;
                countEnemies += count;
            }
        }
        if (netWeight > 0)
        {
            for (int u = 0; u < enemy.Count; u++)
            {
                if (netWeight > 0)
                {
                    int count = UnityEngine.Random.Range(1, Mathf.CeilToInt(enemy[u].countSpawn / 5));
                    if (enemy[u].countSpawn - count >= 0)
                    {

                    }
                    else
                    {
                        count = enemy[u].countSpawn;
                    }
                    if (count > 0)
                    {
                        for (int o = 0; o < count; o++)
                        {
                            CreateObjectsOnCoord(enemy[u].enemy, _centerRoomX + UnityEngine.Random.Range(_xMin + 2, _xMax - 2), _centerRoomY + UnityEngine.Random.Range(_yMin + 2, _yMax - 2), parentForEnemy);
                        }
                        netWeight -= count * enemy[u].weight;
                        enemy[u].countSpawn -= count;
                        countEnemies += count;
                    }
                }
            }
        }
    }

    GameObject CreateObjectsOnCoord(GameObject obj, float x, float y)
    {
        return Instantiate(obj, new Vector3(x / 2, y / 2, 0), Quaternion.identity);
    }

    GameObject CreateObjectsOnCoord(GameObject obj, float x, float y, Transform parent)
    {
        GameObject t = Instantiate(obj, new Vector3(x / 2, y / 2, 0), Quaternion.identity);
        t.transform.parent = parent;
        return t;
    }
}
