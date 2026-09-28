using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip gameMusic;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    void PlayMusicForScene(string sceneName)
    {
        if (sceneName == "Game")
        {
            PlayMusic(gameMusic);
        }
        else
        {
            PlayMusic(menuMusic);
        }
    }

    public void PlayMusic(AudioClip music)
    {
        if (music == null)
        {
            Debug.LogWarning("MusicManager: ไม่มีเพลงถูกใส่");
            return;
        }

        if (audioSource.clip == music && audioSource.isPlaying)
            return;

        audioSource.clip = music;
        audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}