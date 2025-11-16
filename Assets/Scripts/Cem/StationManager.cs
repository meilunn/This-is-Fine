using System;
using System.Collections;
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
    [SerializeField] private Collider2D stationConfiner;
    [SerializeField] private GameObject platformBottomCollider;

    [SerializeField] private GameObject tenSecondWarning;

    [SerializeField] private Transform stationPlayerSpawnPoint;


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
        player.ExitedTrain  += OnPlayerExitWagon;  


        cinemachine.Follow = player.transform;

        confiner.BoundingShape2D = stationConfiner;


        tenSecondWarning.SetActive(false);

        PrepareNextWagon();
    }

    private void Start()
    {
        confiner.BoundingShape2D = stationConfiner;
    }


    private void Update()
    {
        ProcessFSM();
    }



public void OnTimerEnds()
{
    Debug.Log("[StationManager] OnTimerEnds fired");

    if (currentWagon != null)
    {
        var exit = currentWagon.GetExitTrigger();
        if (exit != null)
        {
            Debug.Log($"[StationManager] Activating exit on wagon {currentWagon.name}");
            exit.Activate();
        }
        else
        {
            Debug.LogWarning("[StationManager] currentWagon has NO exitTrigger assigned!");
        }
    }
    else
    {
        Debug.LogWarning("[StationManager] OnTimerEnds but currentWagon is null!");
    }
}
    public void OnPlayerEntersWagon()
    {

        if (GameManager.Instance.CurrentGameStage == GameState.EndGame)
        {
            return;
        }

        // Position player inside the current wagon
        Transform spawn = currentWagon.GetPlayerSpawnPoint();
        if (spawn != null)
        {
            player.transform.position = spawn.position;

            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
        else
        {
            Debug.LogWarning($"StationManager: Wagon {currentWagon.name} has no player spawn point assigned!");
        }


        platformBottomCollider.SetActive(false);
        timer.SetTimer(currentWagon.features.DurationSec);
        timer.StartTimer();

        currentWagon.gameObject.SetActive(true);
        currentWagon.RebuildNavMesh();
        station.SetActive(false);


         // Switch camera confiner to wagon bounds
        Collider2D wagonConfiner = currentWagon.GetConfinerObj();
        if (wagonConfiner != null)
        {
            confiner.BoundingShape2D = wagonConfiner;

        }
        else
        {
            Debug.LogWarning($"Wagon {currentWagon.name} has no confiner collider assigned.");
        }
        trainAnimController.SetInitialEverything();
        confiner.enabled = true;
        stationCurrentStage = StationStage.InsideTrain;


        // Make sure the exit is closed at the start
        var exit = currentWagon.GetExitTrigger();
        if (exit != null) exit.Deactivate();
        
        FadeInOutScript.Instance.startFadeOut();
    }

    public void OnPlayerExitWagon()
{
    // 1) move player to station spawn
    if (stationPlayerSpawnPoint != null)
    {
        player.transform.position = stationPlayerSpawnPoint.position;

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    // 2) hide current wagon, show station
    currentWagon.gameObject.SetActive(false);
    station.SetActive(true);
    platformBottomCollider.SetActive(true);

    // 3) confine camera to station again
    confiner.BoundingShape2D = stationConfiner;

    // 4) close wagon exit (we still have reference to old currentWagon)
    var exit = currentWagon.GetExitTrigger();
    if (exit != null) exit.Deactivate();

    // 5) prepare next wagon for the *next* time the player boards
    PrepareNextWagon();
    InitializeNextStage();

    // 6) back to “waiting for train”
    stationCurrentStage = StationStage.WaitingForTrain;
    
    FadeInOutScript.Instance.startFadeOut();
}

    private void PrepareNextWagon()
    {
        OnStageCompleted?.Invoke();
if (currentStageIndex >= scenes.Count)
    {
        // We finished all wagons
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteGame();
        }
        else
        {
            Debug.Log("[StationManager] All wagons done, but no GameManager.Instance. Just staying at station.");
        }
        return;
    }
         if(currentStageIndex > 0)
            {
                //SceneController.Instance.RemoveSceneOnAdditive(scenes[currentStageIndex - 1].GetSceneName());

            }
            //SceneController.Instance.LoadeSceneAdditive(scenes[currentStageIndex].GetSceneName());
            currentWagon = scenes[currentStageIndex];

            //currentWagon = stages[currentStageIndex];
            currentStageIndex++;

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
                StartCoroutine(ShowMessageTemporarily(tenSecondWarning));
                stationCurrentStage = StationStage.TrainStopping;
                break;
            case StationStage.TrainStopping:
                //todo train stopping
                if (timer.IsTimerEnd())
                {
                    stationCurrentStage = StationStage.GameOver;
                }
                break;
            case StationStage.TrainStopped:

                //Todo player is out ---> success nextWagon
                //todo player is in ----> failed gameover

                //stationCurrentStage = StationStage.PrepareNextWagon;

                break;
            case StationStage.PrepareNextWagon:
                //PrepareNextWagon();
                //InitializeNextStage();

                //stationCurrentStage = StationStage.WaitingForTrain;
                break;
            case StationStage.GameOver:
                break;
        }
    }

    public void InitializeNextStage()
    {
        //confiner.BoundingShape2D = currentWagon.GetConfinerObj();
    }

    public PlayerMovement GetPlayer()
    {
        return player;
    }

    private IEnumerator ShowMessageTemporarily(GameObject obj)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        obj.SetActive(false);
    }

}
