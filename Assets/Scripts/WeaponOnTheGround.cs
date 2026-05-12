using UnityEngine;
using UnityEngine.UI;

public class WeaponOnTheGround : MonoBehaviour
{
    public GameObject prefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerStats>().SetHandText(this);
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerStats>().SetHandText(null);
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
