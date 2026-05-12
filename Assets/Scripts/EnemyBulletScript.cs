using System.Collections;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    public int damage;
    public bool canPenetrate;
    public bool isBreaking;
    public bool isPushing;
    float timeDestroy;
    GameObject bulletPrefab;

    public void SetSettings(int _damage, bool _canPenetrate, float _timeDestroy, bool _isPushing, bool _isBreaking, GameObject _bulletPrefab)
    {
        canPenetrate = _canPenetrate;
        damage = _damage;
        timeDestroy = _timeDestroy;
        isPushing = _isPushing;
        isBreaking = _isBreaking;
        bulletPrefab = _bulletPrefab;
        StartCoroutine(BulletDestroy());
    }

    public IEnumerator BulletDestroy()
    {
        while (true)
        {
            timeDestroy -= 1f;
            if (timeDestroy <= 0f) { ObjectDestroy(); }
            yield return new WaitForSeconds(1f);
        }
    }

    public void ObjectDestroy()
    {
        StopAllCoroutines();
        CentralizedObjectPool.instancePool.ReturnObject(bulletPrefab, gameObject);
    }

    public void Push(Collider2D collision)
    {
        var push = collision.transform.position - transform.position;
        push.Normalize();
        collision.attachedRigidbody.AddForce(push * 2, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>())
        {
            collision.GetComponent<PlayerStats>().RemoveHp(damage);
            Push(collision);
            ObjectDestroy();
        }
        else if (collision.tag == "Wall")
        {
            ObjectDestroy();
        }
        //else if (collision.tag == "PlayerBullet")
        //{
        //    if (isPushing)
        //    {
        //        collision.attachedRigidbody.linearVelocity = Vector2.zero;
        //        collision.GetComponent<SpriteRenderer>().flipX = !collision.GetComponent<SpriteRenderer>().flipX;
        //        Push(collision);
        //        EnemyBulletScript c = collision.gameObject.AddComponent<EnemyBulletScript>();
        //        c.SetSettings(damage, canPenetrate, timeDestroy, isPushing, isBreaking);
        //        Destroy(collision.GetComponent<BulletScript>());
        //    }
        //    if (isBreaking)
        //    {
        //        Destroy(collision.gameObject);
        //    }
        //}
    }
}
