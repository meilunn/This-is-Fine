using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_main : MonoBehaviour
{
    public bool trainDeparts;
    public bool trainTravels;

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


    void Start()
    {
        
        trackDisplayLength = FillWithValues(testValues);


        //go through all the elements
        for (int i = 0; i < trackDisplayLength.Count; i++)
            {
                int amount = trackDisplayLength[i].Item1;
                bool isStation = trackDisplayLength[i].Item2;

                if (isStation)
                {
                    var obj = Instantiate(trainStation, parentDisplayTracks.transform);
                    spawnedTiles.Add(obj.transform);

                }
                else
                {
                    for (int j = 0; j < amount; j++)
                    {
                        var obj = Instantiate(track, parentDisplayTracks.transform);
                        spawnedTiles.Add(obj.transform);

                    }
                }
           
            }
        Instantiate(train, parentDisplayTracks.transform);

        StartCoroutine(MoveTrainBackground());

    }


    private IEnumerator MoveTrainBackground()
    {
        while (trainTravels)
        {
            foreach (Transform tile in spawnedTiles)
            {
                tile.position += speed * Time.deltaTime;
            }

            yield return null; //!!!
        }

        // stop points, also stop when last train station reached 
        yield return new WaitUntil(() => trainDeparts);
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
