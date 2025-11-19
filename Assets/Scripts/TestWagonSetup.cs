using System;
using UnityEngine;

public class TestWagonSetup : MonoBehaviour
{
    [Header("Managers")]
    public NPCManager npcManager;
    public AIManager aiManager;

    [Header("Spawn Points")]
    public NPCManager.SpawnPoint[] npcSpawnPoints;
    public Transform[] chaserSpawnPoints;
    public Transform[] patrollerSpawnPoints;

    

    
    private void OnEnable()
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
        if (aiManager != null)
        {
            aiManager.OnEnterWagon(
                chaserSpawnPoints,
                patrollerSpawnPoints,
                npcManager
            );
        }
        else
        {
            Debug.LogWarning("TestWagonSetup: aiManager is null.");
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
        if (aiManager != null)
        {
            aiManager.ClearCurrentControllers();
            Debug.LogWarning("CleanUpControllers");
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
