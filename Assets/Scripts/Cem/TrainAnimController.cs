using UnityEngine;

public class TrainAnimController : MonoBehaviour
{
    [SerializeField] private Transform trainTarget;


    private bool isMoving = false;
    private float moveDuration = 7f;
    private float timer = 0f;
    private Vector3 initialPos;

    private void Awake()
    {
        initialPos = transform.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPos, trainTarget.position, timer / moveDuration);
            if (timer >= moveDuration)
            {
                isMoving = false;
            }
        }
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void SetInitialEverything()
    {
        isMoving = false;
        timer = 0f;
        transform.position = initialPos;
    }
    public bool GetIsMoving()
    {
        return isMoving;
    }

}
