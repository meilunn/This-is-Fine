using UnityEngine;
public class OptionPanel : PanelBase
{
    public override void ShowPanel()
    {
        base.ShowPanel();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HidePanel();
        }
    }
}
