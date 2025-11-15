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
        animator.SetFloat("MoveX", move.x);
        animator.SetFloat("MoveY", move.x);
        controller.Move(move * speed * Time.deltaTime);
    }
}
