using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public enum StationStage
{
    WaitingForTrain,
    TrainComing,
    OnTrainCome,
    InsideTrain,
    StartLast10SecWarning,
    TrainStopping,
    TrainStopped,
    PrepareNextWagon,
    GameOver,
    GameWin,
}


public class StationManager : MonoBehaviour
{

    public static StationManager Instance;
    [SerializeField] private List<WagonController> scenes;
    [SerializeField] private Timer timer;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private CinemachineCamera cinemachine;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private TrainAnimController trainAnimController;
    [SerializeField] private GameObject station;

    private StationStage stationCurrentStage;
    private WagonController currentWagon;
    private Vector3 cameraInitialPos;
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
        timer.OnTimerEnd += OnTimerEnds;
        player.EnteredTrain += OnPlayerEntersWagon;
        PrepareNextWagon();
    }


    private void Update()
    {
        ProcessFSM();
    }



    public void OnTimerEnds()
    {
        stationCurrentStage = StationStage.TrainStopped;
    }

    public void OnPlayerEntersWagon()
    {
        timer.SetTimer(currentWagon.features.DurationSec);
        timer.StartTimer();
        currentWagon.gameObject.SetActive(true);
        station.SetActive(false);
        trainAnimController.SetInitialEverything();
        confiner.enabled = true;
        stationCurrentStage = StationStage.InsideTrain;
    }

    public void OnPlayerExitWagon()
    {
        currentWagon.gameObject.SetActive(false);
        station.SetActive(true);
        stationCurrentStage = StationStage.TrainStopped;
    }

    private void PrepareNextWagon()
    {
        OnStageCompleted?.Invoke();
        if (currentStageIndex >= scenes.Count)
        {
            GameManager.Instance.CompleteGame();
        }
        else
        {
            if(currentStageIndex > 0)
            {
                SceneController.Instance.RemoveSceneOnAdditive(scenes[currentStageIndex - 1].GetSceneName());

            }
            SceneController.Instance.LoadeSceneAdditive(scenes[currentStageIndex].GetSceneName());
            currentWagon = scenes[currentStageIndex];

            //currentWagon = stages[currentStageIndex];
            currentStageIndex++;
        }
    }

    public void ProcessFSM()
    {
        switch (stationCurrentStage)
        {
            case StationStage.WaitingForTrain:

                if (!trainAnimController.GetIsMoving())
                {
                    trainAnimController.StartMoving();
                    stationCurrentStage = StationStage.TrainComing;
                }
                break;
            case StationStage.TrainComing:
                if (!trainAnimController.GetIsMoving())
                {
                    stationCurrentStage = StationStage.OnTrainCome;
                }
                break;
            case StationStage.OnTrainCome:


                break;
            case StationStage.InsideTrain:
                //todo normal play
                if (timer.IsItWarningTime())
                {
                    stationCurrentStage = StationStage.StartLast10SecWarning;
                }
                break;
            case StationStage.StartLast10SecWarning:
                //todo start last10SecWarning
                stationCurrentStage = StationStage.TrainStopping;
                break;
            case StationStage.TrainStopping:
                //todo train stopping
                if (timer.IsTimerEnd())
                {
                    stationCurrentStage = StationStage.GameOver;
                }
                stationCurrentStage = StationStage.TrainStopping;
                break;
            case StationStage.TrainStopped:

                //Todo player is out ---> success nextWagon
                //todo player is in ----> failed gameover

                stationCurrentStage = StationStage.PrepareNextWagon;

                break;
            case StationStage.PrepareNextWagon:
                PrepareNextWagon();
                InitializeNextStage();

                break;
            case StationStage.GameOver:
                break;
        }
    }

    public void InitializeNextStage()
    {
        confiner.BoundingShape2D = currentWagon.GetConfinerObj();
    }

    public PlayerMovement GetPlayer()
    {
        return player;
    }



}
