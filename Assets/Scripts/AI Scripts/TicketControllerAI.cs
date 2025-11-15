using UnityEngine;
using UnityEngine.AI;

public class TicketControllerAI  : MonoBehaviour
{

        public enum ControllerState
    {
        Patrolling,
        CheckingTicket,
        Chasing
    }

     [Header("General")]
    public ControllerState initialState = ControllerState.Patrolling;
    public Transform player;                       // assign in inspector for now, later change to moving player
    public Transform[] patrolToNPC;                // NPC transforms 
    public float npcStoppingDistance = 0.15f;      // how close to get to an NPC
    public float checkTicketDuration = 1.5f;       // time spent "checking" a ticket
    public float catchDistance = 0.2f;             // distance to catch the player
    public float chaseSpeed = 4.5f;
    public float patrolSpeed = 3.0f;


    [Header("References")]
    [SerializeField] private NPCManager npcManager;   // se setea desde AIManager

    private NavMeshAgent agent;
    private ControllerState currentState;
    private int currentNPCIndex = 0;
    private Transform currentNPCTarget;
    private float checkTimer;



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = initialState;

        
        if (currentState == ControllerState.Chasing)
        {
            StartChasing();
        }

                else
        {
            StartPatrolling();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 100);
        switch (currentState)
        {
             case ControllerState.Patrolling:
                UpdatePatrolling();
                break;
            case ControllerState.CheckingTicket:
                UpdateCheckingTicket();
                break;
            case ControllerState.Chasing:
                UpdateChasing();
                break;
        }
    
    }

// patrolling 


    void StartPatrolling()
    {
       
        currentState = ControllerState.Patrolling;
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        ChooseNextNPCTarget();
    }

    void UpdatePatrolling()
    {
        
        //if npc is checked, choose next npc to check
        if(currentNPCTarget == null)
        {
            
            ChooseNextNPCTarget();
            return; 
        }

        //Update destination constantly so it adapts to pushed npcs

        agent.SetDestination(currentNPCTarget.position);  

        if (!agent.pathPending && agent.remainingDistance <= npcStoppingDistance)
        {
            StartCheckingTicket();
        }
    }


    void ChooseNextNPCTarget()
    {
        // Si no hay NPCManager todavía, simplemente nos quedamos sin target
        if (npcManager == null)
        {
            currentNPCTarget = null;
            return;
        }

        GameObject npcGO = null;
        try
        {
            npcGO = npcManager.GetNextPassenger(transform);
            
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"TicketControllerAI {name}: {e.Message}");
        }
    
        if (npcGO != null)
        {
            currentNPCTarget = npcGO.transform;
            agent.SetDestination(currentNPCTarget.position);
        }
        else
        {
            currentNPCTarget = null;
        }


    }


    // Checking ticket region: 

    void StartCheckingTicket()
    {
        currentState = ControllerState.CheckingTicket;
        checkTimer = checkTicketDuration;

        agent.isStopped = true;

        //
    }

void UpdateCheckingTicket()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            
            currentNPCTarget = null;
            
            StartPatrolling();
        }
    }


    //region : Chasing


    void StartChasing()
    {
        Debug.Log($"{name}: >>> StartChasing (state was {currentState})");
        currentState = ControllerState.Chasing;
        agent.isStopped = false;
        agent.speed = chaseSpeed;

    }

    void UpdateChasing()
    {
        if (player == null) return;

        agent.SetDestination(player.position);

        // Catch logic (for now just log; later you’ll call GameManager/AIManager)
        if (Vector3.Distance(transform.position, player.position) <= catchDistance)
        {
            Debug.Log("Player caught by controller: " + name);
            // TODO: GameManager.Instance.OnPlayerCaught();
        }
    }


    //region EXTERNAL TRIGGERS: 
    /// <summary>
    /// Call this from PlayerDetection when the controller sees the player.
    /// Later AIManager can call this too.
    /// </summary>

    public void SwitchToChasing()
    {
        Debug.Log($"{name}: SwitchToChasing called");
        StartChasing();
    }
    public void OnPlayerDetected()
    {
        if (currentState == ControllerState.Chasing)
            return;

        if(AIManager.Instance != null)
        {
            AIManager.Instance.PromotePatrollerToChaser(this); 
        }
        else
        {
            StartChasing(); 
        }
    }



    /// <summary>
    /// Helper if you want to force this controller to be a chaser from the start
    /// (e.g., the one who "knows your face").
    /// </summary>
    public void ForceChaserFromStart()
    {
        initialState = ControllerState.Chasing;
        if (agent != null)
            StartChasing();
    }

    public void SetNPCManager(NPCManager manager)
    {
        npcManager = manager;
    }
}
