using UnityEngine;

public class PlayerWeaponAudio : MonoBehaviour
{
    [Header("Script Reference")]
    public PlayerAimAndWeapon weaponScript; // สคริปต์ยิงตัวเดิม

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip shootSound;      // เสียงยิงปืน
    public AudioClip emptyAmmoSound;  // เสียงกระสุนหมด

    private float nextPlayTime = 0f;

    void Start()
    {
        // ถ้าไม่ได้ลากใส่ ให้ค้นหา AudioSource และ PlayerAimAndWeapon ในตัวให้อัตโนมัติ
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (weaponScript == null)
            weaponScript = GetComponent<PlayerAimAndWeapon>();
    }

    void Update()
    {
        if (weaponScript == null) return;

        // เช็กการกดปุ่มยิง (คลิกซ้าย)
        if (Input.GetButton("Fire1") && Time.time >= nextPlayTime)
        {
            nextPlayTime = Time.time + weaponScript.fireRate;

            // เช็กกระสุนปัจจุบันจากสคริปต์หลัก
            bool hasAmmo = CheckHasAmmo();

            if (hasAmmo)
            {
                // เล่นเสียงยิง
                if (audioSource != null && shootSound != null)
                {
                    audioSource.PlayOneShot(shootSound);
                }
            }
            else
            {
                // เล่นเสียงกระสุนหมด
                if (audioSource != null && emptyAmmoSound != null)
                {
                    audioSource.PlayOneShot(emptyAmmoSound);
                }
            }
        }
    }

    // ฟังก์ชันเช็กจำนวนกระสุนตามชนิดกระสุนปัจจุบัน
    private bool CheckHasAmmo()
    {
        if (weaponScript.currentAmmoType == AmmoType.Normal)
        {
            return weaponScript.normalAmmo > 0;
        }
        else if (weaponScript.currentAmmoType == AmmoType.MagicSilver)
        {
            return weaponScript.magicSilverAmmo > 0;
        }

        return false;
    }
}