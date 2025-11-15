using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private Rigidbody2D rb;

    private Vector2 moveInput;

    [Header("Animator Variables")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        
        float moveX = moveInput.x;
        float moveY = moveInput.y;
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);
        
        ///flipping, cus we only have left facing sprites
        if (Mathf.Abs(moveX) > Mathf.Abs(moveY) && Mathf.Abs(moveX) > 0.1f)
        {
            spriteRenderer.flipX = moveX > 0;
        }
    }
}
