using UnityEngine;

public class Timer : MonoBehaviour
{
    private bool isCounting = false;
    private float totalTime = -1f;
    private float warningStartTreshold = 10f;

    public System.Action OnTimerEnd;
    private void Update()
    {
        if (isCounting)
        {
            totalTime -= Time.deltaTime;

            if (totalTime < 0)
            {
                CompleteTimer();
            }
        }
    }

    public float GetTotalTime()
    {
        return totalTime;
    }
    
    public void SetTimer(float time)
    {
        totalTime = time;
    }

    public void StartTimer()
    {
        isCounting = true;
    }

    public void CompleteTimer()
    {
        OnTimerEnd?.Invoke();
        isCounting = false;
        totalTime = -1;

    }

    public bool IsTimerEnd()
    {
        return totalTime == -1;
    }


    public bool IsItWarningTime()
    {
        return (totalTime - warningStartTreshold) <= 0;
    }
}
