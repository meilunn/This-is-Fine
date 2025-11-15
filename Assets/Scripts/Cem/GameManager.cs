using UnityEngine;

public enum GameState
{
    NotStarted = 0,
    Started = 1,
    Playing = 2,
    Pause = 3,
    EndGame = 4,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    private GameState currentGameState = GameState.NotStarted;

    private int score;

    public GameState CurrentGameStage { get => currentGameState; set => currentGameState = value; }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    
    public void CompleteGame()
    {
        PanelManager.Instance.Show("gamecomplete");
    }

    public void GameOver()
    {
        PanelManager.Instance.Show("gameover");

    }

    
}
