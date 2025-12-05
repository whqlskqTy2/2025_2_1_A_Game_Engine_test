using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    public GameObject enemyPrefab;   // 어떤 몬스터를
    public int count = 5;            // 몇 마리
}

[System.Serializable]
public class Wave
{
    [Header("이 웨이브에서 나올 몬스터들")]
    public List<SpawnEntry> entries = new List<SpawnEntry>();

    [Header("공격 타이밍")]
    public float spawnInterval = 1f;     // 몬스터 한 마리 간격
    public float delayAfterWave = 3f;    // 웨이브 끝나고 다음 웨이브까지 쉬는 시간
}

public class EnemySpawner : MonoBehaviour
{
    [Header("경로 설정")]
    public PathRoute route;          // 몬스터가 따라갈 PathRoute
    public Transform spawnPoint;     // 실제 스폰 위치(없으면 자기 자신)

    [Header("웨이브 설정")]
    public List<Wave> waves = new List<Wave>();
    public float firstWaveDelay = 2f;

    private int waveIndex = 0;
    private int entryIndex = 0;
    private int spawnedInEntry = 0;
    private float timer = 0f;
    private bool started = false;

    private void Start()
    {
        timer = firstWaveDelay;
    }

    private void Update()
    {
        if (route == null)
        {
            Debug.LogError("[EnemySpawner] route 가 설정되어 있지 않습니다.", this);
            return;
        }

        if (waveIndex >= waves.Count)
        {
            // 모든 웨이브 종료
            return;
        }

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Wave wave = waves[waveIndex];
        if (wave.entries.Count == 0)
        {
            // 비어있는 웨이브면 그냥 넘김
            NextWave();
            return;
        }

        SpawnEntry entry = wave.entries[entryIndex];

        // 한 마리 소환
        SpawnEnemy(entry.enemyPrefab);
        spawnedInEntry++;

        // 이 엔트리에서 다 뽑았으면 다음 엔트리로
        if (spawnedInEntry >= entry.count)
        {
            entryIndex++;
            spawnedInEntry = 0;

            // 엔트리도 끝났으면 웨이브 종료 → 다음 웨이브
            if (entryIndex >= wave.entries.Count)
            {
                NextWave();
            }
            else
            {
                // 같은 웨이브 내에서 다음 종류 몬스터로
                timer = wave.spawnInterval;
            }
        }
        else
        {
            // 같은 종류를 계속 소환
            timer = wave.spawnInterval;
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[EnemySpawner] enemyPrefab 이 비어 있습니다.", this);
            return;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject enemyObj = Instantiate(prefab, pos, Quaternion.identity);

        EnemyMovement move = enemyObj.GetComponent<EnemyMovement>();
        if (move != null)
        {
            move.route = route;
        }
    }

    private void NextWave()
    {
        Wave wave = waves[waveIndex];
        Debug.Log($"[EnemySpawner] Wave {waveIndex} 종료. 다음 웨이브까지 {wave.delayAfterWave}초 대기.");

        waveIndex++;
        entryIndex = 0;
        spawnedInEntry = 0;

        if (waveIndex >= waves.Count)
        {
            Debug.Log("[EnemySpawner] 모든 웨이브 종료.");
            return;
        }

        timer = waves[waveIndex - 1].delayAfterWave;
    }
}