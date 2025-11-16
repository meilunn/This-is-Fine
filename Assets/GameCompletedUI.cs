using UnityEngine;
using UnityEngine.SceneManagement;
public class GameCompletedUI : MonoBehaviour
{
public void OnRetryButton()
    {
        // reset state so StationManager will allow entering wagon again
        SoundManager.StopLoop();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentGameStage = GameState.NotStarted;
        }

        // reload the main station scene – name must match your scene
        SceneManager.LoadScene("StationScene");
    }
}
