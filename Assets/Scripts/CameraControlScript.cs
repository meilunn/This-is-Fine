using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraControlScript : MonoBehaviour
{
    [Header("Cinemachine Control")]
    [SerializeField] private GameObject cinemachineCameraObject; // The GameObject with CinemachineCamera
    
    [Header("Pause Camera Shift Settings")]
    [SerializeField] private float shiftAmount = 8f; // How far to shift
    [SerializeField] private float shiftDuration = 0.3f; // Duration of shift animation
    [SerializeField] private AnimationCurve shiftCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Boundary Settings")]
    [SerializeField] private bool respectBoundaries = true; // Check camera boundaries
    [SerializeField] private float boundaryPadding = 0.5f; // Safety margin from boundaries
    
    private Camera mainCamera;
    private CinemachineConfiner2D confiner;
    private Vector3 originalCameraPosition;
    private bool isShiftedLeft = false;
    private Coroutine currentShiftCoroutine;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
        
        if (cinemachineCameraObject == null)
        {
            Debug.LogError("Cinemachine Camera Object not assigned!");
        }
        else
        {
            // Try to get the Confiner component
            confiner = cinemachineCameraObject.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                Debug.Log("CinemachineConfiner2D found - will respect boundaries");
            }
            else
            {
                Debug.Log("No CinemachineConfiner2D found - no boundary checks");
            }
        }
    }
    
    private float GetMaxShiftAmount(Vector3 currentPosition, float desiredShift)
    {
        if (!respectBoundaries || confiner == null || confiner.BoundingShape2D == null)
        {
            return desiredShift; // No boundaries to check
        }

        // Get camera dimensions
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Calculate the target position
        Vector3 targetPosition = currentPosition + new Vector3(-desiredShift, 0, 0);
        
        // Get the bounding shape (Collider2D)
        Collider2D bounds = confiner.BoundingShape2D;
        
        // Check BOTH the left edge of camera at target AND right edge at current position
        float cameraLeftEdgeAfterShift = targetPosition.x - cameraWidth;
        float cameraRightEdgeCurrent = currentPosition.x + cameraWidth;
        
        // Check if left edge would hit boundary
        Vector2 leftCheckPoint = new Vector2(cameraLeftEdgeAfterShift, targetPosition.y);
        Vector2 closestPointLeft = bounds.ClosestPoint(leftCheckPoint);
        
        // Check if right edge is already at or past boundary
        Vector2 rightCheckPoint = new Vector2(cameraRightEdgeCurrent, currentPosition.y);
        Vector2 closestPointRight = bounds.ClosestPoint(rightCheckPoint);
        
        // If right edge is hitting boundary (closest point is to the left of our edge), DON'T SHIFT
        if (closestPointRight.x < cameraRightEdgeCurrent - 0.1f)
        {
            Debug.Log("Right boundary detected - NOT shifting camera!");
            return 0f; // Don't shift at all
        }
        
        // Check if left edge would hit boundary after shift
        float boundaryLeft = closestPointLeft.x;
        float maxAllowedLeft = boundaryLeft + boundaryPadding;
        
        // Calculate maximum shift that keeps us within bounds
        float maxShift = currentPosition.x - cameraWidth - maxAllowedLeft;
        
        if (maxShift < desiredShift)
        {
            Debug.Log($"Left boundary would be hit! Reducing shift from {desiredShift} to {maxShift}");
            return Mathf.Max(0, maxShift); // Don't allow negative shift
        }
        
        return desiredShift;
    }

    public void ShiftLeft25()
    {
        Debug.Log("ShiftLeft25() called");
        
        if (isShiftedLeft)
        {
            Debug.LogWarning("Already shifted!");
            return;
        }

        // Stop any existing shift
        if (currentShiftCoroutine != null)
        {
            StopCoroutine(currentShiftCoroutine);
        }

        currentShiftCoroutine = StartCoroutine(ShiftCameraCoroutine(true));
    }

    public void Restore()
    {
        Debug.Log("Restore() called");
        
        if (!isShiftedLeft)
        {
            Debug.LogWarning("Not shifted!");
            return;
        }

        // Stop any existing shift
        if (currentShiftCoroutine != null)
        {
            StopCoroutine(currentShiftCoroutine);
        }

        currentShiftCoroutine = StartCoroutine(ShiftCameraCoroutine(false));
    }

    private IEnumerator ShiftCameraCoroutine(bool shiftLeft)
    {
        if (shiftLeft)
        {
            // DISABLE Cinemachine
            if (cinemachineCameraObject != null)
            {
                cinemachineCameraObject.SetActive(false);
                Debug.Log("Cinemachine disabled");
            }

            // Save current camera position
            originalCameraPosition = mainCamera.transform.position;
            
            // Calculate actual shift amount (respecting boundaries)
            float actualShift = GetMaxShiftAmount(originalCameraPosition, shiftAmount);
            
            // Calculate target position (shift LEFT = negative X)
            Vector3 targetPosition = originalCameraPosition + new Vector3(-actualShift, 0, 0);
            
            float elapsed = 0f;
            Debug.Log($"Shifting camera from {originalCameraPosition} to {targetPosition} (shift: {actualShift})");

            // Animate the shift
            while (elapsed < shiftDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / shiftDuration);
                float curveValue = shiftCurve.Evaluate(t);

                mainCamera.transform.position = Vector3.Lerp(originalCameraPosition, targetPosition, curveValue);
                yield return null;
            }

            mainCamera.transform.position = targetPosition;
            isShiftedLeft = true;
            Debug.Log("Shift complete");
        }
        else
        {
            // Restore to original position
            Vector3 startPosition = mainCamera.transform.position;
            
            float elapsed = 0f;
            Debug.Log($"Restoring camera from {startPosition} to {originalCameraPosition}");

            // Animate the restore
            while (elapsed < shiftDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / shiftDuration);
                float curveValue = shiftCurve.Evaluate(t);

                mainCamera.transform.position = Vector3.Lerp(startPosition, originalCameraPosition, curveValue);
                yield return null;
            }

            mainCamera.transform.position = originalCameraPosition;
            
            // RE-ENABLE Cinemachine
            if (cinemachineCameraObject != null)
            {
                cinemachineCameraObject.SetActive(true);
                Debug.Log("Cinemachine re-enabled");
            }
            
            isShiftedLeft = false;
            Debug.Log("Restore complete");
        }

        currentShiftCoroutine = null;
    }

    // Instant versions (no animation)
    public void ShiftLeft25Instant()
    {
        if (isShiftedLeft) return;

        // Disable Cinemachine
        if (cinemachineCameraObject != null)
        {
            cinemachineCameraObject.SetActive(false);
        }

        // Save and shift
        originalCameraPosition = mainCamera.transform.position;
        
        // Calculate actual shift amount (respecting boundaries)
        float actualShift = GetMaxShiftAmount(originalCameraPosition, shiftAmount);
        
        mainCamera.transform.position += new Vector3(-actualShift, 0, 0);
        isShiftedLeft = true;
        
        Debug.Log($"Instant shift complete (shift: {actualShift})");
    }

    public void RestoreInstant()
    {
        if (!isShiftedLeft) return;

        // Restore position
        mainCamera.transform.position = originalCameraPosition;
        
        // Re-enable Cinemachine
        if (cinemachineCameraObject != null)
        {
            cinemachineCameraObject.SetActive(true);
        }
        
        isShiftedLeft = false;
        
        Debug.Log("Instant restore complete");
    }
}