using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public Slider sfxSlider;
    public Slider musicSlider;

    void Start()
    {
        // โหลดค่าที่เคยตั้งไว้
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMusicVolume();
        SetSFXVolume();
    }

    public void SetSFXVolume()
    {
        AudioListener.volume = sfxSlider.value;
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(musicSlider.value);
        }

        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();
    }
}