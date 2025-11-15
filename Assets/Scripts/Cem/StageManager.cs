using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{

    public static StageManager Instance;
    [SerializeField] private List<StageController> stages;
    [SerializeField] private Timer timer;


    private StageController currentStage;
    private int currentStageIndex = 0;


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
        currentStage = stages[0];
    }


    public System.Action OnStageCompleted;

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



}
