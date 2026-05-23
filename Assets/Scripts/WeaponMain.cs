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
    public enum TypeWeapon { melee = 0, shotgun = 1, rifle = 2, miniguns = 3 }

    public Sprite spriteWeapon;
    public bool canShoot = true;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioSource audioSource;

    private void Start()
    {
        spriteWeapon = GetComponent<SpriteRenderer>().sprite;


        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartShooting();
    }

    public virtual void StartShooting() { }

    public void AllCorous()
    {
        StopAllCoroutines();
        StartShooting();
    }

    public void Shoot() => StartCoroutine(Shoots());

    protected IEnumerator Shoots()
    {
        
        if (shootSound != null && audioSource != null)
            audioSource.PlayOneShot(shootSound);

        UpgradeManager mgr = UpgradeManager.Instance;

        int finalDamage = CalcFinalDamage(mgr);
        int finalCount = CalcFinalCountBullet(mgr);
        float finalSpread = CalcFinalSpread(mgr);
        float finalForce = CalcFinalForce(mgr);
        bool finalPenetrate = canPenetrate || (mgr?.bonusPenetration ?? false);
        bool finalReflect = isReflect || (mgr?.bonusReflect ?? false);

        if (shotEffect != null)
            Destroy(Instantiate(shotEffect, placeSpawn.transform.position, transform.rotation), 0.2f);

        print(finalCount);

        for (int i = 0; i < finalCount; i++)
        {
            SpawnBullet(finalDamage, finalSpread, finalForce, finalPenetrate, finalReflect);

            if (mgr != null && mgr.skill_P2_TwinShotChance > 0)
                if (Random.Range(0f, 100f) < mgr.skill_P2_TwinShotChance)
                    SpawnBullet(finalDamage, finalSpread + 5f, finalForce, finalPenetrate, finalReflect);

            yield return new WaitForSeconds(timeDelaySpray);
        }
    }

    private void SpawnBullet(int damage, float currentSpread, float force, bool penetrate, bool reflect)
    {
        var _b = CentralizedObjectPool.instancePool.GetObject(bullet);
        _b.transform.position = placeSpawn.position;
        _b.transform.rotation = placeSpawn.rotation;
        _b.transform.Rotate(0, 0, Random.Range(-currentSpread, currentSpread));
        print("123123123");
        var rb = _b.GetComponent<Rigidbody2D>();
        if (rb != null) rb.AddForce(_b.transform.right * force);

        var bs = _b.GetComponent<BulletScript>();
        if (bs != null)
        {
            bs.SetSettings(damage, penetrate, timeDestroy, isPushing, isBreaking, reflect, bullet);
            bs.onHitCallback = OnBulletHit;
        }

        var ebs = _b.GetComponent<EnemyBulletScript>();
        if (ebs != null)
            ebs.SetSettings(damage, penetrate, timeDestroy, isPushing, isBreaking, bullet);
    }

    public void OnBulletHit(GameObject hitObject, int baseDamage, Vector3 hitPosition)
    {
        UpgradeManager mgr = UpgradeManager.Instance;
        if (mgr == null || hitObject == null) return;

        EnemyScript enemy = hitObject.GetComponent<EnemyScript>();
        if (enemy == null) return;

        if (mgr.bonusCritChancePercent > 0 && Random.Range(0f, 100f) < mgr.bonusCritChancePercent)
            enemy.RemoveHp(baseDamage);

        if (mgr.bonusExplosionChance > 0 && Random.Range(0f, 100f) < mgr.bonusExplosionChance)
        {
            foreach (var col in Physics2D.OverlapCircleAll(hitPosition, 2f))
            {
                var e = col.GetComponent<EnemyScript>();
                if (e != null && e.gameObject != hitObject)
                    e.RemoveHp(Mathf.RoundToInt(baseDamage * 0.5f));
            }
        }

        if (mgr.bonusBurnChance > 0 && Random.Range(0f, 100f) < mgr.bonusBurnChance)
        {
            var burn = hitObject.GetComponent<BurnEffect>();
            if (burn != null) burn.Refresh(); else hitObject.AddComponent<BurnEffect>();
        }

        if (mgr.bonusFreezeChance > 0 && Random.Range(0f, 100f) < mgr.bonusFreezeChance)
        {
            var freeze = hitObject.GetComponent<FreezeEffect>();
            if (freeze != null) freeze.Refresh(); else hitObject.AddComponent<FreezeEffect>();
        }
    }

    private int CalcFinalDamage(UpgradeManager mgr)
    {
        if (mgr == null) return damageBullet;
        print(damageBullet * (1f + mgr.bonusDamagePercent / 100f));
        float dmg = 1f + damageBullet * (1f + mgr.bonusDamagePercent / 100f);
        print(dmg);
        if (type == TypeWeapon.rifle) dmg *= 1f + mgr.bonusRifleDamagePercent / 100f;
        return Mathf.Max(1, Mathf.CeilToInt(dmg));
    }

    private int CalcFinalCountBullet(UpgradeManager mgr)
    {
        int result = countBullet;
        if (mgr != null && type == TypeWeapon.shotgun) result += mgr.bonusShotgunCountBullet;
        return Mathf.Max(1, result);
    }

    private float CalcFinalSpread(UpgradeManager mgr)
    {
        float s = spread;
        if (mgr != null && type == TypeWeapon.shotgun) s += mgr.bonusShotgunSpread;
        return s;
    }

    private float CalcFinalForce(UpgradeManager mgr)
    {
        if (mgr == null) return forceBullet;
        return forceBullet * (1f + mgr.bonusBulletSpeedPercent / 100f);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{ if (collision.tag == "Wall") canShoot = false; }

    //private void OnTriggerExit2D(Collider2D collision)
    //{ if (collision.tag == "Wall") canShoot = true; }
}