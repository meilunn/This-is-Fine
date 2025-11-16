using UnityEngine;

public class WagonExitTrigger : MonoBehaviour
{
    private bool isActive = false;

    public void Activate()
    {
        isActive = true;
        Debug.Log($"[WagonExitTrigger] Activated on {name}");
    }

    public void Deactivate()
    {
        isActive = false;
        Debug.Log($"[WagonExitTrigger] Deactivated on {name}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[WagonExitTrigger] OnTriggerEnter2D with {other.name}, isActive={isActive}");

        if (!isActive) return;

        var player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        Debug.Log("[WagonExitTrigger] Player entered active exit, calling StationManager.OnPlayerExitWagon");

        if (StationManager.Instance != null)
        {
            StationManager.Instance.OnPlayerExitWagon();
        }
    }
}
