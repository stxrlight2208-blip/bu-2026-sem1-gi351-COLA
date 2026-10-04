using UnityEngine;

public class PlayerHurtAudio : MonoBehaviour
{
    [Header("Script Reference")]
    public PlayerHealth playerHealth; // อ้างอิงสคริปต์ PlayerHealth

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip hurtSound;       // ไฟล์เสียงเมื่อโดนโจมตี / เลือดลด

    private int lastHealth;

    void Start()
    {
        // ค้นหา Component อัตโนมัติถ้าไม่ได้ลากใส่ใน Inspector
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        // จำค่าเลือดเริ่มต้นไว้
        if (playerHealth != null)
        {
            lastHealth = playerHealth.currentHealth;
        }
    }

    void Update()
    {
        if (playerHealth == null) return;

        // เช็กว่าเลือดปัจจุบันน้อยกว่าเลือดครั้งล่าสุดหรือไม่ (แปลว่าเลือดลด/โดนโจมตี)
        if (playerHealth.currentHealth < lastHealth)
        {
            PlayHurtSound();
        }

        // อัปเดตค่าเลือดล่าสุดเพื่อใช้เช็กในเฟรมถัดไป
        lastHealth = playerHealth.currentHealth;
    }

    void PlayHurtSound()
    {
        if (audioSource != null && hurtSound != null)
        {
            // สุ่ม Pitch เล็กน้อยเพื่อให้เสียงมีความหลากหลาย
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(hurtSound);
        }
    }
}