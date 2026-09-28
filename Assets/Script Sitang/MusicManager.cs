using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void PlayGameMusic(AudioClip gameMusic)
    {
        if (gameMusic == null) return;

        audioSource.clip = gameMusic;
        audioSource.Play();
    }
}