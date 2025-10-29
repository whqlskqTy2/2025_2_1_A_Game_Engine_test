using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMinionSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject minionPrefab;      // 소환될 몬스터 프리팹
    public Transform spawnPoint;         // 소환 위치. 비면 자기 transform
    public float spawnInterval = 5f;     // 몇 초마다 한 번씩 소환할지
    public int spawnPerWave = 1;         // 한 번에 몇 마리 뽑을지

    [Header("Limits")]
    public int maxMinionsAlive = 5;      // 현재 살아있는 소환몹 최대치
    public int maxTotalSpawn = 999;      // 보스 생애 전체 동안 뽑을 수 있는 총량(안전장치)

    [Header("Auto Start")]
    public bool startSpawningOnEnable = true; // 보스가 활성화되자마자 소환 시작

    [Header("Gizmo")]
    public Color gizmoColor = new Color(0.4f, 0.8f, 1f, 0.4f);
    public float gizmoRadius = 0.3f;

    private float lastSpawnTime = -999f;
    private bool spawningActive = false;
    private int totalSpawnedCount = 0;
    private readonly List<GameObject> aliveMinions = new List<GameObject>();

    // 선택: 보스 체력 참조해서 죽으면 중단하고 싶을 때
    public EnemyHealth bossHealth; // 또는 BossHealth 등. 비워도 동작은 되지만 있으면 더 안전

    void Awake()
    {
        if (!spawnPoint) spawnPoint = transform;
        if (!bossHealth)
        {
            bossHealth = GetComponentInParent<EnemyHealth>() ?? GetComponent<EnemyHealth>();
        }
    }

    void OnEnable()
    {
        if (startSpawningOnEnable)
        {
            StartSpawning();
        }
    }

    void Update()
    {
        // 보스가 죽었으면 멈춰
        if (bossHealth != null && bossHealth.GetCurrentHP() <= 0)
        {
            spawningActive = false;
        }

        if (!spawningActive) return;

        // 죽은/비활성화된 하수인 정리
        CleanupDeadMinions();

        // 한도 검사
        if (totalSpawnedCount >= maxTotalSpawn) return;
        if (aliveMinions.Count >= maxMinionsAlive) return;

        // 쿨타임 검사
        if (Time.time < lastSpawnTime + spawnInterval) return;

        // 실제로 스폰
        DoSpawnWave();
    }

    void DoSpawnWave()
    {
        if (!minionPrefab)
        {
            Debug.LogWarning("[BossMinionSpawner] minionPrefab이 비어있어요.", this);
            return;
        }

        lastSpawnTime = Time.time;

        for (int i = 0; i < spawnPerWave; i++)
        {
            // 스폰 위치 약간 랜덤 오프셋 줄 수도 있음 (겹침 방지)
            Vector3 pos = spawnPoint.position;
            Quaternion rot = spawnPoint.rotation;

            GameObject m = GameObject.Instantiate(minionPrefab, pos, rot);
            aliveMinions.Add(m);
            totalSpawnedCount++;

            // 안전장치: 혹시 즉시 초과했으면 루프 중단
            if (aliveMinions.Count >= maxMinionsAlive) break;
            if (totalSpawnedCount >= maxTotalSpawn) break;
        }
    }

    void CleanupDeadMinions()
    {
        for (int i = aliveMinions.Count - 1; i >= 0; i--)
        {
            var mob = aliveMinions[i];
            if (mob == null)
            {
                aliveMinions.RemoveAt(i);
            }
            else if (!mob.activeInHierarchy)
            {
                aliveMinions.RemoveAt(i);
            }
        }
    }

    public void StartSpawning()
    {
        spawningActive = true;
        // 바로 소환하지 말고 spawnInterval 이후에 나오게 하고 싶으면:
        // lastSpawnTime = Time.time;
        //
        // 바로 소환하고 싶으면:
        lastSpawnTime = Time.time - spawnInterval;
    }

    public void StopSpawning()
    {
        spawningActive = false;
    }

    public int AliveCount => aliveMinions.Count;
    public int TotalSpawned => totalSpawnedCount;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector3 p = spawnPoint ? spawnPoint.position : transform.position;
        Gizmos.DrawSphere(p, gizmoRadius);
    }
}