using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("UI Reference")]
    public Slider volumeSlider;

    private const string VOLUME_KEY = "GlobalVolumeKey";

    void Start()
    {
        // โหลดค่าระดับเสียงที่เคยเซฟไว้ในเครื่อง (ถ้าเพิ่งเล่นครั้งแรก ให้เริ่มที่ 1.0 คือดังสุด)
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);

        // ตั้งค่าความดังของเกมตามค่าที่โหลดมา
        AudioListener.volume = savedVolume;

        // ถ้ามีการเชื่อมต่อ Slider ใน Inspector ให้ตั้งค่า Slider ให้ตรงกับเสียงปัจจุบัน
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;

            // ผูกฟังก์ชันเข้ากับ Slider เพื่อให้ทำงานทันทีเมื่อลากปรับระดับ
            volumeSlider.onValueChanged.AddListener(SetGlobalVolume);
        }
    }

    // ฟังก์ชันสำหรับปรับระดับเสียงทั้งหมดในเกม
    public void SetGlobalVolume(float volume)
    {
        // ปรับระดับเสียง Master ของทั้งเกม (0.0 ถึง 1.0)
        AudioListener.volume = volume;

        // บันทึกค่าไว้ในเครื่อง เพื่อเปิดเกมมาใหม่จะได้จำค่าเดิมได้
        PlayerPrefs.SetFloat(VOLUME_KEY, volume);
    }
}