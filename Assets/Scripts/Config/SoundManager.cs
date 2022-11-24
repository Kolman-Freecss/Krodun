using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance { get; set; }

    [Range(0, 100)] public float EffectsAudioVolume = 50f;
    [Range(0, 100)] public float MusicAudioVolume = 50f;


    public static SoundManager instance
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<SoundManager>();
            }

            return Instance;
        }
    }

    private void Awake()
    {
        ManageSingleton();
    }

    public void SetEffectsVolume(float volume)
    {
        EffectsAudioVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        GetComponent<AudioSource>().volume = volume;
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
}