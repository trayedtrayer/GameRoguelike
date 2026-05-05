using System.Collections;
using UnityEngine;

public static class StatsTimer
{
    public static float time;
    public static int enemyKilled;
}

public class MozgGeneration : MonoBehaviour
{
    public GameObject portal;
    public Transform portalOld;
    public int enemyCount;
    public GameObject player;
    public int minCountEnemy;
    public float time;
    public int enemyKilled;
    public void EnemyKill()
    {
        enemyCount--;
        StatsTimer.enemyKilled += 1;
        if (enemyCount <= minCountEnemy)
        {
            player.GetComponent<PlayerStats>().SetArrows(GameObject.FindGameObjectsWithTag("Enemy"));
        }
        if (enemyCount <= 0)
        {
            Invoke("SetPortal", 1f);
        }
    }
    private void Start()
    {
        StartCoroutine(Timer());
    }
    void SetPortal()
    {
        Instantiate(portal, portalOld.transform.position, portalOld.transform.rotation);
        Destroy(portalOld.gameObject);
    }
    public void AddEnemy()
    {
        enemyCount += 1;
    }
    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            StatsTimer.time += 1;
        }
    }
}
