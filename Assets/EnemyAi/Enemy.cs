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

    [Header("Audio Settings")]
    public AudioClip hitSound;          // เสียงตอนโดนยิง/รับ Damage
    [Range(0f, 1f)] public float hitSoundVolume = 1f;
    
    public AudioClip footstepSound;     // เสียงตอนเดิน
    [Range(0f, 1f)] public float footstepVolume = 0.5f;

    private float currentHealth;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Transform playerTransform;
    private AudioSource audioSource;    // ใช้สำหรับเล่นเสียงเดินแบบวนลูป

    // ป้องกันการให้คะแนนซ้ำ
    private bool scoreAdded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // เพิ่มหรือดึง AudioSource สำหรับเสียงเดิน
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = true; // ตั้งให้วนลูปเสียงเดิน
    }

    private void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // ตั้งค่าเสียงเดินตั้งต้น
        if (footstepSound != null)
        {
            audioSource.clip = footstepSound;
            audioSource.volume = footstepVolume;
        }
    }

    private void FixedUpdate()
    {
        if (playerTransform != null && currentHealth > 0)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            if (rb != null)
            {
                rb.MovePosition(
                    rb.position +
                    direction *
                    moveSpeed *
                    Time.fixedDeltaTime
                );
            }

            if (direction.x != 0 && spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0;
            }

            // จัดการเสียงเดิน
            HandleFootstepSound(true);
        }
        else
        {
            // หยุดเสียงเดินเมื่อไม่มี Player หรือตายแล้ว
            HandleFootstepSound(false);
        }
    }

    // ควบคุมการเปิด-ปิด เสียงเดิน
    private void HandleFootstepSound(bool isMoving)
    {
        if (footstepSound == null) return;

        if (isMoving && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!isMoving && audioSource.isPlaying)
        {
            audioSource.Stop();
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
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

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

        // ================================
        // BOSS LOCK
        // ================================

        if (enemyType == EnemyType.Boss)
        {
            WaveSpawner spawner = FindObjectOfType<WaveSpawner>();

            if (spawner != null && !spawner.IsBossUnlocked())
            {
                Debug.Log("Boss ยังโจมตีไม่ได้! ต้องฆ่าศัตรูตัวอื่นให้หมดก่อน");
                return;
            }
        }

        // ================================
        // SilverOnly แพ้เฉพาะ Silver
        // ================================

        if (enemyType == EnemyType.SilverOnly && ammoType != AmmoType.MagicSilver)
        {
            Debug.Log("Silver Vampire เป็นอมตะต่อกระสุนธรรมดา!");
            return;
        }

        // ================================
        // ลดเลือด
        // ================================

        currentHealth -= damage;

        Debug.Log($"{enemyType} HP เหลือ: {currentHealth}/{maxHealth}");

        // เล่นเสียงเมื่อโดน Damage
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, hitSoundVolume);
        }

        // ================================
        // Hit Animation
        // ================================

        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }

        // ================================
        // ตาย
        // ================================

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

        // หยุดเสียงเดินทันทีเมื่อตาย
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // ================================
        // เพิ่ม Score
        // ================================

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(scoreValue);
            Debug.Log($"{enemyType} ถูกฆ่า! +{scoreValue} Score");
        }
        else
        {
            Debug.LogWarning("ไม่พบ ScoreManager ใน Scene!");
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

        scoreAdded = false;
    }
}