using UnityEngine;

public interface IInteractable
{
    void Interact();
    bool CanInteract();

    void SetInteractionMessage(bool b);
}
