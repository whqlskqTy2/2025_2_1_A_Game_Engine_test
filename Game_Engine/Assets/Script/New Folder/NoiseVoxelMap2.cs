using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseVoxelMap2 : MonoBehaviour
{
    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject waterPrefab;
    public GameObject mineral;

    public int width = 20;
    public int depth = 20;
    public int maxHeight = 16;
    [SerializeField] float noiseScale = 20f;
    [SerializeField] int waterHeight = 4; // 물 높이 기준선

    void Start()
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
                    // 최상단 블록이면 Grass, 아니면 Dirt
                    if (y == h - 1)
                        Place(grassPrefab, BlockType.Grass, x, y, z);
                    else
                        Place(dirtPrefab, BlockType.Dirt, x, y, z);
                }

                // 물 채우기: 일정 높이 이하에 블록이 없으면 물 배치
                for (int y = h; y < waterHeight; y++)
                {
                    Place(waterPrefab, BlockType.Water, x, y, z);
                }
            }
        }
    }

    // --- 수정된 Place() ---
    private void Place(GameObject prefab, BlockType type, int x, int y, int z)
    {
        var go = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, transform);
        go.name = $"{prefab.name}_{x}_{y}_{z}";

        // Block 컴포넌트 세팅
        var b = go.GetComponent<Block>() ?? go.AddComponent<Block>();
        b.type = type;

        // 블록 타입별 설정
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
                b.mineable = false; // 물은 채굴 불가
                break;
        }
    }
}