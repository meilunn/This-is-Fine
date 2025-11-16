using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting; // This namespace is often not needed, can be removed if you don't use it.
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_ProgressDisplay : MonoBehaviour
{
    // --- 1. ADD THIS "READY" FLAG ---
    public bool isReady { get; private set; } = false;

    // This is no longer needed, call StartTravel(duration) instead
    // public bool trainDeparts;
    public bool trainTravels { get; private set; } // Made public-get, private-set
    
    [Header("Travel Setup")]
    [SerializeField] private float threshhold; // The target X-position for the *last* station

    private List<Tuple<int, bool>> trackDisplayLength;

    // This array is no longer used, duration is passed as a parameter
    // private int[] notSketchyListAtAll= new int[3];

    [Header("Prefabs & Hierarchy")]
    [SerializeField] private List<int> testValues;
    [SerializeField] private GameObject trainStation;
    [SerializeField] private GameObject track;
    [SerializeField] private GameObject trainPrefab; 
    [SerializeField] private GameObject parentDisplayTracks;
    
    // This is now calculated automatically
    // [SerializeField] private Vector3 speed;

    private List<RectTransform> spawnedTiles = new List<RectTransform>();
    private List<int> stationIndices = new List<int>(); 
    
    // New variables for the calculated movement
    private List<Vector2> initialTilePositions = new List<Vector2>();
    private float totalDistanceToTravel;
    
    private UI_ControlleurDisplay myControlleurDisplay;
    
    private Animator anim; // This will hold the train's animator

    void Start()
    {
        //        myControlleurDisplay = myControlleurDisplayObject.GetComponent<UI_ControlleurDisplay>();

        // Durations are no longer hard-coded here
        // notSketchyListAtAll[0] = 30; ...

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
                stationIndices.Add(tileIndex); // Save the index of the station tile
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
        
        // Create the train and save its instance
        GameObject trainInstance = Instantiate(trainPrefab, parentDisplayTracks.transform);
        // Get the Animator from that new instance
        anim = trainInstance.GetComponent<Animator>(); 

        // Waiting for Layout Group to position everything
        StartCoroutine(InitializeAfterLayout());
    }

    private IEnumerator InitializeAfterLayout()
    {
        // Waiting for layout to rebuild (at least 2 frames)
        yield return null;
        yield return null;
        
        if (stationIndices.Count == 0)
        {
            Debug.LogError("No stations were instantiated! Cannot calculate travel distance.", this);
            isReady = false; // Mark as failed
            yield break;
        }

        // --- New Logic ---
        // 1. Save the initial position of ALL tiles
        foreach (RectTransform tile in spawnedTiles)
        {
            initialTilePositions.Add(tile.anchoredPosition);
        }
        
        // 2. Calculate the total distance the *last* station needs to move
        // Get the index of the last station
        int lastStationTileIndex = stationIndices[stationIndices.Count - 1];
        RectTransform lastStation = spawnedTiles[lastStationTileIndex];
        
        // Calculate distance from its starting X to the target X (threshhold)
        totalDistanceToTravel = lastStation.anchoredPosition.x - threshhold;
        
        Debug.Log($"Initialization complete. Total travel distance calculated: {totalDistanceToTravel} units.", this);
        
        // --- 2. ADD THIS LINE AT THE VERY END ---
        // Tell the world we are now ready to be called!
        isReady = true;
    }

    
    // --- THIS IS THE NEW PUBLIC COROUTINE ---
    
    /// <summary>
    /// Starts the train travel, moving the entire track over a specific duration.
    /// The speed is calculated to make the trip last exactly this long.
    /// </summary>
    /// <param name="duration">The total time (in seconds) the travel should take.</param>
    public IEnumerator StartTravel(float duration)
    {
        // --- ADD A READY CHECK ---
        if (!isReady)
        {
            Debug.LogError("StartTravel was called, but UI_ProgressDisplay is not ready! (Did InitializeAfterLayout finish?)", this);
            yield break;
        }

        if (trainTravels)
        {
            Debug.LogWarning("Train is already traveling!", this);
            yield break; // Don't start a new trip if one is in progress
        }

        if (duration <= 0)
        {
            Debug.LogError($"Travel duration must be positive. Received {duration}s.", this);
            yield break;
        }

        trainTravels = true;
        if (anim != null) anim.SetBool("trainIsTravelling", true);
        Debug.Log($"Train travel started. Duration: {duration} seconds.");

        float timer = 0f;
        
        // --- Time-Based Travel Loop (using Lerp for precision) ---
        while (timer < duration)
        {
            // Calculate progress from 0.0 to 1.0
            float progress = timer / duration;
            
            // Calculate how far we should have moved at this point in time
            float currentDistance = Mathf.Lerp(0, totalDistanceToTravel, progress);

            // Move all tiles based on their initial position minus the current distance
            for (int i = 0; i < spawnedTiles.Count; i++)
            {
                float newX = initialTilePositions[i].x - currentDistance;
                spawnedTiles[i].anchoredPosition = new Vector2(newX, initialTilePositions[i].y);
            }

            timer += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        
        // --- End of Travel ---
        
        // Snap all tiles to their final exact position to ensure precision
        for (int i = 0; i < spawnedTiles.Count; i++)
        {
            float finalX = initialTilePositions[i].x - totalDistanceToTravel;
            spawnedTiles[i].anchoredPosition = new Vector2(finalX, initialTilePositions[i].y);
        }

        trainTravels = false;
        if (anim != null) anim.SetBool("trainIsTravelling", false);
        Debug.Log($"Train travel complete. (Time elapsed: {timer:F2}s).");
    }


    private List<Tuple<int, bool>> FillWithValues(List<int> IntList)
    {
       List<Tuple<int, bool>> returnList = new();

       for (int i = 0; i < IntList.Count; i++)
       {
           int value = IntList[i];
           // A value of -1 in testValues signifies a station
           bool isStation = value == -1; 

            returnList.Add(Tuple.Create(value, isStation));
        }

    return returnList;
    }


    // Update is called once per frame
    void Update()
    {
        // All logic is in the coroutine, so this can be empty!
    }
}