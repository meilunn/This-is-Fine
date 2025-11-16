using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pause_CenterOnPlayer : MonoBehaviour
{
    //SetUp: Use PauseCanvas Prefab, add the CameraControlScript to the Camera, lastly fill in the SerializeFields

    
    [SerializeField] private TextMeshProUGUI tippText;
    [SerializeField] private GameObject pauseImage; 

    [Header("Camera Control")]
    [SerializeField] private GameObject cameraObject;
    private CameraControlScript myCameraControlScript;

    public static bool isPaused;

    void Start()
    {
        if (cameraObject != null)
        {
            myCameraControlScript = cameraObject.GetComponent<CameraControlScript>();
        }
        else
        {
            Debug.LogError("CameraObject not assigned in Inspector!");
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!isPaused)
        {
            Time.timeScale = 0;
            if (pauseImage != null) pauseImage.SetActive(true);

            if (myCameraControlScript != null)
                myCameraControlScript.ShiftLeft25(); 


            RandomText(); 

            isPaused = true;
        }
        else
        {
            if (myCameraControlScript != null)
                myCameraControlScript.Restore();

            if (pauseImage != null) pauseImage.SetActive(false);

            Time.timeScale = 1;
            isPaused = false;
        }
    }


    private void RandomText()
    {
        int randomInt = Random.Range(0, 10);

        switch (randomInt)
        {
            case 0: tippText.text = "Push objects or people to block the controller's path.";
            break; 
            case 1: tippText.text = "Change trains to evade the controllers.";
            break; 
            case 2: tippText.text = //"There is a bar on the top right corner that shows how many controllers are in which waggon.";
                                    "There is a bar on the top right corner that shows when you come close to the next destination";
            break; 
            case 3: tippText.text = "You already ticket-dodged a few times, some of the controllers may know your face. Watch out.";
            break; 
            case 4: tippText.text = "Be aware, every train has its own curiosities.";
            break; 
            case 5: tippText.text = //"Controllers can communicate, don't be suspicious";
                                    "Change trains three times to reach your destination and win the game.";
            break; 
            case 6: tippText.text = "The music indicates your progress.";
            break; 
            case 7: tippText.text = "Don't get to close to the passive controllers, they will chase you.";
            break; 
            case 8: tippText.text = "Also try Minecraft!";
            break; 
            //this is not the real API key, don't worry :)
            case 9: tippText.text = "c795259ab473886fbbe6332dd3d44210";
            break; 
            //unless...
        }

    }
}
