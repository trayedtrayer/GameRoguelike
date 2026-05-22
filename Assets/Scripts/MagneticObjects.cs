using UnityEngine;
using System.Collections;

public class MagneticObjects : MonoBehaviour
{
    public int expAdd;
    public int hpAdd;
    public int money;
    Transform target;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStats>().AddFunds(expAdd, hpAdd, money);
            if (gameObject.GetComponent<ItemScript>())
            {
                collision.GetComponent<PlayerStats>().AddItem(gameObject.GetComponent<ItemScript>().item);
            }
            Destroy(gameObject);
        }
    }
}
