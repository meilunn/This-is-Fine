using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private Rigidbody2D rb;

    private Vector2 moveInput;


    public System.Action EnteredTrain;
    public System.Action ExitedTrain;

    //comment as long as we dont have animation sprites
    //[Header("Animator Variables")]
    //[SerializeField] private Animator animator;
    //[SerializeField] private SpriteRenderer spriteRenderer;

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

        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        
        //float moveX = moveInput.x;
        //float moveY = moveInput.y;
        //animator.SetFloat("MoveX", moveX);
        //animator.SetFloat("MoveY", moveY);
        
        ///flipping, cus we only have left facing sprites
        //if (moveX < 0)
        //{
        //    spriteRenderer.flipX = false;
        //}
        //else if (moveX > 0)
        //{
        //    spriteRenderer.flipX = true;
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WagonEntrance"))
        {
            EnteredTrain?.Invoke();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("WagonExit"))
        {
            ExitedTrain?.Invoke();
        }
    }
}
