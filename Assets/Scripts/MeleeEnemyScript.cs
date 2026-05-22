using System.Collections;
using UnityEngine;

public class MeleeEnemyScript : EnemyScript
{
    public GameObject target;
    Vector2 posToMove;
    Rigidbody2D rb;
    float moveSpeed;
    Animator animator;
    public float stoppingDistance;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(MovingEnum());
        FindMozg();
        moveSpeed = speed;
    }
    IEnumerator MovingEnum()
    {
        while (true)
        {
            speed = 0f;
            animator.Play("Idle");
            yield return new WaitForSeconds(1f);
            speed = moveSpeed;
            animator.Play("Run");
            yield return new WaitForSeconds(6f);

        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            if(moveSpeed == 0f)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = 0;
            }
            if (Vector2.Distance((Vector2)target.transform.position, (Vector2)transform.position) > stoppingDistance)
            {
                Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
                rb.AddForce(direction * moveSpeed, ForceMode2D.Force);
            }
        }
    }
}