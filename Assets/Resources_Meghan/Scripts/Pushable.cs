using System.Collections;
using UnityEngine;

public class Pushable : MonoBehaviour, IInteractable
{
    [SerializeField] private float moveDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private GameObject interactionMessage;
    [SerializeField] private GameObject player;

    private bool isMoving;
    public int ObjID { get; set; }

    private Vector2 moveDirection;

    void Start()
    {
        if (ObjID == 0) ObjID = GlobalHelper.GenerateUniqueID(gameObject);
        //SetMoveDirection();
    }

    public void Interact()
    {
        Vector3 dir = (transform.position - StageManager.Instance.GetPlayer().transform.position).normalized;
        dir.z = 0;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, moveDistance, obstacleLayer);
        if (hit.collider != null) return;

        if (!isMoving)
            StartCoroutine(
                MoveToPosition(new Vector2(transform.position.x, transform.position.y) + new Vector2(dir.x,dir.y) * moveDistance)
                );

    }

    public void SetInteractionMessage(bool b)
    {
        interactionMessage.SetActive(b);
    }

    // check surroundings, sets move direction to free adjacent tile
    private void SetMoveDirection()
    {
        Vector2[] directions =
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        foreach (Vector2 dir in directions)
        {
            // Raycast to check if path is clear
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, moveDistance, obstacleLayer);

            if (hit.collider == null) // No obstacle found
            {
                moveDirection = dir;
                Debug.Log($"Move Direction for Object {ObjID}: {moveDirection}");
                return;
            }
        }

        moveDirection = Vector2.zero; // No free direction
    }

    private IEnumerator MoveToPosition(Vector2 target)
    {
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
        isMoving = false;
        Debug.Log($"Object {ObjID} moved from {startPosition} to {target}");
    }
}
