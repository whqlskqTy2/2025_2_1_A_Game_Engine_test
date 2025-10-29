using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMinionSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject minionPrefab;      // ��ȯ�� ���� ������
    public Transform spawnPoint;         // ��ȯ ��ġ. ��� �ڱ� transform
    public float spawnInterval = 5f;     // �� �ʸ��� �� ���� ��ȯ����
    public int spawnPerWave = 1;         // �� ���� �� ���� ������

    [Header("Limits")]
    public int maxMinionsAlive = 5;      // ���� ����ִ� ��ȯ�� �ִ�ġ
    public int maxTotalSpawn = 999;      // ���� ���� ��ü ���� ���� �� �ִ� �ѷ�(������ġ)

    [Header("Auto Start")]
    public bool startSpawningOnEnable = true; // ������ Ȱ��ȭ���ڸ��� ��ȯ ����

    [Header("Gizmo")]
    public Color gizmoColor = new Color(0.4f, 0.8f, 1f, 0.4f);
    public float gizmoRadius = 0.3f;

    private float lastSpawnTime = -999f;
    private bool spawningActive = false;
    private int totalSpawnedCount = 0;
    private readonly List<GameObject> aliveMinions = new List<GameObject>();

    // ����: ���� ü�� �����ؼ� ������ �ߴ��ϰ� ���� ��
    public EnemyHealth bossHealth; // �Ǵ� BossHealth ��. ����� ������ ������ ������ �� ����

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
        // ������ �׾����� ����
        if (bossHealth != null && bossHealth.GetCurrentHP() <= 0)
        {
            spawningActive = false;
        }

        if (!spawningActive) return;

        // ����/��Ȱ��ȭ�� �ϼ��� ����
        CleanupDeadMinions();

        // �ѵ� �˻�
        if (totalSpawnedCount >= maxTotalSpawn) return;
        if (aliveMinions.Count >= maxMinionsAlive) return;

        // ��Ÿ�� �˻�
        if (Time.time < lastSpawnTime + spawnInterval) return;

        // ������ ����
        DoSpawnWave();
    }

    void DoSpawnWave()
    {
        if (!minionPrefab)
        {
            Debug.LogWarning("[BossMinionSpawner] minionPrefab�� ����־��.", this);
            return;
        }

        lastSpawnTime = Time.time;

        for (int i = 0; i < spawnPerWave; i++)
        {
            // ���� ��ġ �ణ ���� ������ �� ���� ���� (��ħ ����)
            Vector3 pos = spawnPoint.position;
            Quaternion rot = spawnPoint.rotation;

            GameObject m = GameObject.Instantiate(minionPrefab, pos, rot);
            aliveMinions.Add(m);
            totalSpawnedCount++;

            // ������ġ: Ȥ�� ��� �ʰ������� ���� �ߴ�
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
        // �ٷ� ��ȯ���� ���� spawnInterval ���Ŀ� ������ �ϰ� ������:
        // lastSpawnTime = Time.time;
        //
        // �ٷ� ��ȯ�ϰ� ������:
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