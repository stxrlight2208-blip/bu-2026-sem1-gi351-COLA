using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    [Header("Ammo Settings")]
    public AmmoType ammoType; // <--- เพิ่มตัวแปรระบุชนิดกระสุน (เลือกใน Inspector ได้เลย)
    public int ammoAmount = 15;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAimAndWeapon playerWeapon = other.GetComponent<PlayerAimAndWeapon>();

            if (playerWeapon != null)
            {
                // ส่งไปให้ครบทั้งประเภทกระสุน และ จำนวนกระสุน
                playerWeapon.AddAmmo(ammoType, ammoAmount);
                Destroy(gameObject);
            }
        }
    }
}