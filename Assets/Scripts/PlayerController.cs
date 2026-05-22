using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    float moveHorizontal;
    float moveVertical;
    public Animator animator;
    Rigidbody2D rb2d;
    public bool isRoll;
    PlayerStats playerStats;
    public float timeDilation;
    public float timeDilationRollMax;
    public float timeRoll;
    public float speedMultiplierRoll;
    ParticleSystem particle;
    bool spB;
    bool isBlown;
    bool canMove = true;
    public GameObject parentStatusBar;
    public GameObject prefabCooldown;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        particle = GetComponentInChildren<ParticleSystem>();
        print(particle.gameObject.name);
        particle.Stop();
    }

    void Update()
    {
        if (canMove)
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
            if (moveHorizontal != 0 || moveVertical != 0)
            {
                animator.SetBool("isRun", true);
            }
            else
            {
                animator.SetBool("isRun", false);
            }
            if (Input.GetMouseButtonDown(1) && !isRoll && !(moveHorizontal == 0 && moveVertical == 0))
            {
                StartCoroutine(Rollin());
            }
        }
    }

    IEnumerator Rollin()
    {
        print("YAHA");
        isRoll = true;
        float _sp = speed;
        var push = new Vector2(moveHorizontal, moveVertical);
        push = push.normalized;
        moveHorizontal = push.x;
        moveVertical = push.y;
        speed *= speedMultiplierRoll;
        playerStats.BecomeImmortal();
        particle.Play();
        canMove = false;
        yield return new WaitForSeconds(timeRoll);
        speed = _sp;
        playerStats.BecomeMortal();
        particle.Stop();
        canMove = true;
        Image t = Instantiate(prefabCooldown, parentStatusBar.transform).GetComponent<Image>();
        timeDilation = timeDilationRollMax;
        while (true)
        {
            if (timeDilation > 0)
            {
                timeDilation -= 0.02f;
                t.fillAmount = timeDilation / timeDilationRollMax;
                yield return new WaitForSeconds(0.02f);
            }
            else
            {
                Destroy(t.gameObject);
                break;
            }
        }
        isRoll = false;
    }

    public void ChangeSpeedV(float _speed)
    {
        spB = true;
        StartCoroutine(ChangeSpeed(_speed));
    }

    IEnumerator ChangeSpeed(float _speed)
    {
        float _sp = speed;
        speed = _speed;
        while (spB)
        {
            yield return new WaitForSeconds(0.02f);
        }
        speed = _sp;
    }

    public void StopChangeSpeed()
    {
        spB = false;
    }

    public void BlewUp()
    {
        isBlown = true;
        Invoke("StopBlew", 1f);
    }

    public void StopBlew()
    {
        isBlown = false;
    }

    void FixedUpdate()
    {
        if (!isBlown)
        {
            rb2d.linearVelocity = new Vector2(moveHorizontal * speed, moveVertical * speed);
        }
        else
        {
            rb2d.AddForce(new Vector2(moveHorizontal > 1 ? 1 : moveHorizontal, moveVertical > 1 ? 1 : moveVertical).normalized * speed, ForceMode2D.Force);
        }
    }

    public void Apply(float bonusSpeedPercent)
    {
        speed = bonusSpeedPercent == 0 ? speed : speed * (1 + bonusSpeedPercent / 100f);
    }
}
