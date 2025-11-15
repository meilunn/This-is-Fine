using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public enum StationStage
{
    WaitingForTrain,
    TrainComing,
    OnTrainCome,
    InsideTrain,
    TrainStopping,
    TrainStopped
}


public class StationManager : MonoBehaviour
{

    public static StationManager Instance;
    [SerializeField] private List<WagonController> stages;
    [SerializeField] private Timer timer;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private CinemachineCamera cinemachine;


    private WagonController currentStage;
    private int currentStageIndex = 0;



    public System.Action OnStageCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        if (stages.Count > 0) currentStage = stages[0];
        timer.OnTimerEnd += CompleteCurrentStage;
    }



    public void CompleteCurrentStage()
    {
        OnStageCompleted?.Invoke();
        currentStageIndex++;
        if(currentStageIndex>= stages.Count)
        {
            GameManager.Instance.CompleteGame();
        }
        else
        {

            currentStage = stages[currentStageIndex];
            InitializeNextStage();
        }
    }

    public void InitializeNextStage()
    {

    }

    public PlayerMovement GetPlayer()
    {
        return player;
    }



}
