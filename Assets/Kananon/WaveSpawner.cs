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
        public int minWaveToAppear = 1; // ตั้งค่าเป็น 5 สำหรับศัตรูพิเศษ
    }

    [Header("Enemy Settings")]
    public EnemySpawnInfo[] enemyTypes;
    public Transform playerTransform;
    public float minSpawnRadius = 10f;
    public float maxSpawnRadius = 18f;

    [Header("Item Settings (ประจำ Wave)")]
    public GameObject medkitPrefab;
    public int medkitsPerWave = 1;

    public GameObject[] powerUpPrefabs;
    public int powerUpsPerWave = 1;

    [Header("Wave Progression Settings")]
    public float baseTimeBetweenWaves = 5f;
    public float minTimeBetweenWaves = 2f;
    public float timeReducePerWave = 0.3f;

    public float baseSpawnDelay = 1.0f;
    public float minSpawnDelay = 0.2f;
    public float spawnDelayReducePerWave = 0.05f;

    public int baseZombieCount = 5;
    public int extraZombiesPerWave = 3;

    [Header("UI Settings")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI zombieCountText;

    private int currentWave = 0;
    private bool isSpawning = false;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        int remainingZombies = GetActiveEnemyCount();

        if (zombieCountText != null)
        {
            zombieCountText.text = "Zombies Left: " + remainingZombies;
        }

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
        }

        float currentWaitTime = Mathf.Max(minTimeBetweenWaves, baseTimeBetweenWaves - ((currentWave - 1) * timeReducePerWave));
        yield return new WaitForSeconds(currentWaitTime);

        currentWave++;

        if (waveText != null)
        {
            waveText.text = "WAVE " + currentWave;
        }

        // เสกไอเทมประจำ Wave ใกล้ๆ ตัวผู้เล่น
        if (medkitPrefab != null) SpawnItemsAroundPlayer(new GameObject[] { medkitPrefab }, medkitsPerWave);
        SpawnItemsAroundPlayer(powerUpPrefabs, powerUpsPerWave);

        int zombiesToSpawn = baseZombieCount + (currentWave - 1) * extraZombiesPerWave;

        // อัปเดตโควต้าผีใน InfiniteMapGenerator (แก้ Warning เป็น FindAnyObjectByType แล้ว)
        InfiniteMapGenerator mapGen = Object.FindAnyObjectByType<InfiniteMapGenerator>();
        if (mapGen != null)
        {
            mapGen.ResetWaveEnemyCount(zombiesToSpawn);
        }

        float currentSpawnDelay = Mathf.Max(minSpawnDelay, baseSpawnDelay - ((currentWave - 1) * spawnDelayReducePerWave));

        for (int i = 0; i < zombiesToSpawn; i++)
        {
            SpawnZombieAroundPlayer();
            yield return new WaitForSeconds(currentSpawnDelay);
        }

        isSpawning = false;
    }

    void SpawnZombieAroundPlayer()
    {
        GameObject selectedPrefab = GetRandomEnemyPrefab();
        if (selectedPrefab == null || playerTransform == null) return;

        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 spawnPos = playerTransform.position + new Vector3(randomCircle.x, randomCircle.y, 0);

        GameObject newEnemy = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        newEnemy.tag = "Enemy";
    }

    void SpawnItemsAroundPlayer(GameObject[] itemPrefabs, int count)
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0 || playerTransform == null) return;

        for (int i = 0; i < count; i++)
        {
            GameObject selectedItem = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

            if (selectedItem != null)
            {
                Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(3f, 8f);
                Vector3 spawnPos = playerTransform.position + new Vector3(randomCircle.x, randomCircle.y, 0);

                Instantiate(selectedItem, spawnPos, Quaternion.identity);
            }
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