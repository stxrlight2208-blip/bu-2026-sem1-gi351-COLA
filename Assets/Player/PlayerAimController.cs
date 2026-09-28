using UnityEngine;
using TMPro; // ใช้สำหรับแสดงจำนวนกระสุนบน UI (TextMeshPro)

public class PlayerAimAndWeapon : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Aiming Setup")]
    public Transform handPivot;       // ลาก HandLeft_WeaponPivot มาใส่ที่นี่
    public SpriteRenderer bodySprite; // ลาก Body (SpriteRenderer) มาใส่ที่นี่

    [Header("Weapon Settings")]
    public Transform firePoint;     // ตำแหน่งปลายปืน
    public GameObject bulletPrefab; // Prefab ของกระสุน
    public float fireRate = 0.15f;   // ความเร็วในการยิง (วินาที/นัด)
    public float bulletForce = 20f;  // ความเร็วพุ่งของกระสุน
    public int damage = 10;          // ความเสียหาย

    [Header("Limited Ammo Settings")]
    public int maxAmmo = 30;         // จำนวนกระสุนทั้งหมดที่มี
    public TextMeshProUGUI ammoText; // (Optional) ลาก UI Text มาใส่เพื่อแสดงกระสุนบนหน้าจอ
    
    [HideInInspector]
    public int currentAmmo;         // จำนวนกระสุนปัจจุบันที่เหลืออยู่

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
            rb.freezeRotation = true; // ป้องกันตัวละครหมุนติ้วด้วยระบบ Physics
        }

        // กำหนดกระสุนเริ่มต้นเท่ากับกระสุนสูงสุด
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        SetHandSortingOrder();
    }

    void Update()
    {
        // 1. รับค่าการเดิน (WASD / Arrow Keys)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 2. รับตำแหน่งเมาส์
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // 3. ปุ่มยิงปืน (คลิกซ้ายค้าง)
        HandleShooting();
    }

    void FixedUpdate()
    {
        // คำนวณการเดิน
        rb.linearVelocity = moveInput.normalized * moveSpeed;

        // คำนวณการหมุนมือเล็งตามเมาส์
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
            bodySprite.flipX = true; // หันลำตัวไปทางซ้าย
            handPivot.localScale = new Vector3(1f, -1f, 1f); // พลิกปืนไม่ให้กลับหัว
        }
        else
        {
            bodySprite.flipX = false; // หันลำตัวไปทางขวา
            handPivot.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void HandleShooting()
    {
        // ยิงได้เฉพาะเมื่อกระสุนยังเหลือมากกว่า 0 (ถ้าหมดแล้วจะไม่ทำงาน)
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // ลดจำนวนกระสุนลงทีละ 1 นัด
        currentAmmo--;
        UpdateAmmoUI();

        // สร้างกระสุน
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(firePoint.right * bulletForce, ForceMode2D.Impulse);
        }

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = damage;
        }
    }

    void UpdateAmmoUI()
    {
        // อัปเดตตัวเลขกระสุนบนหน้าจอ (ถ้ามีการเชื่อมต่อ Text)
        if (ammoText != null)
        {
            ammoText.text = "AMMO: " + currentAmmo;
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
    // เพิ่มฟังก์ชันนี้ลงใน PlayerAimAndWeapon.cs
    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        UpdateAmmoUI(); // อัปเดต UI กระสุนถ้ามี
        Debug.Log("เก็บกระสุนเพิ่มได้: " + amount + " นัด | กระสุนปัจจุบัน: " + currentAmmo);
    }
}