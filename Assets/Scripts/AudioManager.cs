using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public float volume = 0.1f;
    public float musicVolume = 0.05f;
    public AudioClip musicClip;
    public AudioClip winClip;
    private static AudioManager _instance;

    // Public instance to access the AudioManager
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Search for existing instance
                _instance = FindObjectOfType<AudioManager>();

                // If no instance found, create a new one
                if (_instance == null)
                {
                    GameObject singleton = new GameObject("AudioManager");
                    _instance = singleton.AddComponent<AudioManager>();
                    //DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // If an instance already exists and it's not this one, destroy this one
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this one
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to the OnSceneLoaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnSceneLoaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Play the music clip when a new scene is loaded
        PlayMusic(musicClip, true);
    }

    private void Start()
    {
        PlayMusic(musicClip, true);
    }

    

    // Method to play a sound effect
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null. Cannot play sound.");
            return;
        }
        //Debug.Log("Playing SFX"+ clip.name);
        // Create a temporary GameObject
        GameObject tempAudioObject = new GameObject("TempAudio");
        tempAudioObject.transform.position = AudioManager.Instance.transform.position;
        // Add an AudioSource component to the GameObject
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
        // Set the clip to the AudioSource
        audioSource.clip = clip;

        // Play the clip
        audioSource.Play();

        // Destroy the GameObject after the clip has finished playing
        Destroy(tempAudioObject, clip.length);
    }

    public void PlaySFX(AudioClip clip, float delay)
    {
        StartCoroutine(PlaySFXRoutine(clip, delay));
    }

    IEnumerator PlaySFXRoutine(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySFX(clip);
    }
    public void PlayMusic(AudioClip musicClip, bool isLooping = false)
    {
        if (musicClip == null)
        {
            Debug.LogWarning("AudioClip is null. Cannot play music.");
            return;
        }

        // Create a temporary GameObject
        GameObject tempAudioObject = new GameObject("MusicObj");
        tempAudioObject.transform.position = AudioManager.Instance.transform.position;

        // Add an AudioSource component to the GameObject
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.volume = musicVolume;
        audioSource.loop = isLooping;
        // Set the clip to the AudioSource
        audioSource.clip = musicClip;

        // Play the clip
        audioSource.Play();

    }
}
