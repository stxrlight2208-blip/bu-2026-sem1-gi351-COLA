using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("Movement Settings")]
    public float speed = 1.5f;
    public float chaseRange = 5f;
    private Transform playerTransform;

    [Header("Sprite Settings")]
    public Sprite hitSprite;     // ลากรูปผี (Hit) มาใส่
    public Sprite deathSprite;   // ลากรูปศพเขียว (Dead) มาใส่
    public float flashDuration = 0.15f;

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Collider2D zombieCollider;
    public Animator anim;        // ลาก Animator ของซอมบี้มาใส่เพื่อสั่งหยุดวิ่งชั่วคราวตอนโดนยิง

    private bool isDead = false;
    private Coroutine flashCoroutine;

    void Start()
    {
        currentHealth = maxHealth;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= chaseRange)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    playerTransform.position,
                    speed * Time.deltaTime
                );

                if (playerTransform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (playerTransform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("ซอมบี้โดนยิง! เลือดเหลือ: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashSprite());
        }
    }

    IEnumerator FlashSprite()
    {
        if (spriteRenderer == null || hitSprite == null) yield break;

        if (anim != null) anim.enabled = false;

        spriteRenderer.sprite = hitSprite;
        yield return new WaitForSeconds(flashDuration);

        if (!isDead)
        {
            if (anim != null) anim.enabled = true;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("ซอมบี้ตายแล้ว!");

        // บวกคะแนน 100 คะแนนเมื่อฆ่าซอมบี้ได้
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(100);
        }

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);

        if (anim != null) anim.enabled = false;

        if (spriteRenderer != null && deathSprite != null)
        {
            spriteRenderer.sprite = deathSprite;
        }

        if (zombieCollider != null)
        {
            zombieCollider.enabled = false;
        }

        Destroy(gameObject, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}