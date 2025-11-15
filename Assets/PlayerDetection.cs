using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private TicketControllerAI controller;

    private void Awake()
    {
        controller = GetComponentInParent<TicketControllerAI>();
        if (controller == null)
        {
            Debug.LogError("PlayerDetection: No TicketControllerAI found in parent!");
        }
        else
        {
            Debug.Log($"PlayerDetection on {name}: controller = {controller.name}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Is this the player?
        var playerMovement = other.GetComponentInParent<PlayerMovement>();
        if (playerMovement == null) return;

        // Only count as a "fine" if this controller is already chasing
        if (controller != null && controller.IsChasing)
        {
            Debug.Log($"{name}: Player touched while CHASING → fine!");
            controller.OnPlayerCaught();
        }
        else
        {
            // Just for debugging, to see that the trigger works
            Debug.Log($"{name}: Player touched, but controller not chasing (state = {controller})");
        }
    }
}