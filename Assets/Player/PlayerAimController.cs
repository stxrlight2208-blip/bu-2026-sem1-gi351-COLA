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

    public GameObject bulletPrefab;

    public GameObject silverBulletPrefab;

    public float fireRate = 0.15f;

    public float bulletForce = 20f;

    public int damage = 10;



    [Header("Ammo Type Settings")]

    public AmmoType currentAmmoType = AmmoType.Normal;



    [Header("Ammo Icon UI")]

    public Image ammoIconUI;

    public SpriteRenderer ammoSource;



    public Color normalAmmoColor = Color.white;

    public Color silverAmmoColor = Color.gray;



    [Header("Ammo")]

    public int normalAmmo = 30;

    public int magicSilverAmmo = 0;



    public TextMeshProUGUI ammoText;



    [Header("Ammo UI")]

    public AmmoUI ammoUI;



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

        UpdateAmmoIcon();

        SetHandSortingOrder();

    }



    void Update()

    {

        // ==============================

        // MOVEMENT INPUT

        // ==============================



        moveInput.x = Input.GetAxisRaw("Horizontal");

        moveInput.y = Input.GetAxisRaw("Vertical");



        // ==============================

        // MOUSE POSITION

        // ==============================



        if (mainCam != null)

        {

            mousePos =

                mainCam.ScreenToWorldPoint(

                    Input.mousePosition

                );

        }



        // ==============================

        // Q = SWITCH AMMO

        // ==============================



        if (Input.GetKeyDown(KeyCode.Q))

        {

            SwitchAmmoType();

        }



        // ==============================

        // SHOOT

        // ==============================



        HandleShooting();

    }



    void FixedUpdate()

    {

        // ==============================

        // MOVEMENT

        // ==============================



        if (rb != null)

        {

            rb.linearVelocity =

                moveInput.normalized * moveSpeed;

        }



        // ==============================

        // AIM

        // ==============================



        HandleAiming();

    }



    // =========================================

    // SWITCH AMMO

    // =========================================



    void SwitchAmmoType()

    {

        if (currentAmmoType == AmmoType.Normal)

        {

            currentAmmoType =

                AmmoType.MagicSilver;

        }

        else

        {

            currentAmmoType =

                AmmoType.Normal;

        }



        UpdateAmmoUI();

        UpdateAmmoIcon();



        Debug.Log(

            "เปลี่ยนกระสุนเป็น: " +

            currentAmmoType

        );

    }



    // =========================================

    // AIM

    // =========================================



    void HandleAiming()

    {

        if (handPivot == null ||

            bodySprite == null)

        {

            return;

        }



        Vector2 aimDir =

            mousePos -

            (Vector2)handPivot.position;



        float angle =

            Mathf.Atan2(

                aimDir.y,

                aimDir.x

            ) * Mathf.Rad2Deg;



        handPivot.rotation =

            Quaternion.Euler(

                0f,

                0f,

                angle

            );



        if (aimDir.x < 0)

        {

            bodySprite.flipX = true;



            handPivot.localScale =

                new Vector3(

                    -1f,

                    -1f,

                    1f

                );

        }

        else

        {

            bodySprite.flipX = false;



            handPivot.localScale =

                new Vector3(

                    1f,

                    1f,

                    1f

                );

        }

    }



    // =========================================

    // SHOOTING

    // =========================================



    void HandleShooting()

    {

        bool hasAmmo = false;



        if (currentAmmoType == AmmoType.Normal)

        {

            hasAmmo = normalAmmo > 0;

        }

        else if (currentAmmoType == AmmoType.MagicSilver)

        {

            hasAmmo = magicSilverAmmo > 0;

        }



        if (Input.GetButton("Fire1") &&

            Time.time >= nextFireTime &&

            hasAmmo)

        {

            nextFireTime =

                Time.time + fireRate;



            Shoot();

        }

    }



    // =========================================

    // SHOOT

    // =========================================



    void Shoot()

    {

        if (firePoint == null)

        {

            return;

        }



        // เก็บชนิดกระสุนที่ยิงจริง

        AmmoType firedAmmoType =

            currentAmmoType;



        // =========================================

        // เลือกกระสุน

        // =========================================



        GameObject prefabToSpawn = null;



        if (firedAmmoType == AmmoType.Normal)

        {

            prefabToSpawn =

                bulletPrefab;

        }

        else if (firedAmmoType == AmmoType.MagicSilver)

        {

            prefabToSpawn =

                silverBulletPrefab;

        }



        if (prefabToSpawn == null)

        {

            Debug.LogWarning(

                "ยังไม่ได้ใส่ Bullet Prefab!"

            );



            return;

        }



        // =========================================

        // ลดกระสุน

        // =========================================



        if (firedAmmoType == AmmoType.Normal)

        {

            normalAmmo--;



            if (normalAmmo < 0)

            {

                normalAmmo = 0;

            }

        }

        else if (firedAmmoType == AmmoType.MagicSilver)

        {

            magicSilverAmmo--;



            if (magicSilverAmmo < 0)

            {

                magicSilverAmmo = 0;

            }



            // ไม่เปลี่ยนกลับ Normal อัตโนมัติ

        }



        // =========================================

        // UPDATE UI

        // =========================================



        UpdateAmmoUI();



        // =========================================

        // CREATE BULLET

        // =========================================



        GameObject bulletObj =

            Instantiate(

                prefabToSpawn,

                firePoint.position,

                firePoint.rotation

            );



        // =========================================

        // BULLET FORCE

        // =========================================



        Rigidbody2D bulletRb =

            bulletObj.GetComponent<Rigidbody2D>();



        if (bulletRb != null)

        {

            bulletRb.AddForce(

                firePoint.right *

                bulletForce,

                ForceMode2D.Impulse

            );

        }



        // =========================================

        // SEND AMMO TYPE TO BULLET

        // =========================================



        Bullet bulletScript =

            bulletObj.GetComponent<Bullet>();



        if (bulletScript != null)

        {

            bulletScript.damage =

                damage;



            bulletScript.ammoType =

                firedAmmoType;

        }

    }



    // =========================================

    // AMMO ICON

    // =========================================



    void UpdateAmmoIcon()

    {

        if (ammoIconUI == null ||

            ammoSource == null)

        {

            return;

        }



        ammoIconUI.sprite =

            ammoSource.sprite;



        if (currentAmmoType ==

            AmmoType.Normal)

        {

            ammoIconUI.color =

                normalAmmoColor;

        }

        else

        {

            ammoIconUI.color =

                silverAmmoColor;

        }

    }



    // =========================================

    // AMMO UI

    // =========================================



    void UpdateAmmoUI()

    {

        if (ammoText != null)

        {

            ammoText.text =

                $"Normal: {normalAmmo} | Silver: {magicSilverAmmo} [{currentAmmoType}]";

        }



        if (ammoUI != null)

        {

            ammoUI.UpdateAmmo(

                normalAmmo,

                magicSilverAmmo

            );

        }

    }



    // =========================================

    // HAND SORTING

    // =========================================



    void SetHandSortingOrder()

    {

        if (handPivot == null ||

            bodySprite == null)

        {

            return;

        }



        int targetOrder =

            bodySprite.sortingOrder + 1;



        SpriteRenderer[] handSprites =

            handPivot

            .GetComponentsInChildren<SpriteRenderer>();



        foreach (SpriteRenderer sr in handSprites)

        {

            sr.sortingLayerID =

                bodySprite.sortingLayerID;



            sr.sortingOrder =

                targetOrder;

        }

    }



    // =========================================

    // ADD AMMO

    // =========================================



    public void AddAmmo(

        AmmoType type,

        int amount

    )

    {

        if (type == AmmoType.Normal)

        {

            normalAmmo += amount;

        }

        else if (type == AmmoType.MagicSilver)

        {

            magicSilverAmmo += amount;

        }



        // สำคัญ:

        // ไม่มีการเปลี่ยน currentAmmoType ตรงนี้

        // เก็บกระสุนแล้วจะใช้กระสุนเดิมต่อ



        UpdateAmmoUI();

        UpdateAmmoIcon();



        Debug.Log(

            "เก็บกระสุน " +

            type +

            " +" +

            amount +

            " | ตอนนี้ใช้ " +

            currentAmmoType

        );

    }

}