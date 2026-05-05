using System.Collections;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    public int damage;
    public bool canPenetrate;
    public bool isBreaking;
    public bool isPushing;
    float timeDestroy;


    public void SetSettings(int _damage, bool _canPenetrate, float _timeDestroy, bool _isPushing, bool _isBreaking)
    {
        canPenetrate = _canPenetrate;
        damage = _damage;
        timeDestroy = _timeDestroy;
        isPushing = _isPushing;
        isBreaking = _isBreaking;
        StartCoroutine(BulletDestroy());
    }

    public IEnumerator BulletDestroy()
    {
        while (true)
        {
            timeDestroy -= 1f;
            if (timeDestroy <= 0f) { Destroy(gameObject); }
            yield return new WaitForSeconds(1f);
        }
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
            Destroy(gameObject);
        }
        else if (collision.tag == "Wall")
        {
            Destroy(gameObject);
        }
        if (isBreaking)
        {
            Destroy(collision.gameObject);
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
