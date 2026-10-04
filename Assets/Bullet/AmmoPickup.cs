using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Ammo Pickup Settings")]
    public AmmoType ammoType = AmmoType.Normal;

    public int ammoAmount = 15;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected)
            return;

        PlayerAimAndWeapon playerWeapon =
            collision.GetComponent<PlayerAimAndWeapon>();

        if (playerWeapon != null)
        {
            collected = true;

            // เติมกระสุน
            // ไม่เปลี่ยนประเภทกระสุน
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

            // ทำลายกล่อง
            Destroy(gameObject);
        }
    }
}