using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public enum EnemyType { Normal, SilverOnly, Boss }

    [Header("Enemy Setup")]
    public EnemyType enemyType = EnemyType.Normal;
    public float maxHealth = 100f;
    public float moveSpeed = 2.5f;

    private float currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Transform playerTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        // อิงโค้ดเพื่อน: ค้นหา Player ด้วย Tag "Player"
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void FixedUpdate()
    {
        // ถ้าศัตรูยังไม่ตาย และเจอ Player ให้วิ่งเข้าหา
        if (playerTransform != null && currentHealth > 0)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

            // กลับหน้า Sprite ตามทิศทางเดิน
            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
    }

    public void TakeDamage(int damage, AmmoType ammoType)
    {
        if (currentHealth <= 0) return;

        // แวมไพร์กระสุนเงิน เป็นอมตะต่อกระสุนธรรมดา
        if (enemyType == EnemyType.SilverOnly && ammoType != AmmoType.MagicSilver)
        {
            Debug.Log("Silver Vampire เป็นอมตะต่อกระสุนธรรมดา!");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"{enemyType} HP: {currentHealth}/{maxHealth}");

        // สั่งเล่น Animation Hit ของ Asset
        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // ปิด Collider ไม่ให้ชนกับ Player หรือกระสุนเพิ่ม
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // สั่งเล่น Animation Dead ของ Asset
        if (anim != null)
        {
            anim.SetBool("Dead", true);
        }

        Destroy(gameObject, 0.5f); // ลบออกจากฉากหลังตาย 0.5 วินาที
    }
}