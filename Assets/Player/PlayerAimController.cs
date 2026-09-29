using UnityEngine;
using TMPro;

public class PlayerAimAndWeapon : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Aiming Setup")]
    public Transform handPivot;
    public SpriteRenderer bodySprite;

    [Header("Weapon Settings")]
    public Transform firePoint;
    public GameObject bulletPrefab;       // Prefab กระสุนธรรมดา
    public GameObject silverBulletPrefab; // Prefab กระสุนเงิน
    public float fireRate = 0.15f;
    public float bulletForce = 20f;
    public int damage = 10;

    [Header("Ammo Type Settings")]
    public AmmoType currentAmmoType = AmmoType.Normal;

    [Header("Limited Ammo Settings")]
    public int normalAmmo = 30;       // จำนวนกระสุนธรรมดา
    public int magicSilverAmmo = 0;   // จำนวนกระสุนเงินวิเศษ
    public TextMeshProUGUI ammoText;

    private Rigidbody2D rb;
    private Camera mainCam;
    private Vector2 moveInput;
    private Vector2 mousePos;
    private float nextFireTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;

        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        UpdateAmmoUI();
        SetHandSortingOrder();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // กด Q เพื่อสลับประเภทกระสุน
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentAmmoType = (currentAmmoType == AmmoType.Normal) ? AmmoType.MagicSilver : AmmoType.Normal;
            UpdateAmmoUI();
        }

        HandleShooting();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
        HandleAiming();
    }

    void HandleAiming()
    {
        if (handPivot == null || bodySprite == null) return;

        Vector2 aimDir = mousePos - (Vector2)handPivot.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        handPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        if (aimDir.x < 0)
        {
            bodySprite.flipX = true;
            handPivot.localScale = new Vector3(1f, -1f, 1f);
        }
        else
        {
            bodySprite.flipX = false;
            handPivot.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void HandleShooting()
    {
        // เช็กจำนวนกระสุนตามประเภทที่เลือกอยู่
        bool hasAmmo = (currentAmmoType == AmmoType.Normal && normalAmmo > 0) ||
                       (currentAmmoType == AmmoType.MagicSilver && magicSilverAmmo > 0);

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && hasAmmo)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (firePoint == null) return;

        GameObject prefabToSpawn = (currentAmmoType == AmmoType.MagicSilver && silverBulletPrefab != null)
            ? silverBulletPrefab
            : bulletPrefab;

        if (prefabToSpawn == null) return;

        // หักกระสุนตามประเภทที่ใช้อยู่
        if (currentAmmoType == AmmoType.Normal)
        {
            normalAmmo--;
        }
        else if (currentAmmoType == AmmoType.MagicSilver)
        {
            magicSilverAmmo--;

            // ถ้ากระสุนเงินหมด ให้สลับกลับเป็นกระสุนธรรมดาให้อัตโนมัติ
            if (magicSilverAmmo <= 0)
            {
                currentAmmoType = AmmoType.Normal;
            }
        }

        UpdateAmmoUI();

        GameObject bulletObj = Instantiate(prefabToSpawn, firePoint.position, firePoint.rotation);

        Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(firePoint.right * bulletForce, ForceMode2D.Impulse);
        }

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = damage;
            bulletScript.ammoType = currentAmmoType;
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            // แสดงทั้งสองแบบแยกกันชัดเจน
            ammoText.text = $"Normal: {normalAmmo} | Silver: {magicSilverAmmo} [{currentAmmoType}]";
        }
    }

    void SetHandSortingOrder()
    {
        if (handPivot != null && bodySprite != null)
        {
            int targetOrder = bodySprite.sortingOrder + 1;
            SpriteRenderer[] handSprites = handPivot.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sr in handSprites)
            {
                sr.sortingLayerID = bodySprite.sortingLayerID;
                sr.sortingOrder = targetOrder;
            }
        }
    }

    // ฟังก์ชันรับกระสุนแยกตามประเภท
    public void AddAmmo(AmmoType type, int amount)
    {
        if (type == AmmoType.Normal)
        {
            normalAmmo += amount;
        }
        else if (type == AmmoType.MagicSilver)
        {
            magicSilverAmmo += amount;
            currentAmmoType = AmmoType.MagicSilver; // สลับมาใช้กระสุนเงินทันทีเมื่อเก็บได้
        }

        UpdateAmmoUI();
    }
}