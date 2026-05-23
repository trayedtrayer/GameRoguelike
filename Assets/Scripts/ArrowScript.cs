using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public GameObject enemy;
    public float offsetFromPlayer = 1.5f;

    void Update()
    {
        if (enemy != null)
        {
            Vector3 directionToEnemy = (enemy.transform.position - transform.parent.position).normalized;
            transform.position = transform.parent.position + directionToEnemy * offsetFromPlayer;
            float angle = Vector2.Angle(Vector2.right, directionToEnemy);
            transform.eulerAngles = new Vector3(0, 0, directionToEnemy.y < 0 ? -angle : angle);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
