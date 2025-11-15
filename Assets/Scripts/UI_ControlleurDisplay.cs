using UnityEngine;
using UnityEngine.Events;

public class UI_ControlleurDisplay : MonoBehaviour
{
    public UnityEvent newControlleur;

    public void UpdateControlleurs(int waggon, int sumExisting)
    {
        string waggonName = $"DisplayWaggon{waggon}";
        Transform waggonTransform = transform.Find(waggonName);

        if (waggonTransform == null)
        {
            Debug.LogError($"Could not find {waggonName}");
            return;
        }

        // Reset ONLY THIS waggon
        foreach (Transform image in waggonTransform)
        {
            image.gameObject.SetActive(false);
        }

        // Enable the required amount
        for (int i = 0; i < sumExisting; i++)
        {
            Transform t = waggonTransform.Find($"Image{i}");
            if (t != null)
                t.gameObject.SetActive(true);
        }
    }
}
