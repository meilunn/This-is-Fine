using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Train,
    FullTrack,
    LoopTrack,
    Walk,
    DoorClose,
    Grunt,
    PushHit,
    PushMiss,
    Shock,
    WalkOut,
    Lose,
    Win
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;

    public static SoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        // Simple singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        if (instance == null) return;

        AudioClip clip = instance.soundList[(int)sound];
        if (clip == null) return;
        instance.audioSource.PlayOneShot(clip, volume);

        
    }

    public static void PlayLoop(SoundType sound, float volume = 1f)
    {
        if (instance == null) return;

        AudioClip clip = instance.soundList[(int)sound];
        if (clip == null) return;

        instance.audioSource.clip = clip;
        instance.audioSource.volume = volume;
        instance.audioSource.loop = true;
        instance.audioSource.Play();
    }

    public static void StopLoop()
    {
        if (instance == null || instance.audioSource == null)
            return;

        instance.audioSource.loop = false;
        instance.audioSource.Stop();
        instance.audioSource.clip = null;
    }

    // Play one sound, then another when the first finishes
    public static void PlaySoundAfter(SoundType first, SoundType second, float firstVolume = 1f, float secondVolume = 1f)
    {
        if (instance == null) return;
        instance.StartCoroutine(instance.PlaySoundAfterRoutine(first, second, firstVolume, secondVolume));
    }

    private IEnumerator PlaySoundAfterRoutine(SoundType first, SoundType second, float firstVolume, float secondVolume)
    {
        AudioClip firstClip = soundList[(int)first];
        AudioClip secondClip = soundList[(int)second];

        if (firstClip == null || secondClip == null)
            yield break;

        // Play first
        audioSource.PlayOneShot(firstClip, firstVolume);

        // Wait until first finishes
        float x = 0;
        if(firstClip == soundList[6]) x = 0.8f;
        yield return new WaitForSeconds(firstClip.length - x);


        // Then play second
        audioSource.PlayOneShot(secondClip, secondVolume);
    }
}
