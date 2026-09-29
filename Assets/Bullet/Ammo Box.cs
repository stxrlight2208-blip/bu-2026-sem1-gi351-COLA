using UnityEngine; // <<-- ตรวจสอบว่ามีบรรทัดนี้อยู่บนสุดของไฟล์แล้วหรือยัง

public class AmmoBox : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int ammoAmount = 15;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ดึงสคริปต์ตัวละคร (เช็คชื่อสคริปต์ให้ตรงกับที่คุณใช้จริง เช่น PlayerAimAndWeapon หรือ PlayerAimController)
            PlayerAimAndWeapon playerWeapon = other.GetComponent<PlayerAimAndWeapon>();

            if (playerWeapon != null)
            {
                playerWeapon.AddAmmo(ammoAmount);
                Destroy(gameObject);
            }
        }
    }
}