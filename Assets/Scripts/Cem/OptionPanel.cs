using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionPanel : PanelBase
{
    public AudioMixer mixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Load saved values
        float master = PlayerPrefs.GetFloat("Master", 0);
        float music = PlayerPrefs.GetFloat("Background", 0);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0);

        masterSlider.value = master;
        musicSlider.value = music;
        sfxSlider.value = sfx;

        SetMaster(master);
        SetMusic(music);
        SetSFX(sfx);
    }

    public void SetMaster(float value)
    {
        mixer.SetFloat("MasterVolume", value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusic(float value)
    {
        mixer.SetFloat("MusicVolume", value);
        PlayerPrefs.SetFloat("BackgroundVolume", value);
    }

    public void SetSFX(float value)
    {
        mixer.SetFloat("SFXVolume", value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
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
