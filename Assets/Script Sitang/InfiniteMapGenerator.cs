using UnityEngine;
using System.Collections.Generic;

public class InfiniteMapGenerator : MonoBehaviour
{
    public Transform player;

    // ใส่ Prefab พื้นทั้ง 6 แบบ
    public GameObject[] mapChunkPrefabs;

    public int chunkSize = 20;
    public int viewDistance = 3;

    private Dictionary<Vector2Int, GameObject> chunks =
        new Dictionary<Vector2Int, GameObject>();

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
        if (player == null ||
            mapChunkPrefabs == null ||
            mapChunkPrefabs.Length == 0)
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
        int randomIndex = Random.Range(
            0,
            mapChunkPrefabs.Length
        );

        GameObject selectedPrefab =
            mapChunkPrefabs[randomIndex];

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

        SpriteRenderer spriteRenderer =
            chunk.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = -10;
        }

        chunk.name =
            "Chunk_" + position.x + "_" + position.y;

        chunks.Add(position, chunk);
    }

    void RemoveFarChunks(Vector2Int playerChunk)
    {
        List<Vector2Int> chunksToRemove =
            new List<Vector2Int>();

        foreach (var chunk in chunks)
        {
            int distanceX =
                Mathf.Abs(chunk.Key.x - playerChunk.x);

            int distanceY =
                Mathf.Abs(chunk.Key.y - playerChunk.y);

            if (distanceX > viewDistance ||
                distanceY > viewDistance)
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