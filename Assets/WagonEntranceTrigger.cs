using UnityEngine;

public class WagonEntranceTrigger : MonoBehaviour
{
    public bool PlayerIsHere { get; private set; } = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            PlayerIsHere = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            PlayerIsHere = false;
    }
}
