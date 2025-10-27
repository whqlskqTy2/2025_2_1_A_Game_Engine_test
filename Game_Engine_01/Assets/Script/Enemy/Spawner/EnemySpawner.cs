using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;     // 🔹 소환할 적 프리팹 (인스펙터에서 드래그)
    public int maxAlive = 5;           // 🔹 동시에 살아있을 수 있는 최대 마리 수
    public int spawnLimit = 20;        // 🔹 이 스포너가 평생 소환할 수 있는 총 마리 수
    public float spawnInterval = 2f;   // 🔹 소환 주기 (초)

    [Header("Start Options")]
    public bool autoStart = true;      // 시작하자마자 소환 시작할지
    public float firstSpawnDelay = 0f; // 첫 소환까지 딜레이

    [Header("Gizmo")]
    public Color gizmoColor = new Color(1f, 0.2f, 0.2f, 0.4f);
    public float gizmoRadius = 0.3f;

    private float lastSpawnTime = -999f;
    private int totalSpawned = 0;       // 지금까지 소환된 전체 수
    private readonly List<GameObject> aliveList = new List<GameObject>();

    private bool isActive = false;

    void Start()
    {
        if (autoStart)
        {
            Activate();
        }
    }

    void Update()
    {
        if (!isActive) return;

        // dead/alive 관리: 이미 죽은 애들 리스트에서 제거
        CleanupDead();

        // 한도 체크
        if (totalSpawned >= spawnLimit) return;             // 총 소환 횟수 제한 도달
        if (aliveList.Count >= maxAlive) return;            // 동시에 존재 가능한 수 초과

        // 쿨타임 체크
        if (Time.time < lastSpawnTime + spawnInterval) return;

        // spawn!
        SpawnOne();
    }

    void SpawnOne()
    {
        if (!enemyPrefab)
        {
            Debug.LogWarning("[EnemySpawner] enemyPrefab이 비어있습니다.", this);
            return;
        }

        lastSpawnTime = Time.time;

        GameObject clone = Instantiate(enemyPrefab, transform.position, transform.rotation);
        totalSpawned++;

        // 리스트에 등록
        aliveList.Add(clone);

        // 적이 죽을 때 자동으로 리스트에서 빠지게 하고 싶다면, 
        // 적 쪽에서 Death 때 이벤트를 쏘거나, 우리가 주기적으로 CleanupDead()로 정리하는 걸로 충분.
    }

    void CleanupDead()
    {
        // 리스트에 null(죽어서 Destroy된 애들) 남아 있으면 정리
        for (int i = aliveList.Count - 1; i >= 0; i--)
        {
            if (aliveList[i] == null)
            {
                aliveList.RemoveAt(i);
            }
            else
            {
                // 혹시 비활성화만 된 경우도 제거하고 싶다면:
                if (!aliveList[i].activeInHierarchy)
                {
                    aliveList.RemoveAt(i);
                }
            }
        }
    }

    // --- 외부에서 웨이브 시작/중지하고 싶을 때 쓸 API ---

    public void Activate()
    {
        isActive = true;
        lastSpawnTime = Time.time - spawnInterval + firstSpawnDelay;
        // 이렇게 하면 첫 Update에서 거의 곧바로 소환되거나 firstSpawnDelay 후에 되도록 타이밍 세팅
    }

    public void Deactivate()
    {
        isActive = false;
    }

    // 현재 얼마나 소환했는지, 얼마나 살아있는지 확인하고 싶을 때 쓰는 읽기용 프로퍼티
    public int AliveCount => aliveList.Count;
    public int TotalSpawned => totalSpawned;
    public bool IsFinished => totalSpawned >= spawnLimit && aliveList.Count == 0;

    // 에디터에서 위치를 보기 좋게
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}
