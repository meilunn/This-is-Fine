using UnityEngine;

public class NPCBumpDetection : MonoBehaviour
{
    private TicketControllerAI controller; 
    private void Awake()
    {
        controller = GetComponentInParent<TicketControllerAI>(); 
        Debug.Log($"NPCBumpDetection on {name}: controller = {(controller ? controller.name : "NULL")}");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"NPCBumpDetection on {name}: OnTriggerEnter2D with {other.name}, layer={LayerMask.LayerToName(other.gameObject.layer)}");
        // Check if it's a pushable NPC
        if (other.TryGetComponent<Pushable>(out Pushable npc))
        {
            // Only trigger shock if the NPC is currently being pushed / sliding
            if (npc.isMoving)
            {
                Debug.Log($"{controller.name} hit by moving NPC → shocked!");

                controller.StartShockedExternal();  // custom public method
            }
        }
    }


}
