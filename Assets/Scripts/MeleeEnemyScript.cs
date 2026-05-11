using System.Collections;
using UnityEngine;

public class MeleeEnemyScript : EnemyScript
{
    public float speed;

    public GameObject target;
    Vector2 posToMove;
    Rigidbody2D rb;
    public float moveSpeed;
    Animator animator;
    public float stoppingDistance;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(MovingEnum());
        FindMozg();
    }
    IEnumerator MovingEnum()
    {
        while (true)
        {
            moveSpeed = 0f;
            animator.Play("Idle");
            yield return new WaitForSeconds(1f);
            moveSpeed = speed;
            animator.Play("Run");
            yield return new WaitForSeconds(6f);

        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            if (Vector2.Distance((Vector2)target.transform.position, (Vector2)transform.position) > stoppingDistance)
            {
                Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
                rb.AddForce(direction * moveSpeed, ForceMode2D.Force);
            }
        }
    }
}