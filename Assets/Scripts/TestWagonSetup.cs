using UnityEngine;

public class TestWagonSetup : MonoBehaviour
{
    [Header("Managers")]
    public NPCManager npcManager;

    [Header("Spawn Points")]
    public NPCManager.SpawnPoint[] npcSpawnPoints;
    public Transform[] chaserSpawnPoints;
    public Transform[] patrollerSpawnPoints;

    [Header("Player")]
    public Transform playerTransform;   // you can assign in inspector



    private void Awake()
    {
        // Autodetect player if not set
        if (playerTransform == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
        }
    }
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

    private void AssignPlayerToControllers()
    {
        if (playerTransform == null) return;

        var controllers = FindObjectsOfType<TicketControllerAI>();
        foreach (var ctrl in controllers)
        {
            ctrl.player = playerTransform;
        }
    }
    
}
