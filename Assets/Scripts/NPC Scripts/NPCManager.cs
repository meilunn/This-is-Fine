using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCManager : MonoBehaviour
{

    public GameObject[] NPCPrefabs;
    
    public Sprite[] standingNPCSprites;
    public Sprite[] sittingNPCSprites;

    [Serializable]
    public class SpawnPoint
    {
        public Transform spawnPoint;
        public SpawnType spawnType;
        public bool useCustomRotation;
        [Range(0f, 360f)]
        public float rotationAngle;
    }
    public enum SpawnType
    {
        Standing, Sitting
    }
    
    [SerializeField] 
    private SpawnPoint[] npcSpawnPoints;
    
    public int minPassengerAmount;
    public int maxPassengerAmount;
    public int passengerAmountDecrease;

    [SerializeField]
    private int currentPassengerAmountDecrease;
    
    [SerializeField] 
    private List<GameObject> npcPassengers = new List<GameObject>();
    private List<GameObject> checkedNpcPassengers = new List<GameObject>();

    public void IntializeNPCs(SpawnPoint[] npcSpawnPoints)
    {
        Debug.Log("Initializing NPCs");
        this.npcSpawnPoints = npcSpawnPoints;
        checkedNpcPassengers.Clear();
        npcPassengers.Clear();
        ChooseSpawnLocations();
    }
    
    public void IntializeNPCs(SpawnPoint[] npcSpawnPoints, int difficultyLevel)
    {
        IntializeNPCs(npcSpawnPoints);
        currentPassengerAmountDecrease = difficultyLevel * passengerAmountDecrease;
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
        Debug.Log(currentPassengerAmount);
        currentPassengerAmount = Math.Max(0, currentPassengerAmount - currentPassengerAmountDecrease);
        Debug.Log(currentPassengerAmount);
        currentPassengerAmount = Mathf.Min(currentPassengerAmount, npcSpawnPoints.Length);
        Debug.Log(currentPassengerAmount);

        List<int> pI = new List<int>();

        for (int i = 0; i < currentPassengerAmount; i++)
        {
            int pIndex;
            do
            {
                pIndex = Random.Range(0, npcSpawnPoints.Length);
            } while (pI.Contains(pIndex));

            pI.Add(pIndex);

            int r = i % NPCPrefabs.Length;
            GameObject npc = Instantiate(NPCPrefabs[r]);
            SpawnPoint point = npcSpawnPoints[pIndex];
            npc.transform.position = point.spawnPoint.position;

            if (point.spawnType == SpawnType.Standing)
            {
                float rotation = Random.Range(0f, 360f);
                if (point.useCustomRotation)
                {
                    rotation = point.rotationAngle;
                }
                Quaternion newRotation = Quaternion.AngleAxis(rotation, Vector3.forward);
                npc.transform.rotation = newRotation;
                SpriteRenderer sR = npc.GetComponent<SpriteRenderer>();
                if (standingNPCSprites.Length > 0)
                {
                    sR.sprite = standingNPCSprites[Random.Range(0, standingNPCSprites.Length)];
                }
            }
            else if (point.spawnType == SpawnType.Sitting)
            {
                Quaternion newRotation = Quaternion.AngleAxis(point.rotationAngle, Vector3.forward);
                npc.transform.rotation = newRotation;
                SpriteRenderer sR = npc.GetComponent<SpriteRenderer>();
                if (standingNPCSprites.Length > 0)
                {
                    sR.sprite = sittingNPCSprites[Random.Range(0, sittingNPCSprites.Length)];
                }
            }
            npcPassengers.Add(npc);
        }
        
        Debug.Log("Created " + currentPassengerAmount + " Passengers");
    }

    public GameObject GetNextPassenger(Transform controller)
    {
        if (npcPassengers.Count <= 0)
        {
            npcPassengers.AddRange(checkedNpcPassengers);
            checkedNpcPassengers.Clear();
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
