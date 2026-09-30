using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Ammo Pickup Settings")]
    public AmmoType ammoType = AmmoType.Normal;
    public int ammoAmount = 15;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerAimAndWeapon playerWeapon =
            collision.GetComponent<PlayerAimAndWeapon>();

        if (playerWeapon != null)
        {
            // เติมกระสุนอย่างเดียว
            // ไม่เปลี่ยนประเภทกระสุน
            playerWeapon.AddAmmo(
                ammoType,
                ammoAmount
            );

            Debug.Log(
                $"เก็บกล่องกระสุน {ammoType} " +
                $"ได้เพิ่ม {ammoAmount} นัด!"
            );

            // ทำลายกล่อง
            Destroy(gameObject);
        }
    }
}