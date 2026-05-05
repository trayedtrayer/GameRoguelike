using UnityEngine;
using System.Collections;

public class MagneticObjects : MonoBehaviour
{
    public int expAdd;
    public int hpAdd;
    public float speed;
    Transform target;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStats>().AddFunds(expAdd, hpAdd);
            if (gameObject.GetComponent<ItemScript>())
            {
                collision.GetComponent<PlayerStats>().AddItem(gameObject.GetComponent<ItemScript>().item);
            }
            Destroy(gameObject);
        }
    }

    public void StartLerp(Transform _target)
    {
        target = _target;
        gameObject.layer = 0;
        StartCoroutine(LerpStart());
    }

    IEnumerator LerpStart()
    {
        while (target != null)
        {
            Vector3 position = new Vector3(target.position.x, target.position.y, 0);
            transform.position = Vector3.Lerp(transform.position, position, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
