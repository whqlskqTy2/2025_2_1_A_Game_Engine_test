using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMinionSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject minionPrefab;         // 소환할 몬스터 프리팹
    public Transform[] spawnPoints;         // 소환 지점들 (비워도 됨 → 보스 위치 사용)
    public float spawnInterval = 5f;        // 몇 초마다 한 번씩 소환 시도
    public int spawnPerWave = 2;            // 한 웨이브에 총 몇 마리 뽑을지

    [Header("Limits")]
    public int maxMinionsAlive = 6;         // 동시에 살아 있는 최대 수
    public int maxTotalSpawn = 999;         // 전체 전투 동안 총 생성 가능한 마리 수(안전장치)

    [Header("Auto Start")]
    public bool startSpawningOnEnable = true; // 보스가 나타나자마자 소환 시작?
    public float firstSpawnDelay = 0f;        // 보스 등장 직후 몇 초 뒤에 첫 웨이브를 뿌릴지

    [Header("Boss State")]
    public EnemyHealth bossHealth; // 보스 HP. (죽으면 멈춤). 안 넣어도 작동은 함.

    [Header("Gizmo")]
    public Color gizmoColor = new Color(0.4f, 0.8f, 1f, 0.4f);
    public float gizmoRadius = 0.3f;

    // 내부 상태
    private readonly List<GameObject> aliveMinions = new List<GameObject>();
    private float lastSpawnTime = -999f;
    private bool spawningActive = false;
    private int totalSpawnedCount = 0;

    void Awake()
    {
        // bossHealth 자동으로 찾아보기(없으면 null 유지)
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
        // 보스가 죽었으면 그만
        if (bossHealth != null && bossHealth.GetCurrentHP() <= 0)
        {
            spawningActive = false;
        }

        if (!spawningActive) return;

        // 현재 살아있는 소환몹 리스트 정리
        CleanupDeadMinions();

        // 한도 초과면 멈춘다
        if (totalSpawnedCount >= maxTotalSpawn) return;
        if (aliveMinions.Count >= maxMinionsAlive) return;

        // 쿨타임
        if (Time.time < lastSpawnTime + spawnInterval) return;

        // 스폰
        DoSpawnWave();
    }

    void DoSpawnWave()
    {
        if (!minionPrefab)
        {
            Debug.LogWarning("[BossMinionSpawnerMulti] minionPrefab이 비어있습니다.", this);
            return;
        }

        lastSpawnTime = Time.time;

        // 한 웨이브에서 spawnPerWave 마리까지 생성 시도
        for (int i = 0; i < spawnPerWave; i++)
        {
            // 소환 위치 결정
            Transform chosenPoint = ChooseSpawnPoint();
            Vector3 pos = chosenPoint ? chosenPoint.position : transform.position;
            Quaternion rot = chosenPoint ? chosenPoint.rotation : transform.rotation;

            GameObject m = Instantiate(minionPrefab, pos, rot);

            aliveMinions.Add(m);
            totalSpawnedCount++;

            // 안전장치: 초과하면 중단
            if (aliveMinions.Count >= maxMinionsAlive) break;
            if (totalSpawnedCount >= maxTotalSpawn) break;
        }
    }

    Transform ChooseSpawnPoint()
    {
        // 여러 스폰 포인트 중 랜덤 선택.
        // spawnPoints가 비어 있으면 null을 반환 → 보스 위치 사용
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int idx = Random.Range(0, spawnPoints.Length);
            return spawnPoints[idx];
        }
        return null;
    }

    void CleanupDeadMinions()
    {
        for (int i = aliveMinions.Count - 1; i >= 0; i--)
        {
            var mob = aliveMinions[i];
            if (mob == null || !mob.activeInHierarchy)
            {
                aliveMinions.RemoveAt(i);
            }
        }
    }

    public void StartSpawning()
    {
        spawningActive = true;

        // firstSpawnDelay 이후부터 첫 웨이브가 나오도록 타이밍 초기화
        // 예시:
        //   lastSpawnTime = Time.time - spawnInterval + firstSpawnDelay
        // 이렇게 하면:
        //   firstSpawnDelay초 지난 다음 Update에서 바로 웨이브가 나옴
        lastSpawnTime = Time.time - spawnInterval + firstSpawnDelay;
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

        // 여러 포인트 전부 표시
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            foreach (var p in spawnPoints)
            {
                if (p)
                {
                    Gizmos.DrawSphere(p.position, gizmoRadius);
                }
            }
        }
        else
        {
            // 스폰 포인트 없으면 자기 위치
            Gizmos.DrawSphere(transform.position, gizmoRadius);
        }
    }
}