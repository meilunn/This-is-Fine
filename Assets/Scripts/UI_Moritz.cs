using UnityEngine;

public class UI_Moritz : MonoBehaviour
{
    //only visualize 2 tracks and 3 stations at once
    //[SerializeField] GameObject trackElement;
    [SerializeField] int[] trackDisplayLength;
    [SerializeField] GameObject[] wholeDisplayTracks;
    [SerializeField] GameObject parentDisplayTracks;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //for every element in the wDTracks go through the other loop
        for (int i = 0; i < wholeDisplayTracks.Length; i++)
        {
            //for every element in tDLength instantiate, tracks will get instantiated multiple times
            for (int j = 0; j < trackDisplayLength[i]; j++)
            {
                Instantiate(wholeDisplayTracks[i], parentDisplayTracks.transform);
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {

    }
    

}
