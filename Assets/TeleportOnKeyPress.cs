using UnityEngine;

public class TeleportOnKeyPress : MonoBehaviour
{
    [SerializeField]
    public Vector3 teleportTarget; // The target position where the player will teleport

    [SerializeField]
    public KeyCode teleportKey = KeyCode.Z; // The key to trigger teleportation
    private bool canTeleport; // Flag to check if the player can teleport

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = true; // Set canTeleport to true when the player enters the trigger zone
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = false; // Set canTeleport to false when the player exits the trigger zone
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            canTeleport = true; // Set canTeleport to true when the player enters the trigger zone
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            canTeleport = false; // Set canTeleport to false when the player exits the trigger zone
        }
    }

    private void Update()
    {
        // Check if the player can teleport and if the teleport key is pressed
        if (canTeleport && Input.GetKeyDown(teleportKey))
        {
            TeleportPlayer(); // Teleport the player to the specified target position
        }
    }

    private void TeleportPlayer()
    {
        if (teleportTarget != null)
        {
            // Teleport the player to the teleportTarget position
            GameObject.FindGameObjectWithTag("Player").transform.position = teleportTarget;
        }
        else
        {
            Debug.LogWarning("Teleport target is not assigned!"); // Log a warning if teleportTarget is not assigned
        }
    }
}