using UnityEngine;

public class WagonExitTrigger : MonoBehaviour
{
    private bool isActive = false;
    public bool PlayerIsHere { get; private set; }

    [SerializeField] private GameObject pressEHintExit;   // optional separater Hint

    public void Activate()
    {
        isActive = true;
        Debug.Log($"[WagonExitTrigger] Activated on {name}");
    }

    public void Deactivate()
    {
        isActive = false;
        PlayerIsHere = false;

        if (pressEHintExit != null)
            pressEHintExit.SetActive(false);

        Debug.Log($"[WagonExitTrigger] Deactivated on {name}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;
        if (!isActive) return; // Tür noch zu

        PlayerIsHere = true;

        if (pressEHintExit != null)
            pressEHintExit.SetActive(true);

        Debug.Log("[WagonExitTrigger] Player is at exit");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        PlayerIsHere = false;

        if (pressEHintExit != null)
            pressEHintExit.SetActive(false);
    }
}