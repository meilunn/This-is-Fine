using UnityEngine;

public class WagonExitTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
private bool isActive = false;

    public void Activate()  => isActive = true;
    public void Deactivate() => isActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        // Only react to the player
        var player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        // Tell StationManager the player left the wagon
        if (StationManager.Instance != null)
        {
            StationManager.Instance.OnPlayerExitWagon();
        }
    }
}
