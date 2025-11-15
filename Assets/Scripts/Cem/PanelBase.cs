using UnityEngine;

public abstract class PanelBase : MonoBehaviour
{
    public string panelName;



    public virtual void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    public virtual void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
