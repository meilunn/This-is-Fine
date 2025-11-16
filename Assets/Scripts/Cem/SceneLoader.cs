using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneName targetScene;

    public void LoadScene()
    {
        FadeInOutScript.Instance.startFadeOut();
        SceneController.Instance.LoadSceneByName(targetScene);
    }


}
