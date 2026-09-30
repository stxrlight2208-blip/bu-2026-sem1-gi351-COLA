using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialMap : MonoBehaviour
{
    public Transform player;

    // ใส่ Prefab พื้นทั้ง 6 แบบ
    public GameObject[] mapChunkPrefabs;

    public int chunkSize = 20;
    public int viewDistance = 3;

    [Header("Ammo Spawns Settings")]
    public GameObject[] ammoPrefabs; // Prefab กล่องกระสุน (Normal / MagicSilver)
    [Range(0f, 100f)]
    public float ammoSpawnChance = 40f; // โอกาสเกิดกระสุนใน Chunk (%)
    public int maxAmmoPerChunk = 2;     // จำนวนกระสุนสูงสุดต่อ Chunk

    [Header("Enemy Spawns Settings")]
    public GameObject[] enemyPrefabs; // Prefab ศัตรูชนิดต่างๆ
    [Range(0f, 100f)]
    public float enemySpawnChance = 60f; // โอกาสเกิดศัตรูใน Chunk (%)
    public int maxEnemiesPerChunk = 3;   // จำนวนศัตรูสูงสุดต่อ Chunk

    private Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        Debug.Log("Map Generator Started");
        Debug.Log("Player = " + player);
        Debug.Log("Prefabs = " + mapChunkPrefabs.Length);
        UpdateMap();
    }

    void Update()
    {
        UpdateMap();
    }

    void UpdateMap()
    {
        if (player == null || mapChunkPrefabs == null || mapChunkPrefabs.Length == 0)
        {
            return;
        }

        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.y / chunkSize)
        );

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(
                    playerChunk.x + x,
                    playerChunk.y + y
                );

                if (!chunks.ContainsKey(chunkPosition))
                {
                    CreateChunk(chunkPosition);
                }
            }
        }

        RemoveFarChunks(playerChunk);
    }

    void CreateChunk(Vector2Int position)
    {
        Debug.Log("Creating Chunk: " + position);

        // สุ่ม Prefab พื้น
        int randomIndex = Random.Range(0, mapChunkPrefabs.Length);
        GameObject selectedPrefab = mapChunkPrefabs[randomIndex];

        Vector3 worldPosition = new Vector3(
            position.x * chunkSize,
            position.y * chunkSize,
            0
        );

        GameObject chunk = Instantiate(
            selectedPrefab,
            worldPosition,
            Quaternion.identity,
            transform
        );

        SpriteRenderer spriteRenderer = chunk.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = -10;
        }

        chunk.name = "Chunk_" + position.x + "_" + position.y;

        // สุ่มเสกกล่องกระสุนบน Chunk นี้
        TrySpawnAmmoInChunk(chunk, worldPosition);

        // สุ่มเสกศัตรูบน Chunk นี้ (เว้นการเสกตรง Chunk แรกที่ผู้เล่นยืนตอนเริ่มเกม)
        if (position != Vector2Int.zero)
        {
            TrySpawnEnemiesInChunk(chunk, worldPosition);
        }

        chunks.Add(position, chunk);
    }

    void TrySpawnAmmoInChunk(GameObject parentChunk, Vector3 chunkWorldPos)
    {
        if (ammoPrefabs == null || ammoPrefabs.Length == 0) return;

        if (Random.Range(0f, 100f) <= ammoSpawnChance)
        {
            int ammoCount = Random.Range(1, maxAmmoPerChunk + 1);

            for (int i = 0; i < ammoCount; i++)
            {
                float offsetLimit = (chunkSize / 2f) - 2f;
                float randomX = Random.Range(-offsetLimit, offsetLimit);
                float randomY = Random.Range(-offsetLimit, offsetLimit);

                Vector3 spawnPos = chunkWorldPos + new Vector3(randomX, randomY, 0);

                GameObject randomAmmoPrefab = ammoPrefabs[Random.Range(0, ammoPrefabs.Length)];

                if (randomAmmoPrefab != null)
                {
                    Instantiate(randomAmmoPrefab, spawnPos, Quaternion.identity, parentChunk.transform);
                }
            }
        }
    }

    void TrySpawnEnemiesInChunk(GameObject parentChunk, Vector3 chunkWorldPos)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        if (Random.Range(0f, 100f) <= enemySpawnChance)
        {
            int enemyCount = Random.Range(1, maxEnemiesPerChunk + 1);

            for (int i = 0; i < enemyCount; i++)
            {
                float offsetLimit = (chunkSize / 2f) - 2f;
                float randomX = Random.Range(-offsetLimit, offsetLimit);
                float randomY = Random.Range(-offsetLimit, offsetLimit);

                Vector3 spawnPos = chunkWorldPos + new Vector3(randomX, randomY, 0);

                GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                if (randomEnemyPrefab != null)
                {
                    GameObject newEnemy = Instantiate(randomEnemyPrefab, spawnPos, Quaternion.identity, parentChunk.transform);
                    newEnemy.tag = "Enemy"; // กำหนด Tag ป้องกันการตกหล่น
                }
            }
        }
    }

    void RemoveFarChunks(Vector2Int playerChunk)
    {
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();

        foreach (var chunk in chunks)
        {
            int distanceX = Mathf.Abs(chunk.Key.x - playerChunk.x);
            int distanceY = Mathf.Abs(chunk.Key.y - playerChunk.y);

            if (distanceX > viewDistance || distanceY > viewDistance)
            {
                chunksToRemove.Add(chunk.Key);
            }
        }

        foreach (Vector2Int position in chunksToRemove)
        {
            Destroy(chunks[position]);
            chunks.Remove(position);
        }
    }
}