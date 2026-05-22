using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int hp;
    public GameObject deadBody;
    MozgGeneration mozg;
    public GameObject randItem;
    [HideInInspector]
    protected bool isFound;
    public float speed;

    private void Start()
    {
        FindMozg();
    }

    public void FindMozg()
    {
        mozg = GameObject.Find("Grid").GetComponent<MozgGeneration>();
        mozg.AddEnemy();
    }

    public void RemoveHp(int _damage)
    {
        hp -= _damage;
        GetComponentInChildren<Animator>().Play("Hit");
        if (hp <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        Instantiate(deadBody, transform.position, transform.rotation);
        if (Random.Range(0, 101) > 50 + UpgradeManager.Instance.bonusDropPercent)
        {
            GameObject t = Instantiate(randItem, transform.position, transform.rotation);
            t.GetComponent<ItemScript>().SetItem(DataBase.ReturnRandomItem());
            t.GetComponent<MagneticObjects>().SetSettings(Random.Range(1, 10), Mathf.FloorToInt(UpgradeManager.Instance.bonusHpRegenPerKill), Random.Range(1, 2));
        }
        mozg.EnemyKill();
        Destroy(gameObject);
    }

    public void SetFound()
    {
        isFound = true;
    }
}
