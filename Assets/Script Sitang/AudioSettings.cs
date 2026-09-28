using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public Slider sfxSlider;
    public Slider musicSlider;

    public void SetSFXVolume()
    {
        AudioListener.volume = sfxSlider.value;
    }

    public void SetMusicVolume()
    {
        // ตอนนี้เตรียมไว้สำหรับเชื่อมเสียงเพลง
    }
}