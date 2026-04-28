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
            if (timeDestroy <= 0f) { CentralizedObjectPool.instancePool.ReturnObject(prefabBullet,gameObject); }
            yield return new WaitForSeconds(1f);
        }
    }

    public void Push(Collider2D collision)
    {
        var push = collision.transform.position - transform.position;
        push.Normalize();
        collision.attachedRigidbody.AddForce(push * 2, ForceMode2D.Impulse);
    }
}
