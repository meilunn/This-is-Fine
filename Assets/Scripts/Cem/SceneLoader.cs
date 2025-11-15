using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneName targetScene;

    public void LoadScene()
    {
        SceneController.Instance.LoadSceneByName(targetScene);
    }


}
