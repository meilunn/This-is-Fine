using UnityEngine;

public class NPCBumpDetection : MonoBehaviour
{
    private TicketControllerAI controller; 
    private void Awake()
    {
        controller = GetComponentInParent<TicketControllerAI>(); 
        Debug.Log($"NPCBumpDetection on {name}: controller = {(controller ? controller.name : "NULL")}");
    }
private void TryShock(Collider2D other)
    {
        // Check if it's a pushable NPC
        if (other.TryGetComponent<Pushable>(out Pushable npc))
        {
            if (npc.isMoving)
            {
                Debug.Log($"{controller.name} hit by moving NPC → shocked!");
                controller.StartShockedExternal();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"NPCBumpDetection on {name}: Enter with {other.name}");
        TryShock(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // extra robustness: if they overlap during several frames
        TryShock(other);
    }


}
