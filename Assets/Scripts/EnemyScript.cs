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
    public float distanceToEnableCulling;

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
        GameObject t = Instantiate(randItem, transform.position, transform.rotation);
        t.GetComponent<ItemScript>().SetItem(DataBase.ReturnRandomItem());
        mozg.EnemyKill();
        Destroy(gameObject);

    }
}
