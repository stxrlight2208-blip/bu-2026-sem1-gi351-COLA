using System.Collections;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public string name = "Enemy";
        public GameObject enemyPrefab;

        [Range(1, 100)]
        public int spawnWeight = 10;

        public int minWaveToAppear = 1;
    }


    // =========================================================
    // PLAYER
    // =========================================================

    [Header("Player")]

    [Tooltip("ลาก Player มาใส่")]
    public Transform player;


    // =========================================================
    // ENEMY SETTINGS
    // =========================================================

    [Header("Enemy Settings")]

    public EnemySpawnInfo[] enemyTypes;


    // =========================================================
    // BOSS SETTINGS
    // =========================================================

    [Header("Boss Settings")]

    [Tooltip("ลาก Boss Prefab มาใส่")]
    public GameObject bossPrefab;

    [Tooltip("Boss เริ่มเกิดตั้งแต่ Wave นี้")]
    public int bossStartWave = 5;


    // =========================================================
    // SPAWN DISTANCE
    // =========================================================

    [Header("Spawn Distance Around Player")]

    [Tooltip("ระยะใกล้สุดจาก Player ที่จะเกิด")]
    public float minSpawnDistance = 6f;

    [Tooltip("ระยะไกลสุดจาก Player ที่จะเกิด")]
    public float maxSpawnDistance = 10f;

    [Tooltip("ลองหาตำแหน่งใหม่กี่ครั้ง")]
    public int spawnPositionAttempts = 50;


    // =========================================================
    // AMMO SETTINGS
    // =========================================================

    [Header("Ammo Spawner Settings")]

    public GameObject[] ammoPrefabs;

    [Tooltip("จำนวนกล่องกระสุนพื้นฐานต่อ Wave")]
    public int ammoBoxesPerWave = 8;


    // =========================================================
    // OBSTACLE SETTINGS
    // =========================================================

    [Header("Obstacle Settings")]

    [Tooltip("Layer ของกำแพง/สิ่งกีดขวาง")]
    public LayerMask obstacleLayer;

    [Tooltip("รัศมีตรวจตำแหน่ง")]
    public float spawnCheckRadius = 0.3f;


    // =========================================================
    // WAVE SETTINGS
    // =========================================================

    [Header("Wave Settings")]

    public float timeBetweenWaves = 3f;

    public float spawnDelay = 1f;

    public int baseZombieCount = 3;

    public int extraZombiesPerWave = 2;


    // =========================================================
    // UI SETTINGS
    // =========================================================

    [Header("UI Settings")]

    public TextMeshProUGUI waveText;

    public TextMeshProUGUI zombieCountText;


    // =========================================================
    // PRIVATE VARIABLES
    // =========================================================

    private int currentWave = 0;

    private bool isSpawning = false;

    // Boss จะยังโจมตีไม่ได้จนกว่า Enemy ตัวอื่นจะหมด
    private bool bossUnlocked = false;


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        StartCoroutine(StartNextWave());
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        int remainingEnemies =
            GetActiveEnemyCount();


        // =====================================================
        // UPDATE ZOMBIE COUNT UI
        // =====================================================

        if (zombieCountText != null)
        {
            zombieCountText.text =
                "Zombies Left: " +
                remainingEnemies;
        }


        // =====================================================
        // BOSS UNLOCK CHECK
        // =====================================================

        if (IsBossWave() &&
            !bossUnlocked &&
            !isSpawning)
        {
            int normalEnemies =
                GetNormalEnemyCount();


            // ถ้า Enemy ที่ไม่ใช่ Boss ตายหมด
            if (normalEnemies <= 0)
            {
                bossUnlocked = true;

                Debug.Log(
                    "================================"
                );

                Debug.Log(
                    "BOSS UNLOCKED!"
                );

                Debug.Log(
                    "ศัตรูตัวอื่นตายหมดแล้ว"
                );

                Debug.Log(
                    "สามารถโจมตี Boss ได้แล้ว!"
                );

                Debug.Log(
                    "================================"
                );
            }
        }


        // =====================================================
        // WAIT FOR ENEMIES
        // =====================================================

        if (isSpawning ||
            remainingEnemies > 0)
        {
            return;
        }


        // =====================================================
        // START NEXT WAVE
        // =====================================================

        StartCoroutine(StartNextWave());
    }


    // =========================================================
    // CHECK BOSS WAVE
    // =========================================================

    bool IsBossWave()
    {
        return currentWave >= bossStartWave &&
               currentWave % 5 == 0;
    }


    // =========================================================
    // GET CURRENT WAVE
    // =========================================================

    public int GetCurrentWave()
    {
        return currentWave;
    }


    // =========================================================
    // CHECK BOSS UNLOCKED
    // =========================================================

    public bool IsBossUnlocked()
    {
        return bossUnlocked;
    }


    // =========================================================
    // COUNT ALL ACTIVE ENEMIES
    // =========================================================

    int GetActiveEnemyCount()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");


        int count = 0;


        foreach (GameObject enemyObj in enemies)
        {
            if (enemyObj == null ||
                !enemyObj.activeInHierarchy)
            {
                continue;
            }


            Enemy enemyScript =
                enemyObj.GetComponent<Enemy>();


            if (enemyScript != null &&
                enemyScript.enabled)
            {
                count++;
            }
        }


        return count;
    }


    // =========================================================
    // COUNT NORMAL ENEMIES
    // =========================================================

    int GetNormalEnemyCount()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");


        int count = 0;


        foreach (GameObject enemyObj in enemies)
        {
            if (enemyObj == null ||
                !enemyObj.activeInHierarchy)
            {
                continue;
            }


            Enemy enemy =
                enemyObj.GetComponent<Enemy>();


            if (enemy == null ||
                !enemy.enabled)
            {
                continue;
            }


            // นับเฉพาะ Enemy ที่ไม่ใช่ Boss
            if (enemy.enemyType !=
                Enemy.EnemyType.Boss)
            {
                count++;
            }
        }


        return count;
    }


    // =========================================================
    // START NEXT WAVE
    // =========================================================

    IEnumerator StartNextWave()
    {
        isSpawning = true;


        // =====================================================
        // WAVE CLEAR
        // =====================================================

        if (waveText != null &&
            currentWave > 0)
        {
            waveText.text =
                "Wave Cleared! Get Ready...";


            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(
                    currentWave * 500
                );
            }
        }


        // =====================================================
        // WAIT
        // =====================================================

        yield return new WaitForSeconds(
            timeBetweenWaves
        );


        // =====================================================
        // NEXT WAVE
        // =====================================================

        currentWave++;


        // ล็อก Boss ใหม่ทุก Wave
        bossUnlocked = false;


        // =====================================================
        // WAVE UI
        // =====================================================

        if (waveText != null)
        {
            waveText.text =
                "WAVE " +
                currentWave;
        }


        // =====================================================
        // AMMO
        // =====================================================

        SpawnAmmoBoxes();


        // =====================================================
        // BOSS
        // =====================================================

        if (IsBossWave())
        {
            SpawnBoss();
        }


        // =====================================================
        // ZOMBIE COUNT
        // =====================================================

        int zombiesToSpawn =
            baseZombieCount +
            (currentWave - 1) *
            extraZombiesPerWave;


        // =====================================================
        // SPAWN ZOMBIES
        // =====================================================

        for (int i = 0;
             i < zombiesToSpawn;
             i++)
        {
            SpawnZombie();


            yield return new WaitForSeconds(
                spawnDelay
            );
        }


        // =====================================================
        // FINISHED SPAWNING
        // =====================================================

        isSpawning = false;
    }


    // =========================================================
    // FIND RANDOM POSITION
    // =========================================================

    bool FindRandomSpawnPosition(
        out Vector2 position)
    {
        position = Vector2.zero;


        if (player == null)
        {
            Debug.LogWarning(
                "ยังไม่ได้กำหนด Player!"
            );

            return false;
        }


        for (int i = 0;
             i < spawnPositionAttempts;
             i++)
        {
            float angle =
                Random.Range(
                    0f,
                    360f
                );


            float distance =
                Random.Range(
                    minSpawnDistance,
                    maxSpawnDistance
                );


            Vector2 direction =
                new Vector2(
                    Mathf.Cos(
                        angle *
                        Mathf.Deg2Rad
                    ),
                    Mathf.Sin(
                        angle *
                        Mathf.Deg2Rad
                    )
                );


            Vector2 randomPosition =
                (Vector2)player.position +
                direction *
                distance;


            Collider2D hit =
                Physics2D.OverlapCircle(
                    randomPosition,
                    spawnCheckRadius,
                    obstacleLayer
                );


            if (hit == null)
            {
                position =
                    randomPosition;

                return true;
            }
        }


        return false;
    }


    // =========================================================
    // SPAWN BOSS
    // =========================================================

    void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning(
                "ยังไม่ได้ใส่ Boss Prefab!"
            );

            return;
        }


        Vector2 spawnPosition;


        bool foundPosition =
            FindRandomSpawnPosition(
                out spawnPosition
            );


        if (!foundPosition)
        {
            Debug.LogWarning(
                "หา Spawn Position ของ Boss ไม่ได้!"
            );

            return;
        }


        GameObject boss =
            Instantiate(
                bossPrefab,
                spawnPosition,
                Quaternion.identity
            );


        // ให้ระบบนับ Boss เป็น Enemy
        boss.tag = "Enemy";


        // =====================================================
        // SET BOSS TYPE
        // =====================================================

        Enemy bossEnemy =
            boss.GetComponent<Enemy>();


        if (bossEnemy != null)
        {
            // สำคัญ: กำหนดให้เป็น Boss
            bossEnemy.enemyType =
                Enemy.EnemyType.Boss;


            bossEnemy.SetHealth(
                bossEnemy.maxHealth
            );


            if (BossUI.instance != null)
            {
                BossUI.instance.ShowBoss(bossEnemy);
            }
        }


        Debug.Log(
            "================================"
        );

        Debug.Log(
            "BOSS SPAWNED!"
        );

        Debug.Log(
            "BOSS IS LOCKED!"
        );

        Debug.Log(
            "ต้องฆ่า Enemy ตัวอื่นก่อน"
        );

        Debug.Log(
            "Wave: " +
            currentWave
        );

        Debug.Log(
            "Distance From Player: " +
            Vector2.Distance(
                player.position,
                spawnPosition
            )
        );

        Debug.Log(
            "Position: " +
            spawnPosition
        );

        Debug.Log(
            "================================"
        );
    }


    // =========================================================
    // SPAWN AMMO
    // =========================================================

    void SpawnAmmoBoxes()
    {
        if (ammoPrefabs == null ||
            ammoPrefabs.Length == 0)
        {
            Debug.LogWarning(
                "ยังไม่ได้ใส่ Ammo Prefab!"
            );

            return;
        }


        int ammoBoxCount;


        if (currentWave <= 3)
        {
            ammoBoxCount = 8;
        }
        else if (currentWave <= 5)
        {
            ammoBoxCount = 9;
        }
        else if (currentWave <= 7)
        {
            ammoBoxCount = 10;
        }
        else if (currentWave <= 9)
        {
            ammoBoxCount = 12;
        }
        else if (currentWave == 10)
        {
            ammoBoxCount = 14;
        }
        else if (currentWave <= 12)
        {
            ammoBoxCount = 13;
        }
        else if (currentWave <= 14)
        {
            ammoBoxCount = 14;
        }
        else if (currentWave == 15)
        {
            ammoBoxCount = 16;
        }
        else if (currentWave <= 17)
        {
            ammoBoxCount = 15;
        }
        else if (currentWave <= 19)
        {
            ammoBoxCount = 16;
        }
        else
        {
            ammoBoxCount = 18;
        }


        int spawnedAmmoBoxes = 0;


        for (int i = 0;
             i < ammoBoxCount;
             i++)
        {
            Vector2 spawnPosition;


            bool foundPosition =
                FindRandomSpawnPosition(
                    out spawnPosition
                );


            if (!foundPosition)
            {
                Debug.LogWarning(
                    "หา Spawn Position ของ Ammo ไม่ได้!"
                );

                continue;
            }


            GameObject selectedAmmo =
                ammoPrefabs[
                    Random.Range(
                        0,
                        ammoPrefabs.Length
                    )
                ];


            Instantiate(
                selectedAmmo,
                spawnPosition,
                Quaternion.identity
            );


            spawnedAmmoBoxes++;


            // =================================================
            // AMMO BOX UI
            // =================================================

            if (AmmoBoxUI.instance != null)
            {
                AmmoBoxUI.instance.AddAmmoBox();
            }


            Debug.Log(
                "Spawn Ammo: " +
                selectedAmmo.name +
                " | Position: " +
                spawnPosition
            );
        }


        Debug.Log(
            $"Wave {currentWave}: " +
            $"สร้างกล่องกระสุน " +
            $"{spawnedAmmoBoxes} กล่อง"
        );
    }


    // =========================================================
    // SPAWN ZOMBIE
    // =========================================================

    void SpawnZombie()
    {
        GameObject selectedPrefab =
            GetRandomEnemyPrefab();


        if (selectedPrefab == null)
        {
            Debug.LogWarning(
                "หา Enemy Prefab ไม่เจอ!"
            );

            return;
        }


        Vector2 spawnPosition;


        bool foundPosition =
            FindRandomSpawnPosition(
                out spawnPosition
            );


        if (!foundPosition)
        {
            Debug.LogWarning(
                "หา Spawn Position ของ Zombie ไม่ได้!"
            );

            return;
        }


        GameObject newEnemy =
            Instantiate(
                selectedPrefab,
                spawnPosition,
                Quaternion.identity
            );


        newEnemy.tag = "Enemy";


        Enemy enemy =
            newEnemy.GetComponent<Enemy>();


        if (enemy != null)
        {
            enemy.SetHealth(
                enemy.maxHealth
            );


            Debug.Log(
                "Spawn: " +
                selectedPrefab.name +
                " | Type = " +
                enemy.enemyType +
                " | HP = " +
                enemy.maxHealth
            );
        }
    }


    // =========================================================
    // RANDOM ENEMY
    // =========================================================

    GameObject GetRandomEnemyPrefab()
    {
        if (enemyTypes == null ||
            enemyTypes.Length == 0)
        {
            return null;
        }


        int totalWeight = 0;


        foreach (var enemy in enemyTypes)
        {
            if (enemy.enemyPrefab != null &&
                currentWave >=
                enemy.minWaveToAppear)
            {
                totalWeight +=
                    enemy.spawnWeight;
            }
        }


        // =====================================================
        // FALLBACK
        // =====================================================

        if (totalWeight <= 0)
        {
            foreach (var enemy in enemyTypes)
            {
                if (enemy.enemyPrefab != null)
                {
                    return enemy.enemyPrefab;
                }
            }


            return null;
        }


        // =====================================================
        // RANDOM WEIGHT
        // =====================================================

        int randomValue =
            Random.Range(
                0,
                totalWeight
            );


        int currentSum = 0;


        foreach (var enemy in enemyTypes)
        {
            if (enemy.enemyPrefab != null &&
                currentWave >=
                enemy.minWaveToAppear)
            {
                currentSum +=
                    enemy.spawnWeight;


                if (randomValue <
                    currentSum)
                {
                    return enemy.enemyPrefab;
                }
            }
        }


        return enemyTypes[0].enemyPrefab;
    }
}