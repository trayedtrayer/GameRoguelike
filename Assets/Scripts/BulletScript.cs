using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BulletScript : MonoBehaviour
{
    public int damage;
    public bool canPenetrate;
    public bool isBreaking;
    public bool isReflect;
    public bool isPushing;
    float timeDestroy;
    int layermask;
    public GameObject puffOnWall;
    private GameObject prefabBullet;

    private void Start()
    {
        layermask = (1 << LayerMask.NameToLayer("Wall"));
    }

    public void SetSettings(int _damage, bool _canPenetrate, float _timeDestroy, bool _isPushing, bool _isBreaking, bool _isReflect, GameObject _prefab)
    {
        canPenetrate = _canPenetrate;
        damage = _damage;
        timeDestroy = _timeDestroy;
        isPushing = _isPushing;
        isBreaking = _isBreaking;
        isReflect = _isReflect;
        prefabBullet = _prefab;
        StartCoroutine(BulletDestroy());
    }

    public void ReloadTime(float _timeDestroy)
    {
        timeDestroy = _timeDestroy;
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

    void ObjectDestroy()
    {
        StopAllCoroutines();
        CentralizedObjectPool.instancePool.ReturnObject(prefabBullet, gameObject);
    }

    public void Push(Collider2D collision)
    {
        var push = collision.transform.position - transform.position;
        push.Normalize();
        collision.attachedRigidbody.AddForce(push * 2, ForceMode2D.Impulse);
    }

    void Reflection()
    {
        Ray2D ray = new Ray2D(transform.position, transform.right);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 0.1f, layermask);
        if (hit.collider != null)
        {
            //Debug.DrawRay(transform.position, transform.right * 2, Color.blue, 2f);
            //print(hit.point);

            Vector2 pos = Vector2.Reflect(transform.right, hit.normal);
            Debug.DrawRay(hit.point, pos, Color.blue, 2f);
            float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            GetComponent<Rigidbody2D>().AddForce(transform.right * 2, ForceMode2D.Impulse);


            //Debug.DrawRay(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position - transform.position, Color.blue, 2f);
            //print(hit.collider.gameObject.name);
            //Debug.DrawRay(hit.point, hit.normal, Color.magenta, 1f);
        }
    }
    void Puff()
    {
        Ray2D ray = new Ray2D(transform.position, transform.right);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 0.4f, layermask);
        if (hit.collider != null)
        {
            Vector2 pos = -hit.normal;
            Debug.DrawRay(hit.point, pos, Color.blue, 2f);
            float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            Destroy(Instantiate(puffOnWall, hit.point, transform.rotation), 0.3f);
        }


    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            print(2);
            Reflection();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyScript>())
        {
            collision.GetComponent<EnemyScript>().RemoveHp(damage);
            Push(collision);
            ObjectDestroy();
        }
        else if (collision.tag == "Wall")
        {
            if (isReflect == false)
            {
                if (puffOnWall != null) Puff();
                ObjectDestroy();
            }
        }
        else if (collision.GetComponent<EnemyBulletScript>())
        {
            //if (isPushing)
            //{
            //    collision.attachedRigidbody.linearVelocity = Vector2.zero;
            //    collision.GetComponent<SpriteRenderer>().flipX = !collision.GetComponent<SpriteRenderer>().flipX;
            //    Push(collision);
            //    BulletScript c = collision.gameObject.AddComponent<BulletScript>();
            //    c.SetSettings(damage, canPenetrate, timeDestroy, isPushing, isBreaking, isReflect, prefabBullet);
            //    Destroy(collision.GetComponent<EnemyBulletScript>());
            //}
            if (isBreaking)
            {
                collision.GetComponent<EnemyBulletScript>().ObjectDestroy();
                //»«Ã≈Õ»“‹ Õ¿ ¬–¿∆≈— »’ œ”Àﬂ’ ◊≈–≈« Œ¡∆≈ “ œ”ÀÀ
                //»«Ã≈Õ»“‹ Õ¿ ¬–¿∆≈— »’ œ”Àﬂ’ ◊≈–≈« Œ¡∆≈ “ œ”ÀÀ
                //»«Ã≈Õ»“‹ Õ¿ ¬–¿∆≈— »’ œ”Àﬂ’ ◊≈–≈« Œ¡∆≈ “ œ”ÀÀ
                //»«Ã≈Õ»“‹ Õ¿ ¬–¿∆≈— »’ œ”Àﬂ’ ◊≈–≈« Œ¡∆≈ “ œ”ÀÀ
                //»«Ã≈Õ»“‹ Õ¿ ¬–¿∆≈— »’ œ”Àﬂ’ ◊≈–≈« Œ¡∆≈ “ œ”ÀÀ
            }

        }
        else if (collision.GetComponent<DecorativeObjects>())
        {
            collision.GetComponent<DecorativeObjects>().RemoveHp(damage);
            ObjectDestroy();
        }
    }
}
