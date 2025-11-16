using UnityEngine;
using Image = UnityEngine.UI.Image;

public class FadeInOutScript : MonoBehaviour
{
    public static FadeInOutScript Instance;
    
    public Image image;

    public float fadingTime = 2f;
    private float currentFadingTime = 0f;
    
    private bool fadingOut = false;
    private bool fadingIn = false;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void startFadeOut()
    {
        fadingOut = true;
        fadingIn = false;
        currentFadingTime = fadingTime;
    }
    
    // public void startFadeIn()
    // {
    //     fadingIn = true;
    //     fadingOut = false;
    //     currentFadingTime = fadingTime;
    // }

    private void Update()
    {
        if (fadingIn || fadingOut)
        {
            currentFadingTime -= Time.deltaTime;
            
            if (currentFadingTime < 0)
            {
                if (fadingOut)
                {
                    fadingOut = false;
                    fadingIn = true;
                }
                else
                {
                    fadingIn = false;
                }
                currentFadingTime = 0;
            }
            
            Color color = image.color;
            color.a = currentFadingTime / fadingTime;
            if (fadingIn)
            {
                color.a = (1 - currentFadingTime) / fadingTime;
            }
            image.color = color;
        }
    }
}
