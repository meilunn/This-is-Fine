using UnityEngine;
using System.Collections;

public class UISlideIn : MonoBehaviour
{
    [SerializeField] private Vector2 startOffset; // Offset from final position
    [SerializeField] private float slideSpeed;

    private RectTransform rectTransform;
    private Vector2 targetPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        targetPosition = rectTransform.anchoredPosition;

        // Set starting position (offset from target)
        rectTransform.anchoredPosition = targetPosition + startOffset;

        StartCoroutine(SlideToPosition());
    }

    private IEnumerator SlideToPosition()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsedTime = 0f;
        float duration = Vector2.Distance(startPosition, targetPosition) / (slideSpeed * 100f);

        while (elapsedTime < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }
}
