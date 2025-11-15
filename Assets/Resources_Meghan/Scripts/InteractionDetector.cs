using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable closestInteractable;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(!Pause_CenterOnPlayer.isPaused && context.performed)
        {
            closestInteractable?.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out IInteractable interactable))
        {
            closestInteractable = interactable;
            closestInteractable.SetInteractionMessage(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.TryGetComponent(out IInteractable interactable)
           && interactable == closestInteractable)
        {
            closestInteractable.SetInteractionMessage(false);
            closestInteractable = null;
        }
    }
}
