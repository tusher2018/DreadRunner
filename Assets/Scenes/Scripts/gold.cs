using UnityEngine;

public class Gold : MonoBehaviour
{
    public int goldValue = 1;  // The value of gold that will be added to the player's gold count

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Increase player's gold count
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.AddGold(goldValue);
            }

            // Destroy the gold object
            DestroyGold();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            // If it collides with any other object (e.g., obstacle), destroy itself
            DestroyGold();
        }
    }

    private void DestroyGold()
    {
        Destroy(gameObject);
    }
}
