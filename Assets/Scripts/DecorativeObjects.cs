using UnityEngine;

public class DecorativeObjects : MonoBehaviour
{
    public int hp;
    public GameObject destroyedObject;

    public void RemoveHp(int _damage)
    {
        hp -= _damage;
        if (hp <= 0)
        {
            GameObject t = Instantiate(destroyedObject, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
