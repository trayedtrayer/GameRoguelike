using System.Collections;
using TMPro;
using UnityEngine;

public class RangeEnemyScript : EnemyScript
{
    Vector2 posToMove;
    Rigidbody2D rb;
    float moveSpeed;
    public float speed;
    Animator animator;
    public float distRun;
    public float timeRun;
    public float timeIdle;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(MovingEnum());
        FindMozg();
    }

    IEnumerator MovingEnum()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            moveSpeed = speed;
            posToMove = new Vector2(
            Random.Range(transform.position.x - distRun, transform.position.x + distRun),
            Random.Range(transform.position.x - distRun, transform.position.y + distRun));
            animator.SetBool("isRun", true);
            yield return new WaitForSeconds(timeRun);
            animator.SetBool("isRun", false);
            moveSpeed = 0f;
            yield return new WaitForSeconds(timeIdle);
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = (posToMove - (Vector2)transform.position).normalized;
        // Apply force to the Rigidbody2D to move towards the target
        rb.AddForce(direction * moveSpeed, ForceMode2D.Force);
    }
}
