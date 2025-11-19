using System;
using TMPro;
using UnityEngine;

public class ShowTimeScript : MonoBehaviour
{
    [SerializeField] private Timer timer;

    private TMP_Text text;
    
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int time = (int) Math.Max(0, timer.GetTotalTime());
        text.text = time == 0 ? "You can now exit the wagon on the top side." : "Time left: " + time;
    }
}
