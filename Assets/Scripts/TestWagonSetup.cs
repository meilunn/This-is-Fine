using System;
using UnityEngine;

public class TestWagonSetup : MonoBehaviour
{
    [Header("Managers")]
    public NPCManager npcManager;

    [Header("Spawn Points")]
    public NPCManager.SpawnPoint[] npcSpawnPoints;
    public Transform[] chaserSpawnPoints;
    public Transform[] patrollerSpawnPoints;



    
    private void Start()
    {
        // 1) Initialize NPCs
        if (npcManager != null)
        {
            npcManager.IntializeNPCs(npcSpawnPoints);
        }
        else
        {
            Debug.LogWarning("TestWagonSetup: NPCManager is not assigned.");
        }

        // 2) Spawn controllers via AIManager
        if (AIManager.Instance != null)
        {
            AIManager.Instance.OnEnterWagon(
                chaserSpawnPoints,
                patrollerSpawnPoints,
                npcManager
            );
        }
        else
        {
            Debug.LogWarning("TestWagonSetup: AIManager.Instance is null.");
        }

        // 3) Assign player to all controllers
        AssignPlayerToControllers();
    }

    private void OnDisable()
    {
        if (npcManager != null)
        {
            npcManager.CleanUpNpcPassengers();
            Debug.Log("CleanUpNpcPassengers");
        }
        if (AIManager.Instance != null)
        {
            AIManager.Instance.ClearCurrentControllers();
        }
    }

    private void AssignPlayerToControllers()
    {

        var controllers = FindObjectsOfType<TicketControllerAI>();
        foreach (var ctrl in controllers)
        {
            ctrl.player = StationManager.Instance.GetPlayer().transform;
        }
    }
    
}
