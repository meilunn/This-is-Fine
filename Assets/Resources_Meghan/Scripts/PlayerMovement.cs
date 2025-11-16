using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private GameObject interactionDetector;

    private Vector2 moveInput;


    public System.Action EnteredTrain;
    public System.Action ExitedTrain;

    //comment as long as we dont have animation sprites
    //[Header("Animator Variables")]
    //[SerializeField] private Animator animator;
    //[SerializeField] private SpriteRenderer spriteRenderer;
    [Header("Animator Variables")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private WagonEntranceTrigger wagonEntranceTrigger;
    [SerializeField] private WagonExitTrigger wagonExitTrigger;
    [SerializeField] private GameObject pressEHintEntrance;
    [SerializeField] private GameObject pressEHintExit;


    // NEW: can we currently enter a wagon?
    private bool canEnterWagon = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pressEHintEntrance != null) pressEHintEntrance.SetActive(false);
        if (pressEHintExit != null)      pressEHintExit.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        // Are we standing in the entrance trigger?
        bool nearEntrance = wagonEntranceTrigger != null && wagonEntranceTrigger.PlayerIsHere;
        bool nearExit     = wagonExitTrigger     != null && wagonExitTrigger.PlayerIsHere;

        if (pressEHintEntrance != null)
            pressEHintEntrance.SetActive(nearEntrance);

        if (pressEHintExit != null)
            pressEHintExit.SetActive(nearExit);

        // Press E to enter if near entrance
        if (nearEntrance &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            EnteredTrain?.Invoke();
        }

        // E drücken zum AUSSTEIGEN
        if (nearExit &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            ExitedTrain?.Invoke();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            interactionDetector.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        float moveX = moveInput.x;
        float moveY = moveInput.y;
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);

        ///flipping, cus we only have left facing sprites
        //if (Mathf.Abs(moveX) > Mathf.Abs(moveY) && Mathf.Abs(moveX) > 0.1f)
        //{
        //    spriteRenderer.flipX = moveX > 0;
        //}
        
        //up down flip
        if (moveY < 0)
            spriteRenderer.flipY = true;
        else 
            spriteRenderer.flipY = false;
    }


}
