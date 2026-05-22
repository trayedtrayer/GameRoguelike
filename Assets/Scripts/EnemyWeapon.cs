using UnityEngine;
using System.Collections;

public class EnemyWeapon : MonoBehaviour
{
    [HideInInspector]
    public GameObject target;
    public SpriteRenderer skin;
    public SpriteRenderer weapon;
    public SpriteRenderer weapon2;
    WeaponEnemy weaponEnemy;
    LayerMask layerMask;
    Transform player;

    void Start()
    {
        weaponEnemy = GetComponentInChildren<WeaponEnemy>();
        player = Player.playerObject.transform;
        layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Wall"));
        StartCoroutine(EnemyShoot());
    }

    void Update()
    {
        if (target != null)
        {
            var enemyPos = target.transform.position;
            var angle = Vector2.Angle(Vector2.right, enemyPos - transform.position);
            transform.eulerAngles = new Vector3(0, 0, transform.position.y < enemyPos.y ? angle : -angle);
            if (angle > 90)
            {
                skin.flipX = true;
                weapon.flipY = true;
                if (weapon2 != null)
                {
                    weapon2.flipY = true;
                }
            }
            else
            {
                skin.flipX = false;
                weapon.flipY = false;
                if (weapon2 != null)
                {
                    weapon2.flipY = false;
                }
            }
        }
    }

    GameObject CheckByRay()
    {
        if (player != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, player.position - transform.position, 6f, layerMask);
            // Debug.DrawRay(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position - transform.position, Color.blue, 2f);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Wall")
                {
                    return null;
                }
                else if (hit.collider.tag == "Player")
                {
                    return hit.collider.gameObject;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    IEnumerator EnemyShoot()
    {
        while (true)
        {
            if (target != null)
            {
                weaponEnemy.Shooting();
                yield return new WaitForSeconds(weaponEnemy.timeDelayShot);
            }
            else
            {
                target = CheckByRay();
                if (GetComponentInParent<EnemyScript>())
                {
                    GetComponentInParent<EnemyScript>().SetFound();
                }
                if (GetComponentInParent<MeleeEnemyScript>())
                {
                    GetComponentInParent<MeleeEnemyScript>().target = target;
                }
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
