using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float destroyTime = 2f;
    public int damage = 10;
    public AmmoType ammoType = AmmoType.Normal;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. ถ้าชนตัว Player เอง ให้มองผ่าน ไม่ต้องทำอะไร
        if (hitInfo.GetComponent<PlayerHealth>() != null) return;

        // 2. ถ้าชนวัตถุที่มี IDamageable (Enemy) ให้ส่ง Damage ทำความเสียหาย
        IDamageable damageable = hitInfo.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, ammoType);
        }

        // 3. ชนวัตถุอื่นๆ ทั้งหมด (ผี / กำแพง / สิ่งกีดขวาง) ให้ลบกระสุนทิ้งทันที
        Destroy(gameObject);
    }
}