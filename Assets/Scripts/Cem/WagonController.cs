using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StageFeature
{
    public float DurationSec;
    public int NpcCount;
    public int PassangerCount;
    public List<Transform> spawnPoints;
}

public class WagonController : MonoBehaviour
{
    public StageFeature features;
    [SerializeField]private Collider2D confinerObj;
    [SerializeField]private SceneName sceneName;



    public void InitializeStage()
    {

    }

    public Collider2D GetConfinerObj()
    {
        return confinerObj;
    }

    public SceneName GetSceneName()
    {
        return sceneName;
    }

}
