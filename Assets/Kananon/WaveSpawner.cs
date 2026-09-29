using System.Collections;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 3f;
    public float spawnDelay = 1f;
    public int baseZombieCount = 3;
    public int extraZombiesPerWave = 2;

    [Header("UI Settings (ลาก Text Mesh Pro มาใส่)")]
    public TextMeshProUGUI waveText;        // ตัวหนังสือแสดงเลข Wave
    public TextMeshProUGUI zombieCountText; // ตัวหนังสือแสดงจำนวนซอมบี้ที่เหลือ

    private int currentWave = 0;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        int remainingZombies = GameObject.FindGameObjectsWithTag("Enemy").Length;

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

    IEnumerator StartNextWave()
    {
        isSpawning = true;

        if (waveText != null && currentWave > 0)
        {
            waveText.text = "Wave Cleared! Get Ready...";

            // เพิ่มโบนัสคะแนนเมื่อผ่านแต่ละ Wave (เช่น Wave 1 = 500, Wave 2 = 1,000)
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

        int zombiesToSpawn = baseZombieCount + (currentWave - 1) * extraZombiesPerWave;

        for (int i = 0; i < zombiesToSpawn; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
    }

    void SpawnZombie()
    {
        if (spawnPoints.Length == 0 || zombiePrefab == null) return;

        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(zombiePrefab, randomPoint.position, randomPoint.rotation);
    }
}