using System;
using System.Collections;
using UnityEngine;

public class Pushable : MonoBehaviour, IInteractable
{
    [SerializeField] private float moveDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private GameObject interactionMessage;
    [SerializeField] private SpriteRenderer arrowRenderer;


    [Tooltip("Extra time after reaching the target where the NPC still stuns controllers")]
    [SerializeField] private float dangerousExtraTime = 0.15f;


    public bool isMoving;

    private Animator animator;
    private Animator npcAnimator;
    private bool isNPC;
    public int ObjID { get; set; }

    private Vector2 moveDirection;

    void Start()
    {
        if (ObjID == 0) ObjID = GlobalHelper.GenerateUniqueID(gameObject);
        interactionMessage.SetActive(false);
        arrowRenderer.gameObject.SetActive(false);
        //SetMoveDirection();
        GameObject player = GameObject.FindWithTag("Player");
        animator = player.GetComponent<Animator>();
        npcAnimator = GetComponent<Animator>();
        if (npcAnimator) isNPC = true;
    }

    private void Update()
    {
        if (arrowRenderer.gameObject.activeSelf)
        {
            Vector3 dir = (transform.position - StationManager.Instance.GetPlayer().transform.position).normalized;
            Vector2 dir2 = new Vector2(dir.x, dir.y);
            float angle = Vector2.Angle(Vector2.up, dir2);
            if (dir.x > 0)
            {
                angle = Vector2.Angle(Vector2.down, dir2) + 180f;
            }
            
            arrowRenderer.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            arrowRenderer.gameObject.transform.position = transform.position + dir;
        }
    }

    public void Interact()
    {
        Vector3 dir = (transform.position - StationManager.Instance.GetPlayer().transform.position).normalized;
        Vector2 dir2 = new Vector2(dir.x, dir.y);
        dir.z = 0;
        animator.SetBool("isPushing", true);
        
        //if (isNPC)
        //{
        //    npcAnimator.SetBool("isPushed", true);
        //    npcAnimator.SetFloat("MoveX", dir2.x);
        //    npcAnimator.SetFloat("MoveY", dir2.y);
        //}
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir2, moveDistance, obstacleLayer);
        var moveTarget = new Vector2(0, 0);
        if (hit.collider != null)
        {
            moveTarget = hit.point;
            moveTarget -= dir2 *.5f;
        }
        else
        {
            moveTarget = new Vector2(transform.position.x, transform.position.y) + dir2 * moveDistance;
        }
        if (!isMoving)
            StartCoroutine(
                MoveToPosition(moveTarget)
                );
    }

    public void SetInteractionMessage(bool b)
    {
        interactionMessage.transform.rotation = Quaternion.Euler(Vector3.zero);
        interactionMessage.SetActive(b);
        
        arrowRenderer.gameObject.SetActive(b);
    }

    private IEnumerator MoveToPosition(Vector2 target)
    {
        SoundManager.PlaySoundAfter(SoundType.PushHit, SoundType.Grunt);
        isMoving = true;
        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y);
        float elapsedTime = 0f;
        float duration = Vector2.Distance(startPosition, target) / moveSpeed;

        while (elapsedTime < duration)
        {
            Vector2 lerp = Vector2.Lerp(startPosition, target, elapsedTime / duration);
            transform.position = new Vector3(lerp.x, lerp.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(target.x, target.y, 0f); // Ensure it ends exactly at target

        float extra = dangerousExtraTime;
        while (extra > 0f)
        {
            extra -= Time.deltaTime;
            yield return null;
        }

        isMoving = false;
        
        animator.SetBool("isPushing", false);
        
        //if (isNPC)
        //{
        //    npcAnimator.SetBool("isPushed", false);
        //    npcAnimator.SetFloat("MoveX", 0f);
        //    npcAnimator.SetFloat("MoveY", 0f);
        //}
        Debug.Log($"Object {ObjID} moved from {startPosition} to {transform.position}");
    }
}
