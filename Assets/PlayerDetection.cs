using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private TicketControllerAI controller;
    public void Awake()
    {
        // This script lives on a child of the controller
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
        Debug.Log($"PlayerDetection on {name}: OnTriggerEnter2D with {other.name}");

                // Option 1: detect by PlayerMovement (component on the player root)
        var playerMovement = other.GetComponentInParent<PlayerMovement>();


        if (playerMovement != null)
        {
            Debug.Log($"{name}: Player detected, telling controller to chase.");
            controller?.OnPlayerDetected();
        }
    }
}
