using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ProgressDisplay : MonoBehaviour
{

    //! set trainDeparts to true, in order to start the train again !
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
    private List<RectTransform> spawnedTiles = new List<RectTransform>();
    private List<int> stationIndices = new List<int>(); 
    private List<float> stationCenterPositions = new List<float>(); 


    void Start()
    {
        trackDisplayLength = FillWithValues(testValues);

        int tileIndex = 0;
        
        for (int i = 0; i < trackDisplayLength.Count; i++)
            {
                int amount = trackDisplayLength[i].Item1;
                bool isStation = trackDisplayLength[i].Item2;

                if (isStation)
                {
                    var obj = Instantiate(trainStation, parentDisplayTracks.transform);
                    RectTransform rectTransform = obj.GetComponent<RectTransform>();
                    spawnedTiles.Add(rectTransform);
                    stationIndices.Add(tileIndex);
                    tileIndex++;
                }
                else
                {
                    for (int j = 0; j < amount; j++)
                    {
                        var obj = Instantiate(track, parentDisplayTracks.transform);
                        RectTransform rectTransform = obj.GetComponent<RectTransform>();
                        spawnedTiles.Add(rectTransform);
                        tileIndex++;
                    }
                }
           
            }
        Instantiate(train, parentDisplayTracks.transform);

        // Waiting for Layout Group to position everything, then capture positions
        StartCoroutine(InitializeAfterLayout());
    }

    private IEnumerator InitializeAfterLayout()
    {
        // Waiting for layout to rebuild (at least 2 frames)
        yield return null;
        yield return null;
        
        // captures the ACTUAL positions after Layout Group has positioned them
        for (int i = 0; i < stationIndices.Count; i++)
        {
            RectTransform station = spawnedTiles[stationIndices[i]];
            // Gets actual center position (anchoredPosition is the center by default)
            stationCenterPositions.Add(station.anchoredPosition.x);
        }
        
        trainTravels = true;
        StartCoroutine(MoveTrainBackground());
    }


    private IEnumerator MoveTrainBackground()
    {
        int currentStationIndex = 0;
        
        while (currentStationIndex < stationIndices.Count)
        {
            // Move until next station center reached
            trainTravels = true;
            
            while (trainTravels)
            {
                foreach (RectTransform tile in spawnedTiles)
                {
                    tile.anchoredPosition += (Vector2)speed * Time.deltaTime;
                }

                if (currentStationIndex < stationIndices.Count)
                {
                    RectTransform station = spawnedTiles[stationIndices[currentStationIndex]];
                    
                    // Get the initial center position captured
                    float initialStationCenter = stationCenterPositions[currentStationIndex];
                    
                    // Calculate how far this station should be from its original position to reach threshold
                    float targetPosition = threshhold;
                    
                    // Stop when the station reaches the threshold
                    if (station.anchoredPosition.x <= targetPosition) 
                    {
                        trainTravels = false;
                    }
                }

                yield return null;
            }

            // Train has reached station position
            currentStationIndex++;
            
            // Only wait for trainDeparts if it's NOT the first station
            if (currentStationIndex > 1) // Skip waiting for first station
            {
                // Wait until trainDeparts is set to true again
                yield return new WaitUntil(() => trainDeparts);
                trainDeparts = false; // Reset for next station
            }
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