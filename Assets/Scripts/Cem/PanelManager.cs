using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;


    [Header("Register all panels here")]
    public List<PanelBase> panels;


    private void Awake()
    {
        // Singleton 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        HideAll();

        
    }

    public void Show(string panelName)
    {
        foreach (var entry in panels)
            entry.gameObject.SetActive(false);

        var panel = panels.Find(entry => entry.panelName.Equals(panelName));
        if(panel != null)
        {

            panel.ShowPanel();
        }
        else
        {
            Debug.LogWarning($"Panel '{panelName}' not found!");
        }
    }

    public void Hide(string panelName)
    {

        var panel = panels.Find(entry => entry.panelName.Equals(panelName));
        if (panel != null)
        {

            panel.HidePanel();
        }
        else
        {
            Debug.LogWarning($"Panel '{panelName}' not found!");
        }
    }

    public void HideAll()
    {
        foreach (var entry in panels)
            entry.HidePanel();
    }
}
