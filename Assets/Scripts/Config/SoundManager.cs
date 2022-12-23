using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Range(0, 100)] public float EffectsAudioVolume = 50f;
    [Range(0, 100)] public float MusicAudioVolume = 40f;
    
    public AudioClip ButtonClickSound;

    private void Awake()
    {
        ManageSingleton();
    }

    private void Start()
    {
        SetEffectsVolume(EffectsAudioVolume);
        SetMusicVolume(MusicAudioVolume);
    }

    public void SetEffectsVolume(float volume)
    {
        EffectsAudioVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        GetComponent<AudioSource>().volume = volume / 100;
    }

    public float GetSoundVolume()
    {
        return EffectsAudioVolume;
        //EffectsAudioVolume = PlayerPrefs.GetFloat("EffectsVolume", 1f);
    }

    void ManageSingleton()
    {
        if (Instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void PlayButtonClickSound(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(ButtonClickSound, position, EffectsAudioVolume / 100);
    }
}