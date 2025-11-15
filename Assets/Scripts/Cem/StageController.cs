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

public class StageController : MonoBehaviour
{
    [SerializeField]private StageFeature features;


    public void InitializeStage()
    {

    }

}
