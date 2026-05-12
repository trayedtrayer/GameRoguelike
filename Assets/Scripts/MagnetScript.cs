using UnityEngine;

public class MagnetScript : MonoBehaviour
{
    public float magnetStrength = 3f;
    public float magnetRadius = 5f;
    LayerMask layerMask;

    private void Start()
    {
        layerMask = (1 << LayerMask.NameToLayer("Magnetic"));
    }

    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, magnetRadius, layerMask);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<MagneticObjects>())
            {
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                Vector2 direction = (Vector2)transform.position - rb.position;
                float distance = direction.magnitude;
                if (distance > 0.1f)
                {
                    Vector2 force = direction.normalized * (magnetStrength / distance);
                    rb.AddForce(force);
                }
            }
        }
    }
}
