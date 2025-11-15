using UnityEngine;
using UnityEngine.AI;

public class Ai_Nav_Chris : MonoBehaviour
{
    public Transform[] patrolToNPC;
    private int currentNPCIndex = 0;
    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis =false;

        agent.SetDestination(patrolToNPC[currentNPCIndex].position);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!agent.pathPending && agent.remainingDistance < 0.1f)
        {

            currentNPCIndex = (currentNPCIndex + 1) % patrolToNPC.Length;
            agent.SetDestination(patrolToNPC[currentNPCIndex].position);

        }
        
    }
}
