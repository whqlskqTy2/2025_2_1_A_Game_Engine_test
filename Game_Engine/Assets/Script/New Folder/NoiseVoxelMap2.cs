using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseVoxelMap2 : MonoBehaviour
{
    [Header("Block Prefabs")]
    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject waterPrefab;
    public GameObject mineral;

    [Header("Map Settings")]
    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16;
    [SerializeField] float noiseScale = 20f;
    [SerializeField] int waterHeight = 4;

    GameObject GetPrefab(BlockType type)
    {
        return type switch
        {
            BlockType.Dirt => dirtPrefab,
            BlockType.Grass => grassPrefab,
            BlockType.Water => waterPrefab,
            _ => null
        };
    }

    void Start()
    {
        GenerateMap();
        SpawnTestBlocks(); //  BlockSpawner 
    }

    public void PlaceTile(Vector3Int pos, BlockType type)
    {
        GameObject prefab = GetPrefab(type);

        if (prefab == null)
        {
            Debug.LogError($" 해당 BlockType의 Prefab 없음 : {type}");
            return;
        }

        // 이미 블록이 있는지 체크 (겹치는 설치 방지)
        Collider[] hits = Physics.OverlapBox(
            pos,
            Vector3.one * 0.45f,
            Quaternion.identity
        );

        if (hits.Length > 0)
        {
            Debug.Log("이미 블록이 존재해서 설치할 수 없음");
            return;
        }

        // 설치
        Place(prefab, type, pos.x, pos.y, pos.z);
        Debug.Log($"설치됨 : {type} at {pos}");
    }

    void GenerateMap()
    {
        float offsetX = Random.Range(-9999f, 9999f);
        float offsetZ = Random.Range(-9999f, 9999f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float nx = (x + offsetX) / noiseScale;
                float nz = (z + offsetZ) / noiseScale;

                float noise = Mathf.PerlinNoise(nx, nz);
                int h = Mathf.FloorToInt(noise * maxHeight);

                // 땅 생성
                for (int y = 0; y < h; y++)
                {
                    if (y == h - 1)
                        Place(grassPrefab, BlockType.Grass, x, y, z);
                    else
                        Place(dirtPrefab, BlockType.Dirt, x, y, z);
                }

                // 물 생성
                for (int y = h; y < waterHeight; y++)
                {
                    Place(waterPrefab, BlockType.Water, x, y, z);
                }
            }
        }
    }


    void SpawnTestBlocks()
    {
        Vector3 start = new Vector3(-2, 0, 0);

        Place(dirtPrefab, BlockType.Dirt, (int)start.x, (int)start.y, (int)start.z);
        Place(grassPrefab, BlockType.Grass, (int)start.x + 2, (int)start.y, (int)start.z);
        Place(waterPrefab, BlockType.Water, (int)start.x + 4, (int)start.y, (int)start.z);
    }

  
    private void Place(GameObject prefab, BlockType type, int x, int y, int z)
    {
        var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"{prefab.name}_{x}_{y}_{z}";

        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = type;

        switch (type)
        {
            case BlockType.Dirt:
                b.maxHP = 3;
                b.dropCount = 1;
                b.mineable = true;
                break;

            case BlockType.Grass:
                b.maxHP = 4;
                b.dropCount = 1;
                b.mineable = true;
                break;

            case BlockType.Water:
                b.maxHP = 1;
                b.dropCount = 0;
                b.mineable = false;
                break;
        }
    }
}