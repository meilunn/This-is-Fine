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
    // 1. If boundaries are off, allow the full shift
    if (!respectBoundaries || confiner == null || confiner.BoundingShape2D == null)
    {
        Debug.Log("No boundaries, allowing full shift.");
        return desiredShift; // No boundaries to check
    }

    // 2. Get the camera's half-width
    // (mainCamera.orthographicSize is HALF the height)
    float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

    // 3. Get the absolute world-space boundaries of the *entire* level
    Bounds levelBounds = confiner.BoundingShape2D.bounds;
    float boundaryLeft = levelBounds.min.x;
    float boundaryRight = levelBounds.max.x;

    // 4. Check if we're already at the RIGHT edge
    // (This prevents shifting left if the entire screen is already at the right boundary)
    float currentCameraRightEdge = currentPosition.x + cameraHalfWidth;
    if (currentCameraRightEdge >= boundaryRight - boundaryPadding)
    {
        Debug.Log("Camera is at or past the RIGHT boundary. Not shifting.");
        return 0f;
    }

    // 5. Check how much space we have on the LEFT
    float currentCameraLeftEdge = currentPosition.x - cameraHalfWidth;

    // Calculate the total space available between the camera's left edge and the boundary
    // e.g., CamLeftEdge (10) - (BoundaryLeft (0) + Padding (0.5)) = 9.5 available
    float availableSpaceOnLeft = currentCameraLeftEdge - (boundaryLeft + boundaryPadding);

    // If we have no space, return 0
    if (availableSpaceOnLeft <= 0.01f) // Use a small epsilon
    {
        Debug.Log("Camera is at or past the LEFT boundary. Not shifting.");
        return 0f;
    }

    // 6. Return the shift amount, clamped by the available space
    // e.g., We want to shift 8, but only have 3.5 space. Return 3.5.
    // e.g., We want to shift 8, and have 9.5 space. Return 8.
    float actualShift = Mathf.Min(desiredShift, availableSpaceOnLeft);
    
    Debug.Log($"Available space on left: {availableSpaceOnLeft:F2}, Desired shift: {desiredShift}, Actual shift: {actualShift:F2}");
    return actualShift;
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