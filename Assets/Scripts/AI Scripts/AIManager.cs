using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Central brain for all ticket controllers.
/// - Tracks patrollers and chasers in the current scene.
/// - Remembers how many chasers exist in total (for future scenes).
/// - Spawns controllers when entering a wagon or station.
/// </summary>
/// 
/// 
/// 
public class AIManager : MonoBehaviour
{

    public static AIManager Instance { get; private set; }

    [Header("Global Settings")]
    [Tooltip("How many patrolling controllers to spawn in a wagon by default.")]
    public int basePatrollersPerWagon = 2;

    [Tooltip("How many chasers exist at the very start (the ones who know your face).")]
    public int startingChasers = 1;


    [Header("Prefabs")]
    [Tooltip("Controller prefab. Its initialState will be set before Start() runs.")]
    public TicketControllerAI controllerPrefab;


    [Header("Debug Info (read-only)")]
    [SerializeField] private int currentStationIndex = 1;
    [SerializeField] private int totalChaserCount;    // chasers accumulated so far




    [Header("Current Wagon / NPCs")]
    [SerializeField] private NPCManager currentNPCManager;



    private readonly List<TicketControllerAI> activePatrollers = new();
    private readonly List<TicketControllerAI> activeChasers = new();



    public int CurrentStationIndex => currentStationIndex;
    /// <summary>Total number of chasers that must exist in any new scene.</summary>
    public int TotalChaserCount => Mathf.Max(totalChaserCount, startingChasers);

    public NPCManager CurrentNPCManager => currentNPCManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
private void Awake()
{
    //Singleton pattern 

        // if (Instance != null && Instance != this)
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        //
        Instance = this;
        // DontDestroyOnLoad(gameObject);

        totalChaserCount = startingChasers;
}



    // REGISTRATION

    /// <summary>
    /// For controllers that are already placed in the scene (optional).
    /// It puts them into the correct list based on their initialState.
    /// </summary>
    public void RegisterController(TicketControllerAI ctrl)
    {
        if (ctrl == null) return;

        if (ctrl.initialState == TicketControllerAI.ControllerState.Chasing)
        {
            RegisterChaser(ctrl, false);
        }
        else
        {
            RegisterPatroller(ctrl);
        }
    }

    public void RegisterPatroller(TicketControllerAI ctrl)
    {
        if (ctrl == null) return;
        if (!activePatrollers.Contains(ctrl))
            activePatrollers.Add(ctrl);
    }



    /// <param name="countForPersistence">
    /// true if this is a new chaser that should increase totalChaserCount (e.g. patroller promoted),
    /// false if it’s just a respawn of an already counted chaser.
    /// </param>
    public void RegisterChaser(TicketControllerAI ctrl, bool countForPersistence)
    {
        if (ctrl == null) return;

        if (!activeChasers.Contains(ctrl))
            activeChasers.Add(ctrl);

        if (countForPersistence)
            totalChaserCount++;
    }
    
    public void UnregisterController(TicketControllerAI ctrl)
    {
        if (ctrl == null) return;
        activePatrollers.Remove(ctrl);
        activeChasers.Remove(ctrl);
    }

    // PROMOTION

    /// <summary>
    /// Called when a patrolling controller detects/catches the player
    /// and becomes a chaser from now on.
    /// </summary>
    public void PromotePatrollerToChaser(TicketControllerAI ctrl)
    {
        if (ctrl == null) return;

        // Already a chaser? Then nothing to do.
        if (activeChasers.Contains(ctrl))
            return;

        // Move from patroller list to chaser list.
        activePatrollers.Remove(ctrl);
        activeChasers.Add(ctrl);

        // This chaser will persist for all future scenes.
        totalChaserCount++;

        // Switch its state in the AI.
        ctrl.SwitchToChasing();
    }

  


    // SPAWNING / SCENE FLOW

    /// <summary>
    /// Destroys all currently active controllers in this scene.
    /// Call this before spawning for a new wagon/station.
    /// </summary>

    public void ClearCurrentControllers()
    {
        Debug.Log($"Clearing {activePatrollers.Count} patrollers");
        for (int i = activePatrollers.Count - 1; i >= 0; i--)
        {
            if (activePatrollers[i] != null)
                Destroy(activePatrollers[i].gameObject);
        }
        activePatrollers.Clear();

        Debug.Log($"Clearing {activeChasers.Count} chasers");
        for (int i = activeChasers.Count - 1; i >= 0; i--)
        {
            if (activeChasers[i] != null)
                Destroy(activeChasers[i].gameObject);
        }
        activeChasers.Clear();
    }


    /// <summary>
    /// Call this when a station scene is entered.
    /// It increases the station index, clears old controllers
    /// and spawns the accumulated chasers on the platform.
    /// </summary>
    public void OnEnterStation(Transform[] chaserSpawnPoints)
    {
        currentStationIndex++;
        ClearCurrentControllers();
        SpawnChasersAtPoints(chaserSpawnPoints);
        // No patrollers on the station (according to your design).

        // En la estación, de momento no hay NPCs (o habrá otro manager específico)
        currentNPCManager = null;
    }

    /// <summary>
    /// Call this when a wagon scene is entered.
    /// It clears old controllers and spawns both chasers and fresh patrollers.
    /// </summary>
public void OnEnterWagon(Transform[] chaserSpawnPoints,
                         Transform[] patrollerSpawnPoints,
                         NPCManager npcManager)
{
    ClearCurrentControllers();

    // IMPORTANT: set the current NPC manager for this wagon
    currentNPCManager = npcManager;

    // Spawn chasers and patrollers, and give them the NPC manager
    SpawnChasersAtPoints(chaserSpawnPoints);
    SpawnPatrollersAtPoints(patrollerSpawnPoints);
}
    private void SpawnChasersAtPoints(Transform[] spawnPoints)
    {
        if (controllerPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("AIManager.SpawnChasersAtPoints: missing prefab or spawn points.");
            return;
        }

        int chasersToSpawn = TotalChaserCount;

        for (int i = 0; i < chasersToSpawn; i++)
        {
            Transform spawn = spawnPoints[i % spawnPoints.Length];

            TicketControllerAI ctrl =
                Instantiate(controllerPrefab, spawn.position, Quaternion.identity);

            ctrl.initialState = TicketControllerAI.ControllerState.Chasing;

            ctrl.SetNPCManager(currentNPCManager);  // puede ser null en la estación


            // Start() in TicketControllerAI will see initialState and start chasing.
            RegisterChaser(ctrl, false); // already counted in totalChaserCount
        }
    }

    private void SpawnPatrollersAtPoints(Transform[] spawnPoints)
    {
        if (controllerPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        int toSpawn = Mathf.Max(0, basePatrollersPerWagon);

        for (int i = 0; i < toSpawn; i++)
        {
            Transform spawn = spawnPoints[i % spawnPoints.Length];

            TicketControllerAI ctrl =
                Instantiate(controllerPrefab, spawn.position, Quaternion.identity);

            ctrl.initialState = TicketControllerAI.ControllerState.Patrolling;
            // Start() in TicketControllerAI will start patrolling by default.

            ctrl.SetNPCManager(currentNPCManager);


            RegisterPatroller(ctrl);
        }
    }

    
}
