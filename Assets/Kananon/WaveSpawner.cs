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

    [Header("Spawner Settings")]
    public EnemySpawnInfo[] enemyTypes;
    public Transform[] spawnPoints;

    [Header("Ammo Spawner Settings")]
    public GameObject[] ammoPrefabs;      // ใส่ Prefab กล่องกระสุนที่ต้องการเสก (Normal / MagicSilver)
    public Transform[] ammoSpawnPoints;   // จุดที่จะให้กระสุนเกิด (ถ้าไม่ใส่ จะใช้ spawnPoints เดียวกับศัตรู)
    public int ammoBoxesPerWave = 1;      // จำนวนกล่องกระสุนที่จะเสกต่อ round

    [Header("Wave Settings")]
    public float timeBetweenWaves = 3f;
    public float spawnDelay = 1f;
    public int baseZombieCount = 3;
    public int extraZombiesPerWave = 2;

    [Header("UI Settings")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI zombieCountText;

    private int currentWave = 0;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        // เช็กจำนวนศัตรูในฉาก โดยนับเฉพาะตัวที่ยัง alive (สคริปต์ Enemy ยังเปิดใช้งานอยู่)
        int remainingZombies = GetActiveEnemyCount();

        if (zombieCountText != null)
        {
            zombieCountText.text = "Zombies Left: " + remainingZombies;
        }

        // ถ้ากำลังเสกอยู่ หรือยังมีศัตรูเหลืออยู่ ให้รอ
        if (isSpawning || remainingZombies > 0)
        {
            return;
        }

        StartCoroutine(StartNextWave());
    }

    int GetActiveEnemyCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int count = 0;

        foreach (GameObject enemyObj in enemies)
        {
            if (enemyObj != null && enemyObj.activeInHierarchy)
            {
                Enemy enemyScript = enemyObj.GetComponent<Enemy>();
                if (enemyScript != null && enemyScript.enabled)
                {
                    count++;
                }
            }
        }
        return count;
    }

    IEnumerator StartNextWave()
    {
        isSpawning = true;

        if (waveText != null && currentWave > 0)
        {
            waveText.text = "Wave Cleared! Get Ready...";

            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(currentWave * 500);
            }
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;

        if (waveText != null)
        {
            waveText.text = "WAVE " + currentWave;
        }

        // --- เสกกล่องกระสุนประจำ Round ---
        SpawnAmmoBoxes();

        int zombiesToSpawn = baseZombieCount + (currentWave - 1) * extraZombiesPerWave;

        for (int i = 0; i < zombiesToSpawn; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
    }

    void SpawnAmmoBoxes()
    {
        if (ammoPrefabs == null || ammoPrefabs.Length == 0) return;

        // เลือกจุดเสกกระสุน (ถ้าไม่มี ammoSpawnPoints ให้ใช้ spawnPoints ของศัตรู)
        Transform[] pointsToUse = (ammoSpawnPoints != null && ammoSpawnPoints.Length > 0) ? ammoSpawnPoints : spawnPoints;

        if (pointsToUse == null || pointsToUse.Length == 0) return;

        for (int i = 0; i < ammoBoxesPerWave; i++)
        {
            GameObject selectedAmmo = ammoPrefabs[Random.Range(0, ammoPrefabs.Length)];
            Transform randomPoint = pointsToUse[Random.Range(0, pointsToUse.Length)];

            Instantiate(selectedAmmo, randomPoint.position, Quaternion.identity);
        }
    }

    void SpawnZombie()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("ไม่ได้ใส่ SpawnPoints ใน WaveSpawner!");
            return;
        }

        GameObject selectedPrefab = GetRandomEnemyPrefab();

        if (selectedPrefab == null)
        {
            Debug.LogWarning("หา Prefab ศัตรูไม่เจอ! กรุณาเช็ก Enemy Types ใน Inspector");
            return;
        }

        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject newEnemy = Instantiate(
            selectedPrefab,
            randomPoint.position,
            randomPoint.rotation
        );

        newEnemy.tag = "Enemy";

        // เอา HP จาก Prefab ของตัวนั้นโดยตรง
        Enemy enemy = newEnemy.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.SetHealth(enemy.maxHealth);

            Debug.Log(
                "Spawn: " + selectedPrefab.name +
                " | HP = " + enemy.maxHealth
            );
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        if (enemyTypes == null || enemyTypes.Length == 0) return null;

        int totalWeight = 0;

        foreach (var enemy in enemyTypes)
        {
            if (enemy.enemyPrefab != null && currentWave >= enemy.minWaveToAppear)
            {
                totalWeight += enemy.spawnWeight;
            }
        }

        if (totalWeight <= 0)
        {
            foreach (var enemy in enemyTypes)
            {
                if (enemy.enemyPrefab != null) return enemy.enemyPrefab;
            }
            return null;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        foreach (var enemy in enemyTypes)
        {
            if (enemy.enemyPrefab != null && currentWave >= enemy.minWaveToAppear)
            {
                currentSum += enemy.spawnWeight;
                if (randomValue < currentSum)
                {
                    return enemy.enemyPrefab;
                }
            }
        }

        return enemyTypes[0].enemyPrefab;
    }
}