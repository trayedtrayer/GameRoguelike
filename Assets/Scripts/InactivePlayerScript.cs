using UnityEngine;

public class InactivePlayerScript : MonoBehaviour
{
    public int inactivePlayerId;
    GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStats>().SetPlayerText(inactivePlayerId);
            player = collision.gameObject;
        }
        print(collision.tag);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerStats>().SetPlayerText(-1);
            player = null;
        }
    }

    public void Update()
    {
        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                player.GetComponent<PlayerStats>().SwapPlayer(inactivePlayerId, transform);
            }
        }
    }
}
