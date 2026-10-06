using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Ammo Pickup Settings")]
    public AmmoType ammoType = AmmoType.Normal;
    public int ammoAmount = 15;

    [Header("Audio Settings")]
    public AudioClip pickupSound;      // ไฟล์เสียงตอนเก็บกล่องกระสุน
    [Range(0f, 1f)]
    public float soundVolume = 1f;     // ความดังของเสียง (0.0 - 1.0)

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected)
            return;

        PlayerAimAndWeapon playerWeapon = collision.GetComponent<PlayerAimAndWeapon>();

        if (playerWeapon != null)
        {
            collected = true;

            // เติมกระสุน
            playerWeapon.AddAmmo(
                ammoType,
                ammoAmount
            );

            Debug.Log(
                $"เก็บกล่องกระสุน {ammoType} " +
                $"ได้เพิ่ม {ammoAmount} นัด!"
            );

            // ลดจำนวนกล่องบน UI
            if (AmmoBoxUI.instance != null)
            {
                AmmoBoxUI.instance.RemoveAmmoBox();
            }

            // เล่นเสียงเก็บไอเทม ณ ตำแหน่งของกล่องกระสุน
            PlayPickupSound();

            // ทำลายกล่อง
            Destroy(gameObject);
        }
    }

    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            // สร้าง AudioSource ชั่วคราวเล่นเสียง ณ จุดนั้น เพื่อไม่ให้เสียงถูกตัดขาดเมื่อ Destroy(gameObject)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
        }
    }
}