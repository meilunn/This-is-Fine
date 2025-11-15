using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AnimationTestScript : MonoBehaviour
{
    [SerializeField] private InputActionReference moveControl;
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        moveControl.action.Enable();
    }

    private void OnDisable()
    {
        moveControl.action.Disable();
    }

    private void Update()
    {
        Vector2 move = moveControl.action.ReadValue<Vector2>();
        float moveX = move.x;
        float moveY = move.y;
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);
        
        //flipping if sprite only faces to the right
        if (moveX < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveX > 0)
        {
            spriteRenderer.flipX = false;
        }
        controller.Move(move * speed * Time.deltaTime);
    }
}
