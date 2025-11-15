using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCManager : MonoBehaviour
{

    public GameObject NPCPrefab;
    
    [SerializeField] 
    private Transform[] npcLocations;
    public int minPassengerAmount;
    public int maxPassengerAmount;
    public int passengerAmountDecrease;
    
    [SerializeField] 
    private List<GameObject> npcPassengers;
    private List<GameObject> checkedNpcPassengers;
    
    private void Awake()
    {
        npcPassengers = new List<GameObject>();
        checkedNpcPassengers = new List<GameObject>();
    }

    public void IntializeNPCs(Transform[] npcLocations)
    {
        this.npcLocations = npcLocations;
        checkedNpcPassengers.Clear();
        npcPassengers.Clear();
        ChooseSpawnLocations();
    }

    public void CleanUpNpcPassengers()
    {
        for (var i = 0; i < checkedNpcPassengers.Count; i++)
        {
            GameObject.Destroy(checkedNpcPassengers[i]);
        }
        checkedNpcPassengers.Clear();
        for (var i = 0; i < npcPassengers.Count; i++)
        {
            GameObject.Destroy(npcPassengers[i]);
        }
        npcPassengers.Clear();
    }

    private void ChooseSpawnLocations()
    {
        int currentPassengerAmount = Random.Range(minPassengerAmount, maxPassengerAmount);
        List<int> pI = new List<int>();
        for (int i = 0; i < currentPassengerAmount; i++)
        {
            int pIndex;
            do
            {
                pIndex = Random.Range(0, npcLocations.Length);
            } while (pI.Contains(pIndex));
            pI.Add(pIndex);

            GameObject npc = Instantiate(NPCPrefab);
            npc.transform.position = npcLocations[pIndex].position;
            npcPassengers.Add(npc);
        }
    }

    public GameObject GetNextPassenger(Transform controller)
    {
        if (npcPassengers.Count <= 0)
        {
            throw new Exception("There are no npc passengers left!");
        }
        int pIndex = 0;
        float maxDistance = int.MaxValue;
        for (int i = 1; i < npcPassengers.Count; i++)
        {
            float currentDistance = Vector3.Distance(controller.position, npcPassengers[i].transform.position);
            if (currentDistance < maxDistance)
            {
                pIndex = i;
                maxDistance = currentDistance;
            }
        }

        GameObject p = npcPassengers[pIndex];
        checkedNpcPassengers.Add(p);
        npcPassengers.Remove(p);
        return p;
    }

    public void ReturnPassenger(GameObject passenger)
    {
        checkedNpcPassengers.Remove(passenger);
        npcPassengers.Add(passenger);
    }
}
