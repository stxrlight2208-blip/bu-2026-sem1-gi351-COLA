using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public GameObject silverBulletPrefab; // Prefab กระสุนเงิน (เพิ่มบรรทัดนี้)
    public float fireRate = 0.15f;
    public float bulletForce = 20f;
    public int damage = 10;

    [Header("Ammo Type Settings")]
    public AmmoType currentAmmoType = AmmoType.Normal;

    [Header("Limited Ammo Settings")]
    public int maxAmmo = 30;
    public TextMeshProUGUI ammoText;
    public Slider ammoBar;

    [HideInInspector]
    public int currentAmmo;

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

        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        SetHandSortingOrder();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // กด Q เพื่อสลับกระสุนระหว่าง Normal และ MagicSilver
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentAmmoType = (currentAmmoType == AmmoType.Normal) ? AmmoType.MagicSilver : AmmoType.Normal;
            Debug.Log("สลับกระสุนเป็น: " + currentAmmoType);
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
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (firePoint == null) return;

        // เลือก Prefab ตามประเภทกระสุนปัจจุบัน (ถ้าเป็น MagicSilver ให้ใช้ silverBulletPrefab)
        GameObject prefabToSpawn = (currentAmmoType == AmmoType.MagicSilver && silverBulletPrefab != null)
            ? silverBulletPrefab
            : bulletPrefab;

        if (prefabToSpawn == null) return;

        currentAmmo--;
        UpdateAmmoUI();

        // สร้างกระสุนตาม Prefab ที่เลือกไว้
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
            bulletScript.ammoType = currentAmmoType; // ส่งประเภทกระสุนที่เลือกอยู่
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + "/" + maxAmmo;
        }

        if (ammoBar != null)
        {
            ammoBar.maxValue = maxAmmo;
            ammoBar.value = currentAmmo;
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

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        UpdateAmmoUI();
        Debug.Log("เก็บกระสุนเพิ่มได้: " + amount + " นัด | กระสุนปัจจุบัน: " + currentAmmo);
    }
}