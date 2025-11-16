using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;

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
    [SerializeField]private NavMeshPlus.Components.NavMeshSurface surface;



    [Header("Player")]
    [SerializeField] private Transform playerSpawnPoint;
    void OnEnable()
    {
        if (surface == null) surface = GetComponentInChildren<NavMeshPlus.Components.NavMeshSurface>();
        surface.BuildNavMesh();   // runtime generation instead of baked data
    }


    public Collider2D GetConfinerObj()
    {
        return confinerObj;
    }

    public SceneName GetSceneName()
    {
        return sceneName;
    }

        public Transform GetPlayerSpawnPoint()
    {
        return playerSpawnPoint;
    }

}
