using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Ammo Pickup Settings")]
    public AmmoType ammoType = AmmoType.Normal; // เลือกประเภทกระสุน (Normal / MagicSilver)
    public int ammoAmount = 15;                // จำนวนกระสุนที่จะเติม

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจจับเมื่อ Player เดินมาชน
        PlayerAimAndWeapon playerWeapon = collision.GetComponent<PlayerAimAndWeapon>();
        if (playerWeapon != null)
        {
            // 1. สลับประเภทกระสุนให้เป็นประเภทของกล่องที่เก็บ
            playerWeapon.currentAmmoType = ammoType;

            // 2. เติมจำนวนกระสุนเข้าตัว Player (ต้องใส่ทั้ง ammoType และ ammoAmount)
            playerWeapon.AddAmmo(ammoType, ammoAmount);

            Debug.Log($"เก็บกล่องกระสุน {ammoType} ได้เพิ่ม {ammoAmount} นัด!");

            // 3. ทำลายกล่องกระสุนออกจากด่าน
            Destroy(gameObject);
        }
    }
}