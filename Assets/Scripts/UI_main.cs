using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_main : MonoBehaviour
{
    public bool trainDeparts;
    public bool trainTravels;
    [SerializeField] private float threshhold;

    private List<Tuple<int, bool>> trackDisplayLength;

    //
    //every second int value will be ignored! this is for train stations
    //...they have a true bool
    //
    [SerializeField] private List<int> testValues;
    [SerializeField] private GameObject trainStation;
    [SerializeField] private GameObject track;
    [SerializeField] private GameObject train;
    [SerializeField] private GameObject parentDisplayTracks;
    [SerializeField] private Vector3 speed;
    private List<Transform> spawnedTiles = new List<Transform>();
    private List<int> stationIndices = new List<int>(); // Track where stations are


    void Start()
    {
        
        trackDisplayLength = FillWithValues(testValues);


        //go through all the elements and track station positions
        int tileIndex = 0;
        for (int i = 0; i < trackDisplayLength.Count; i++)
            {
                int amount = trackDisplayLength[i].Item1;
                bool isStation = trackDisplayLength[i].Item2;

                if (isStation)
                {
                    var obj = Instantiate(trainStation, parentDisplayTracks.transform);
                    spawnedTiles.Add(obj.transform);
                    stationIndices.Add(tileIndex); // Remember this is a station
                    tileIndex++;
                }
                else
                {
                    for (int j = 0; j < amount; j++)
                    {
                        var obj = Instantiate(track, parentDisplayTracks.transform);
                        spawnedTiles.Add(obj.transform);
                        tileIndex++;
                    }
                }
           
            }
        Instantiate(train, parentDisplayTracks.transform);

        trainTravels = true;
        StartCoroutine(MoveTrainBackground());

    }


    private IEnumerator MoveTrainBackground()
    {
        int currentStationIndex = 0;
        
        while (currentStationIndex < stationIndices.Count)
        {
            // Move until we reach the next station
            trainTravels = true;
            
            while (trainTravels)
            {
                foreach (Transform tile in spawnedTiles)
                {
                    tile.position += speed * Time.deltaTime;
                }

                // Check if we've reached the station position
                // Assuming train starts at position 0 and stations move in negative direction
                if (currentStationIndex < stationIndices.Count)
                {
                    Transform station = spawnedTiles[stationIndices[currentStationIndex]];
                    
                    // Stop when station reaches a specific position (e.g., center of screen)
                    // Adjust the threshold value based on your needs
                    if (station.position.x <= threshhold) // Adjust this value to your train position
                    {
                        trainTravels = false;
                    }
                }

                yield return null;
            }

            // Train has stopped at station
            currentStationIndex++;
            
            // Wait until trainDeparts is set to true again
            yield return new WaitUntil(() => trainDeparts);
            trainDeparts = false; // Reset for next station
        }
        
        // All stations visited
        Debug.Log("Train has reached all stations!");
    }


    private List<Tuple<int, bool>> FillWithValues(List<int> IntList)
    {
       List<Tuple<int, bool>> returnList = new();

       for (int i = 0; i < IntList.Count; i++)
       {
           int value = IntList[i];
           bool isStation = (value == 0);

            returnList.Add(Tuple.Create(value, isStation));
        }

    return returnList;
    }


    // Update is called once per frame
    void Update()
    {

    }
    

}