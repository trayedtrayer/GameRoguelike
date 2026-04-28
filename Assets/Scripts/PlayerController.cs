using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float speed;
    float moveHorizontal;
    float moveVertical;
    Rigidbody2D rb2d;
    bool canMove = true;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (canMove)
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
            if (moveHorizontal != 0 || moveVertical != 0)
            {
                //anim
            }
            else
            {
                //anim
            }
        }
    }

    void FixedUpdate()
    {
        rb2d.linearVelocity = new Vector2(moveHorizontal * speed, moveVertical * speed);   
    }
}
