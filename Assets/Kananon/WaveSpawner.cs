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

    [Tooltip("จำนวนกล่องกระสุนต่อ Wave")]
    public int ammoBoxesPerWave = 3;


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


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        // ถ้ายังไม่ได้ลาก Player มา
        // ให้ค้นหาจาก Tag Player
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


        if (zombieCountText != null)
        {
            zombieCountText.text =
                "Zombies Left: " +
                remainingEnemies;
        }


        if (isSpawning ||
            remainingEnemies > 0)
        {
            return;
        }


        StartCoroutine(StartNextWave());
    }


    // =========================================================
    // COUNT ENEMIES
    // =========================================================

    int GetActiveEnemyCount()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");


        int count = 0;


        foreach (GameObject enemyObj in enemies)
        {
            if (enemyObj != null &&
                enemyObj.activeInHierarchy)
            {
                Enemy enemyScript =
                    enemyObj.GetComponent<Enemy>();


                if (enemyScript != null &&
                    enemyScript.enabled)
                {
                    count++;
                }
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

        if (currentWave >= bossStartWave)
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
        // FINISHED
        // =====================================================

        isSpawning = false;
    }


    // =========================================================
    // FIND RANDOM POSITION AROUND PLAYER
    // =========================================================

    bool FindRandomSpawnPosition(
        out Vector2 position
    )
    {
        position = Vector2.zero;


        if (player == null)
        {
            Debug.LogWarning(
                "ยังไม่ได้กำหนด Player!"
            );

            return false;
        }


        // =====================================================
        // TRY RANDOM POSITIONS
        // =====================================================

        for (int i = 0;
             i < spawnPositionAttempts;
             i++)
        {
            // สุ่มมุมรอบ Player
            float angle =
                Random.Range(
                    0f,
                    360f
                );


            // สุ่มระยะ
            float distance =
                Random.Range(
                    minSpawnDistance,
                    maxSpawnDistance
                );


            // แปลงมุมเป็น Vector2
            Vector2 direction =
                new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );


            // ตำแหน่งรอบ Player
            Vector2 randomPosition =
                (Vector2)player.position +
                direction * distance;


            // =================================================
            // CHECK OBSTACLE
            // =================================================

            Collider2D hit =
                Physics2D.OverlapCircle(
                    randomPosition,
                    spawnCheckRadius,
                    obstacleLayer
                );


            // ถ้าไม่ชนกำแพง
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


        // =====================================================
        // SPAWN BOSS
        // =====================================================

        GameObject boss =
            Instantiate(
                bossPrefab,
                spawnPosition,
                Quaternion.identity
            );


        // ให้ระบบนับ Boss เป็น Enemy
        boss.tag =
            "Enemy";


        // =====================================================
        // RESET HP
        // =====================================================

        Enemy bossEnemy =
            boss.GetComponent<Enemy>();


        if (bossEnemy != null)
        {
            bossEnemy.SetHealth(
                bossEnemy.maxHealth
            );
        }


        Debug.Log(
            "================================"
        );

        Debug.Log(
            "BOSS SPAWNED!"
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


        for (int i = 0;
             i < ammoBoxesPerWave;
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


            // =================================================
            // RANDOM AMMO
            // =================================================

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


            Debug.Log(
                "Spawn Ammo: " +
                selectedAmmo.name +
                " | Position: " +
                spawnPosition
            );
        }
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


        // =====================================================
        // SPAWN ZOMBIE
        // =====================================================

        GameObject newEnemy =
            Instantiate(
                selectedPrefab,
                spawnPosition,
                Quaternion.identity
            );


        newEnemy.tag =
            "Enemy";


        // =====================================================
        // RESET HP
        // =====================================================

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
        // ไม่มี Enemy ที่ตรงเงื่อนไข
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