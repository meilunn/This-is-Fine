using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    StartScene = 0,
    MainMenu = 1,
    GameScene = 2,
}
public class SceneController : MonoBehaviour
{

    public static SceneController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   // remove this line if you don't want it to persist between scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadSceneByName(SceneName.MainMenu);
        PanelManager.Instance.Show("mainmenu");
    }


    public void LoadSceneByName(SceneName sceneName)
    {
        SceneManager.LoadScene((int)sceneName);
    }
}
