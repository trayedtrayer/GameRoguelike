using System.Collections;
using UnityEngine;

public class WeaponMain : MonoBehaviour
{
    [Header("BulletSettings")]
    public int damageBullet;
    public GameObject bullet;
    public GameObject shotEffect;
    public bool canPenetrate;
    public float timeDestroy;
    public bool isPushing;
    public bool isBreaking;
    public bool isReflect;
    [Header("WeaponSettings")]
    public float timeDelayShot;
    public float spread;
    public float timeDelaySpray;
    public int countBullet;
    public float forceBullet;
    protected float timeDelayStartShoot;
    public float timeDelayStartShootMin;
    public float timeDelayStartShootMax;
    public Transform placeSpawn;
    public string weaponName;
    public GameObject prefabWeapon;
    public int lvlWeapon;
    public TypeWeapon type;
    public enum TypeWeapon
    {
        melee = 0,
        shotgun = 1,
        rifle = 2,
        miniguns = 3
    }
    Animator animator;
    [HideInInspector]
    public Sprite spriteWeapon;
    public bool canShoot = true;

    private void Start()
    {
        spriteWeapon = GetComponent<SpriteRenderer>().sprite;
        StartShooting();
    }

    public void WeaponLvlUp()
    {
        lvlWeapon += 1;
        damageBullet *= Mathf.CeilToInt(1 + (0.2f * lvlWeapon));
    }

    public void WeaponUpgrade(Crafting.Upgrade upgrade)
    {
        print(1);
        timeDelayStartShootMin = upgrade.timeDelayStartShootMin;        //увеличение времени "прогретости оружия"
        timeDelayStartShootMax = upgrade.timeDelayStartShootMax;        //уменьшение максимального времени для того чтобы прогреть оружие
        timeDelayShot = upgrade.timeDelayShot;                          //уменьшение времени задержки между выстрелами
        spread += upgrade.spread;                                        //увелчиение колиечство волн в выстрелах оружия
        timeDelaySpray = upgrade.timeDelaySpray;                        //уменьшение задержки меджу волнами выстрелов оружия
        countBullet += upgrade.countBullet;                              //увеличение кол-ва пуль в волне оружия
        forceBullet = upgrade.bulletPower;                              //увеличение скорости пули
        damageBullet = upgrade.damage;                                   //увеличение дамага
        lvlWeapon += 1;                                                  //увеличение лвла оружия для статистики
    }

    public virtual void StartShooting()
    {
        //start
    }

    public void Shoot()
    {
        StartCoroutine(Shoots());

    }

    protected IEnumerator Shoots()
    {
        if (shotEffect != null) Destroy(Instantiate(shotEffect, placeSpawn.transform.position, transform.rotation), 0.2f);
        for (int i = 0; i < countBullet; i++)
        {
            print(1);
            var _b = CentralizedObjectPool.instancePool.GetObject(bullet);
            _b.transform.position = placeSpawn.position;
            _b.transform.rotation = placeSpawn.rotation;//bullet, placeSpawn.position, placeSpawn.rotation);
            _b.transform.Rotate(0, 0, Random.Range(-spread, spread));
            if (_b.GetComponent<Rigidbody2D>())
            {
                _b.GetComponent<Rigidbody2D>().AddForce(_b.transform.right * forceBullet);
            }
            if (_b.GetComponent<BulletScript>())
            {
                _b.gameObject.GetComponent<BulletScript>().SetSettings(damageBullet, canPenetrate, timeDestroy, isPushing, isBreaking, isReflect, bullet);
            }
            yield return new WaitForSeconds(timeDelaySpray);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            canShoot = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            canShoot = true;
        }
    }
}
