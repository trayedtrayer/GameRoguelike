using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class XmlSaver : MonoBehaviour
{
    [System.Serializable]
    public class GameStats
    {
        public string weaponOneName;
        public string weaponTwoName;
        public int hp;
        public float shield;
        public int xp;
        public int money;
        public int xpForNewLvl;
        public int lvl;
        public int idPlayer;
        public int currentLvl;
        public List<DataBase.Item> items;
        public List<bool> objectsActive;
    }

    bool isLoading;
    GameObject gameObj;
    public Image eButton;

    public void SetNewLevel(GameObject player, Image load, int buildIndex)
    {
        if (!isLoading)
        {
            StartCoroutine(PlayerLoad(player, load, buildIndex));
        }
    }

    private void Update()
    {
        if (gameObj != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SetNewLevel(gameObj, gameObj.GetComponent<PlayerStats>().loadBar, 1);
            }
        }
    }

    IEnumerator PlayerLoad(GameObject player, Image load, int buildIndex)
    {
        load.gameObject.SetActive(true);
        player.GetComponent<PlayerStats>().CompleteLevel();
        yield return new WaitForSeconds(0.02f);
        isLoading = true;
        Write(player.GetComponentInChildren<PlayerStats>(), player.GetComponentInChildren<Hand>());
        AsyncOperation ap = SceneManager.LoadSceneAsync(buildIndex);
        Time.timeScale = 0f;
        while (!ap.isDone)
        {
            load.fillAmount = Mathf.Clamp01(ap.progress / .9f);
            yield return null;
        }
        isLoading = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>())
        {
            gameObj = collision.gameObject;
            eButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>())
        {
            eButton.gameObject.SetActive(false);
            gameObj = null;
        }
    }

    public static void Write(PlayerStats playerStats, Hand hand)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "save.xml");
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Dispose();
        }
        GameStats stats = new GameStats
        {
            weaponOneName = hand.GunOne.GetComponentInChildren<WeaponMain>().weaponName,
            weaponTwoName = hand.GunTwo.GetComponentInChildren<WeaponMain>().weaponName,
            hp = playerStats.GetHp(),
            shield = playerStats.GetShield(),
            xp = playerStats.GetXp(),
            xpForNewLvl = playerStats.GetXpForNewLvl(),
            lvl = playerStats.GetLvl(),
            currentLvl = playerStats.GetCurrentGameLvl(),
            idPlayer = playerStats.GetPlayerId(),
            items = playerStats.GetPlayerInventory(),
            money = playerStats.GetMoneyCount(),
            objectsActive = playerStats.PlayerBuilds()
        };
        print(stats.weaponOneName);
        print(stats.weaponTwoName);
        XmlSerializer serializer = new XmlSerializer(typeof(GameStats));
        using (TextWriter writer = new StreamWriter(Path.Combine(Application.persistentDataPath, "save.xml")))
        {
            serializer.Serialize(writer, stats);
        }
    }

    public static GameStats Read()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "save.xml");
        if (File.Exists(filePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameStats));

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return serializer.Deserialize(stream) as GameStats;
            }
        }
        else
        {
            Debug.Log("Файл сохранения не найден.");
            return null;
        }
    }
}
