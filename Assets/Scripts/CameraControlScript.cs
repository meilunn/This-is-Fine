using UnityEngine;

public class CameraControlScript : MonoBehaviour
{
    [Header("Cinemachine Control")]
    [SerializeField] private GameObject cinemachineCameraObject;
    
    [Header("Pause Camera Shift Settings")]
    [SerializeField] private float shiftDuration = 0.5f;
    [SerializeField] private AnimationCurve shiftCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isShifting = false;
    private float shiftProgress = 0f;
    private bool isShiftedLeft = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isShifting)
        {
            shiftProgress += Time.unscaledDeltaTime / shiftDuration;
            
            if (shiftProgress >= 1f)
            {
                shiftProgress = 1f;
                isShifting = false;
            }

            float curveValue = shiftCurve.Evaluate(shiftProgress);
            transform.position = Vector3.Lerp(originalPosition, targetPosition, curveValue);
        }
    }

    public void ShiftLeft25()
    {
        if (isShiftedLeft) return;

        // Disable Cinemachine camera
        if (cinemachineCameraObject != null)
        {
            cinemachineCameraObject.SetActive(false);
        }

        // Calculate shift amount (25% of screen width to the right)
        Camera cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        
        float screenWidth = cam.orthographicSize * cam.aspect;
        float shiftAmount = screenWidth * 0.25f;

        originalPosition = transform.position;
        targetPosition = originalPosition + new Vector3(-shiftAmount, 0, 0);

        isShifting = true;
        isShiftedLeft = true;
        shiftProgress = 0f;
    }

    public void Restore()
    {
        if (!isShiftedLeft) return;

        Camera cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        
        float screenWidth = cam.orthographicSize * cam.aspect;
        float shiftAmount = screenWidth * 0.25f;

        originalPosition = transform.position;
        targetPosition = originalPosition - new Vector3(shiftAmount, 0, 0);

        isShifting = true;
        isShiftedLeft = false;
        shiftProgress = 0f;
        
        // Re-enable Cinemachine camera after shift completes
        // We'll enable it immediately, but you could also enable it after the animation
        if (cinemachineCameraObject != null)
        {
            cinemachineCameraObject.SetActive(true);
        }
    }

    public void ShiftLeft25Instant()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        
        float screenWidth = cam.orthographicSize * cam.aspect;
        float shiftAmount = screenWidth * 0.25f;
        transform.position += new Vector3(shiftAmount, 0, 0);
        isShiftedLeft = true;
    }

    public void RestoreInstant()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        
        float screenWidth = cam.orthographicSize * cam.aspect;
        float shiftAmount = screenWidth * 0.25f;
        transform.position -= new Vector3(shiftAmount, 0, 0);
        isShiftedLeft = false;
    }
}