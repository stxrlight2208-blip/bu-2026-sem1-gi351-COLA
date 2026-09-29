using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public enum EnemyType { Normal, SilverOnly, Boss }

    [Header("Enemy Setup")]
    public EnemyType enemyType = EnemyType.Normal;
    public float maxHealth = 100f;
    public float moveSpeed = 2.5f;

    [Header("Attack Setup")]
    public int attackDamage = 10;
    public float attackRate = 1f; // โจมตีทุก 1 วินาที
    private float nextAttackTime = 0f;

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
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
    }

    // เมื่อเดินชน Player ให้เรียกฟังก์ชัน TakeDamage ของ PlayerHealth.cs
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (currentHealth <= 0) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    nextAttackTime = Time.time + attackRate;
                }
            }
        }
    }

    // รับความเสียหายจากกระสุน
    public void TakeDamage(int damage, AmmoType ammoType)
    {
        if (currentHealth <= 0) return;

        // SilverOnly อมตะต่อกระสุนธรรมดา
        if (enemyType == EnemyType.SilverOnly && ammoType != AmmoType.MagicSilver)
        {
            Debug.Log("Silver Vampire เป็นอมตะต่อกระสุนธรรมดา!");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"{enemyType} HP เหลือ: {currentHealth}/{maxHealth}");

        if (anim != null) anim.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (anim != null) anim.SetBool("Dead", true);

        // ปิดสคริปต์ศัตรูตัวนี้ เพื่อให้ WaveSpawner รู้ว่าตัวนี้ตายแล้ว ไม่นับรวมในฉากอีก
        this.enabled = false;

        Destroy(gameObject, 2f); // (ปรับเวลาให้อนิเมชันเล่นจบตามต้องการ)
    }
}