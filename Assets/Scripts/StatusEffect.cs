using System.Collections;
using UnityEngine;

public class BurnEffect : MonoBehaviour
{
    public int damagePerTick = 5;
    public float tickInterval = 0.5f;
    public float duration = 3f;
    private float elapsed = 0f;
    public void Refresh()
    {
        elapsed = 0f;
    }

    private void Start()
    {
        StartCoroutine(BurnTick());
    }

    private IEnumerator BurnTick()
    {
        while (elapsed < duration)
        {
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;

            var enemy = GetComponent<EnemyScript>();
            if (enemy != null)
                enemy.RemoveHp(damagePerTick);
            else
            {
                Destroy(this);
                yield break;
            }
        }
        Destroy(this);
    }
}

public class FreezeEffect : MonoBehaviour
{
    public float slowPercent = 0.6f; 
    public float duration = 2f;
    private bool applied = false;

    public void Refresh() { }

    private void Start()
    {
        if (applied) return;
        applied = true;
        StartCoroutine(ApplyFreeze());
    }

    private IEnumerator ApplyFreeze()
    {
        var enemy = GetComponent<EnemyScript>();
        if (enemy != null)
        {
            float speed = enemy.speed;
            enemy.speed = 0f;
            yield return new WaitForSeconds(duration);
            enemy.speed = speed;
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }

        Destroy(this);
    }
}