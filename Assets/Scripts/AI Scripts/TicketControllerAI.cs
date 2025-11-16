using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class TicketControllerAI  : MonoBehaviour
{

        public enum ControllerState
    {
        Patrolling,
        CheckingTicket,
        Shocked, 
        Chasing
    }

     [Header("General")]
    public ControllerState initialState = ControllerState.Patrolling;
    public Transform player;                       // assign in inspector for now, later change to moving player
    public Transform[] patrolToNPC;                // NPC transforms 
    public float npcStoppingDistance = 0.15f;      // how close to get to an NPC
    public float checkTicketDuration = 1.5f;       // time spent "checking" a ticket
    public float catchDistance = 0.6f;             // distance to catch the player
    public float chaseSpeed = 4.5f;
    public float patrolSpeed = 3.0f;

    [Header("Chasing / Detection")]
    public float shockedTime = 1.0f;
    [Header("References")]
    [SerializeField] private NPCManager npcManager;   // se setea desde AIManager

    private NavMeshAgent agent;
    private ControllerState currentState;
    private int currentNPCIndex = 0;
    private Transform currentNPCTarget;
    private float checkTimer;
    private float shockedTimer;


    [Header("FOV / Vision")]
    public float fovAngle = 60f;
    public float fovRange = 2.0f;
    public Vector2 lookDirection = Vector2.right; // default facing right

    public bool IsChasing => currentState == ControllerState.Chasing;


    [Header("Animation")] 
    private Animator _animator;
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer ViewCone;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        float MoveX = agent.velocity.x;
        float MoveY = agent.velocity.y;
        _animator.SetFloat("MoveX", MoveX);
        _animator.SetFloat("MoveY", MoveY);
        
        if (Mathf.Abs(MoveX) > Mathf.Abs(MoveY) && Mathf.Abs(MoveX) > 0.1f)
        {
            spriteRenderer.flipX = MoveX > 0;
        }
        
        //float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
        //Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        //transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 100);
        switch (currentState)
        {
             case ControllerState.Patrolling:
                UpdatePatrolling();
                break;
            case ControllerState.CheckingTicket:
                UpdateCheckingTicket();
                break;
            case ControllerState.Shocked:         
                UpdateShocked();
            break;
            case ControllerState.Chasing:
                UpdateChasing();
                break;
        }
        Vector2 vel = agent.velocity;
        if (vel.sqrMagnitude > 0.001f)
        {
            lookDirection = vel.normalized;
        }

        // PATROLLING sight cone → detect player and start chase
        if (currentState == ControllerState.Patrolling && player != null)
        {
            if (IsTargetInsideFov(player))
            {
                Debug.Log($"{name}: Player seen in FOV, notifying AIManager.");
                OnPlayerDetected();   // this will promote via AIManager
                SoundManager.PlaySound(SoundType.Shock);
            }
        }

            Vector3 dir = lookDirection;
            Vector2 dir2 = new Vector2(dir.x, dir.y);
            float angle = Vector2.Angle(Vector2.up, dir2) + 180f;
            if (dir.x > 0)
            {
                angle = Vector2.Angle(Vector2.down, dir2);
            }

            ViewCone.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            ViewCone.gameObject.transform.position = transform.position + dir * fovRange / 2;
            //ViewCone.gameObject.transform.scale.y = fovRange;
            //ViewCone.gameObject.transform.scale.x = fovAngle;
        
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
    void StartShocked()
    {
        Debug.Log($"{name}: >>> StartShocked");
        currentState = ControllerState.Shocked;
        shockedTimer = shockedTime;

        agent.isStopped = true;      // freeze in place
        agent.velocity = Vector3.zero;
    }

    void UpdateShocked()
    {
        shockedTimer -= Time.deltaTime;
        if (shockedTimer <= 0f)
        {
            Debug.Log($"{name}: Shocked done, now start chasing.");
            StartChasing();
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



    public bool IsTargetInsideFov(Transform target)
{
    if (target == null) return false;

    Vector2 toTarget = (target.position - transform.position);
    float distance = toTarget.magnitude;
    if (distance > fovRange) return false;

    // If we’re not moving, keep the last lookDirection
    if (lookDirection.sqrMagnitude < 0.0001f)
        return false; // or keep last, depending how you want it

    Vector2 dirToTargetNorm = toTarget.normalized;
    float angleToTarget = Vector2.Angle(lookDirection, dirToTargetNorm);

    return angleToTarget <= fovAngle * 0.5f;
}
        public float lineWidth = 0.05f;

    private Rigidbody2D rb;
    private LineRenderer line;
    private Vector2 lastMoveDir = Vector2.right; // fallback if standing still


    

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

        float dist = Vector2.Distance(transform.position, player.position);
    if (dist <= catchDistance)
    {
        OnPlayerCaught();
    }
    }


    //region EXTERNAL TRIGGERS: 
    /// <summary>
    /// Call this from PlayerDetection when the controller sees the player.
    /// Later AIManager can call this too.
    /// </summary>

    public void SwitchToChasing()
    {
        StartShocked();
        Debug.Log($"{name}: SwitchToChasing called");
        
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
    public void OnPlayerCaught()
{
Debug.Log($"{name}: Player caught – you get a fine! 💸");

    // Optional: stop this controller

if (dist <= catchDistance)
{
        agent.isStopped = true;
    agent.velocity = Vector3.zero;
    // Tell GameManager to end the game
    if (GameManager.Instance != null &&
        GameManager.Instance.CurrentGameStage != GameState.EndGame)
    {
        GameManager.Instance.GameOver();
    }
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



    public void StartShockedExternal()
{
    // Only stun if chasing
    if (currentState != ControllerState.Chasing)
        return;

    StartShocked();

}
}
