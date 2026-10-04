using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public enum EnemyType
    {
        Normal,
        SilverOnly,
        Boss
    }

    [Header("Enemy Setup")]
    public EnemyType enemyType = EnemyType.Normal;
    public float maxHealth = 100f;
    public float moveSpeed = 2.5f;

    [Header("Score")]
    public int scoreValue = 300;

    [Header("Attack Setup")]
    public int attackDamage = 10;
    public float attackRate = 1f;
    private float nextAttackTime = 0f;

    private float currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Transform playerTransform;

    // ป้องกันการให้คะแนนซ้ำ
    private bool scoreAdded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void FixedUpdate()
    {
        if (playerTransform != null && currentHealth > 0)
        {
            Vector2 direction =
                (playerTransform.position - transform.position).normalized;

            rb.MovePosition(
                rb.position +
                direction * moveSpeed * Time.fixedDeltaTime
            );

            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
    }

    // ================================
    // Enemy โจมตี Player
    // ================================

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (currentHealth <= 0)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                PlayerHealth playerHealth =
                    collision.gameObject.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);

                    nextAttackTime = Time.time + attackRate;
                }
            }
        }
    }

    // ================================
    // Enemy รับ Damage
    // ================================

    public void TakeDamage(int damage, AmmoType ammoType)
    {
        if (currentHealth <= 0)
            return;

        // SilverOnly แพ้เฉพาะกระสุน Silver
        if (enemyType == EnemyType.SilverOnly &&
            ammoType != AmmoType.MagicSilver)
        {
            Debug.Log("Silver Vampire เป็นอมตะต่อกระสุนธรรมดา!");
            return;
        }

        currentHealth -= damage;

        Debug.Log(
            $"{enemyType} HP เหลือ: {currentHealth}/{maxHealth}"
        );

        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ================================
    // Enemy ตาย
    // ================================

    private void Die()
    {
        // ป้องกัน Die ถูกเรียกซ้ำ
        if (scoreAdded)
            return;

        scoreAdded = true;

        // ================================
        // เพิ่ม Score
        // ================================

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);

            Debug.Log(
                $"{enemyType} ถูกฆ่า! +{scoreValue} Score"
            );
        }
        else
        {
            Debug.LogWarning(
                "ไม่พบ ScoreManager ใน Scene!"
            );
        }

        // ================================
        // ปิด Collider
        // ================================

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = false;
        }

        // ================================
        // เล่น Animation ตาย
        // ================================

        if (anim != null)
        {
            anim.SetBool("Dead", true);
        }

        // ปิดการทำงานของ Enemy
        this.enabled = false;

        // ลบ Enemy หลังจาก 2 วินาที
        Destroy(gameObject, 2f);
    }

    // ================================
    // ตั้งเลือด
    // ================================

    public void SetHealth(float health)
    {
        maxHealth = health;
        currentHealth = health;
    }
}
