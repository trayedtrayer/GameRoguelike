using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public GameObject enemy;
    void Update()
    {
        if (enemy != null)
        {
            var mousePosition = enemy.transform.position;
            var angle = Vector2.Angle(Vector2.right, enemy.transform.position - GetComponentInParent<PlayerStats>().GetComponent<Transform>().position);
            transform.eulerAngles = new Vector3(0, 0, transform.position.y < mousePosition.y ? angle : -angle);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
