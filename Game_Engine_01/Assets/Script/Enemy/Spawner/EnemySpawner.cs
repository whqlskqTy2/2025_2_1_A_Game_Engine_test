using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;     //  소환할 적 프리팹 (인스펙터에서 드래그)
    public int maxAlive = 5;           //  동시에 살아있을 수 있는 최대 마리 수
    public int spawnLimit = 20;        // 이 스포너가 평생 소환할 수 있는 총 마리 수
    public float spawnInterval = 2f;   //  소환 주기 (초)

    [Header("Start Options")]
    public bool autoStart = true;         // 게임 시작 시 자동으로 스폰 시스템을 켤지
    public float startDelayFromGameStart = 0f; //  "게임 시작 후 몇 초 뒤에" 스포너가 활성화되는가
    public float firstSpawnDelay = 0f;    //  스포너가 활성화된 뒤 첫 마리 뽑기까지 지연

    [Header("Gizmo")]
    public Color gizmoColor = new Color(1f, 0.2f, 0.2f, 0.4f);
    public float gizmoRadius = 0.3f;

    private float spawnerActivatedAt = -1f;   // Activate()가 실제로 발동된 시각
    private float lastSpawnTime = -999f;
    private int totalSpawned = 0;             // 지금까지 소환된 전체 수
    private readonly List<GameObject> aliveList = new List<GameObject>();

    private bool wantsToRun = false;          // autoStart 등으로 "언젠가 돌겠다" 의사
    private bool isActive = false;            // 실제로 돌아가는 중인지 (startDelay 지나 Activation 완료됐는지)

    void Start()
    {
        if (autoStart)
        {
            // "돌 준비는 해둔다" 상태
            wantsToRun = true;
        }
    }

    void Update()
    {
        // 아직 돌 의사가 없다면 그냥 리턴
        if (!wantsToRun) return;

        // 아직 active가 아니라면, startDelayFromGameStart가 지났는지 확인해서 Activate() 실행 시도
        if (!isActive)
        {
            if (Time.time >= startDelayFromGameStart)
            {
                ActivateInternal(); // 내부용 활성화
            }
            else
            {
                // 아직 시작 시간이 안 됨 → 그냥 기다림
                return;
            }
        }

        // 여기부터는 isActive == true 상태

        // 죽은 애들 정리
        CleanupDead();

        // 한도 체크
        if (totalSpawned >= spawnLimit) return;      // 총 소환 횟수 제한 도달
        if (aliveList.Count >= maxAlive) return;     // 동시에 존재 가능한 수 초과

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

        aliveList.Add(clone);
    }

    void CleanupDead()
    {
        for (int i = aliveList.Count - 1; i >= 0; i--)
        {
            if (aliveList[i] == null)
            {
                aliveList.RemoveAt(i);
            }
            else
            {
                if (!aliveList[i].activeInHierarchy)
                {
                    aliveList.RemoveAt(i);
                }
            }
        }
    }

    // 외부에서 이 스포너를 수동으로 켜고 싶을 때 호출하는 함수
    public void Activate()
    {
        wantsToRun = true;
    }

    // 외부에서 이 스포너를 강제로 끄고 싶을 때 호출
    public void Deactivate()
    {
        wantsToRun = false;
        isActive = false;
    }

    // 실제 내부 활성화 로직
    void ActivateInternal()
    {
        // 한 번만 세팅
        isActive = true;
        spawnerActivatedAt = Time.time;

        // 이제부터는 spawnInterval 기준 타이머를 돌리는데,
        // 첫 마리는 좀 더 늦게 나오게 하고 싶으면 firstSpawnDelay를 반영.
        // 예: Activate된 순간으로부터 firstSpawnDelay 이후에 첫 스폰되도록,
        // lastSpawnTime을 "지금 - interval + firstSpawnDelay" 로 맞춘다.
        lastSpawnTime = Time.time - spawnInterval + firstSpawnDelay;
    }

    // 읽기용 프로퍼티
    public int AliveCount => aliveList.Count;
    public int TotalSpawned => totalSpawned;
    public bool IsFinished => totalSpawned >= spawnLimit && aliveList.Count == 0;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}