using System.Collections;
using TMPro;
using UnityEngine;

public class RangeEnemyScript : EnemyScript
{
    Vector2 posToMove;
    Rigidbody2D rb;
    float moveSpeed;
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
        moveSpeed = speed;
    }

    IEnumerator MovingEnum()
    {
        yield return new WaitForSeconds(1f);
        while (!isFound)
        {
            yield return new WaitForSeconds(0.2f);
        }
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            speed = moveSpeed;
            posToMove = new Vector2(
            Random.Range(transform.position.x - distRun, transform.position.x + distRun),
            Random.Range(transform.position.x - distRun, transform.position.y + distRun));
            animator.SetBool("isRun", true);
            yield return new WaitForSeconds(timeRun);
            animator.SetBool("isRun", false);
            speed = 0f;
            yield return new WaitForSeconds(timeIdle);
        }
    }

    private void FixedUpdate()
    {

        if (moveSpeed == 0f)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = 0;
        }
        else
        {
            Vector2 direction = (posToMove - (Vector2)transform.position).normalized;
            rb.AddForce(direction * moveSpeed, ForceMode2D.Force);
        }
        }
    }
