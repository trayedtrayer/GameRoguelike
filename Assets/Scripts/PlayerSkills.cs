using System.Collections;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    private PlayerStats ps;
    private PlayerController pc;
    private bool isActive = false;

    private float baseSpeedMultiplier;
    private const float TRIGGER_HP_PERCENT = 0.30f;
    private const float BOOST = 1.50f;

    private void Start()
    {
        ps = GetComponent<PlayerStats>();
        pc = GetComponent<PlayerController>();
        baseSpeedMultiplier = pc.speed;
    }

    private void Update()
    {
        if (UpgradeManager.Instance == null || !UpgradeManager.Instance.skill_P1_BerserkMode)
        {
            if (isActive) DeactivateBerserk();
            return;
        }

        float hpRatio = (float)ps.GetHp() / ps.maxHp;

        if (hpRatio < TRIGGER_HP_PERCENT && !isActive)
            ActivateBerserk();
        else if (hpRatio >= TRIGGER_HP_PERCENT && isActive)
            DeactivateBerserk();
    }

    private void ActivateBerserk()
    {
        isActive = true;
        if (pc != null) pc.speed *= BOOST;
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.bonusDamagePercent += 50f;
        Debug.Log("[P1] БЕРСЕРК АКТИВИРОВАН");
    }

    private void DeactivateBerserk()
    {
        isActive = false;
        if (pc != null) pc.speed /= BOOST;
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.bonusDamagePercent -= 50f;
        Debug.Log("[P1] Берсерк снят");
    }

    private void OnDestroy()
    {
        if (isActive && UpgradeManager.Instance != null)
            UpgradeManager.Instance.bonusDamagePercent -= 50f;
    }
}

public class Skill_P1_IronSkin : MonoBehaviour
{
    public int maxStacks = 3;
    private int killCounter = 0;
    private int armorStacks = 0;

    public int ArmorStacks => armorStacks;

    private PlayerStats ps;

    private void Start()
    {
        ps = GetComponent<PlayerStats>();
    }

    public void OnEnemyKilled()
    {
        if (UpgradeManager.Instance == null || !UpgradeManager.Instance.skill_P1_IronSkin) return;
        killCounter++;
        if (killCounter >= 5)
        {
            killCounter = 0;
            if (armorStacks < maxStacks)
            {
                armorStacks++;
                Debug.Log($"[P1] IronSkin стак: {armorStacks}/{maxStacks}");
            }
        }
    }

    public bool TryAbsorbHit()
    {
        if (!UpgradeManager.Instance.skill_P1_IronSkin) return false;
        if (armorStacks <= 0) return false;
        armorStacks--;
        Debug.Log($"[P1] IronSkin поглотил удар. Стаков: {armorStacks}");
        return true;
    }
}

//public class Skill_P2_ShadowStep : MonoBehaviour
//{
//    public float dashDistance = 5f;
//    public float dashDamage = 20f;
//    public float cooldown = 8f;
//    public LayerMask enemyLayer;
//    private float cooldownTimer = 0f;
//    private bool isReady = true;

//    public float CooldownProgress => isReady ? 1f : (cooldown - cooldownTimer) / cooldown;

//    private void Update()
//    {
//        if (UpgradeManager.Instance == null || !UpgradeManager.Instance.skill_P2_ShadowStep) return;

//        if (!isReady)
//        {
//            cooldownTimer += Time.deltaTime;
//            if (cooldownTimer >= cooldown)
//            {
//                isReady = true;
//                cooldownTimer = 0f;
//            }
//        }

//        if (isReady && Input.GetKeyDown(KeyCode.LeftShift))
//            PerformDash();
//    }

//    private void PerformDash()
//    {
//        isReady = false;
//        cooldownTimer = 0f;

//        // Направление — к курсору
//        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        mouseWorld.z = 0;
//        Vector3 dir = (mouseWorld - transform.position).normalized;

//        // Рывок
//        transform.position += dir * dashDistance;

//        // Урон по врагам в радиусе 1.5 вокруг конечной точки
//        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.5f, enemyLayer);
//        foreach (var h in hits)
//        {
//            var enemy = h.GetComponent<EnemyScript>();
//            if (enemy != null) enemy.RemoveHp(Mathf.RoundToInt(dashDamage));
//        }

//        Debug.Log("[P2] ShadowStep!");
//    }
//}


public class Skill_P2_TwinShot : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("[P2] TwinShot готов. Шанс: " +
                  (UpgradeManager.Instance?.skill_P2_TwinShotChance ?? 0) + "%");
    }
}