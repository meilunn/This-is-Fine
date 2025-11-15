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
        interactionMessage.SetActive(false);
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
        interactionMessage.transform.rotation = Quaternion.Euler(Vector3.zero);
        interactionMessage.SetActive(b);
    }

    public IEnumerator MoveToPosition(Vector2 target)
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
