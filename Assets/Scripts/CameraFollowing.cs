using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    Transform objectToFollow;
    public float speed;
    private void Start()
    {
        objectToFollow = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
    }
    void FixedUpdate()
    {
        if (objectToFollow != null)
        {
            Vector3 position = new Vector3(objectToFollow.position.x, objectToFollow.position.y, -10);
            transform.position = Vector3.Lerp(transform.position, position, speed * Time.deltaTime);
        }
    }

    public void SetObjectFollowing(Transform _transform)
    {
        objectToFollow = _transform;
    }
}
