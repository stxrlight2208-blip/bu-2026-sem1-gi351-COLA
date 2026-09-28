using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlayClick()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}