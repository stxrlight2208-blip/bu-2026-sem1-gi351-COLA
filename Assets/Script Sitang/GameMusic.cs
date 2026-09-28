using UnityEngine;

public class GameMusic : MonoBehaviour
{
    public AudioClip gameMusic;

    void Start()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMusic(gameMusic);
        }
    }
}