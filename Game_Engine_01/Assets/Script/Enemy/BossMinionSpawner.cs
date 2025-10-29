using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMinionSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject minionPrefab;         // ��ȯ�� ���� ������
    public Transform[] spawnPoints;         // ��ȯ ������ (����� �� �� ���� ��ġ ���)
    public float spawnInterval = 5f;        // �� �ʸ��� �� ���� ��ȯ �õ�
    public int spawnPerWave = 2;            // �� ���̺꿡 �� �� ���� ������

    [Header("Limits")]
    public int maxMinionsAlive = 6;         // ���ÿ� ��� �ִ� �ִ� ��
    public int maxTotalSpawn = 999;         // ��ü ���� ���� �� ���� ������ ���� ��(������ġ)

    [Header("Auto Start")]
    public bool startSpawningOnEnable = true; // ������ ��Ÿ���ڸ��� ��ȯ ����?
    public float firstSpawnDelay = 0f;        // ���� ���� ���� �� �� �ڿ� ù ���̺긦 �Ѹ���

    [Header("Boss State")]
    public EnemyHealth bossHealth; // ���� HP. (������ ����). �� �־ �۵��� ��.

    [Header("Gizmo")]
    public Color gizmoColor = new Color(0.4f, 0.8f, 1f, 0.4f);
    public float gizmoRadius = 0.3f;

    // ���� ����
    private readonly List<GameObject> aliveMinions = new List<GameObject>();
    private float lastSpawnTime = -999f;
    private bool spawningActive = false;
    private int totalSpawnedCount = 0;

    void Awake()
    {
        // bossHealth �ڵ����� ã�ƺ���(������ null ����)
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
        // ������ �׾����� �׸�
        if (bossHealth != null && bossHealth.GetCurrentHP() <= 0)
        {
            spawningActive = false;
        }

        if (!spawningActive) return;

        // ���� ����ִ� ��ȯ�� ����Ʈ ����
        CleanupDeadMinions();

        // �ѵ� �ʰ��� �����
        if (totalSpawnedCount >= maxTotalSpawn) return;
        if (aliveMinions.Count >= maxMinionsAlive) return;

        // ��Ÿ��
        if (Time.time < lastSpawnTime + spawnInterval) return;

        // ����
        DoSpawnWave();
    }

    void DoSpawnWave()
    {
        if (!minionPrefab)
        {
            Debug.LogWarning("[BossMinionSpawnerMulti] minionPrefab�� ����ֽ��ϴ�.", this);
            return;
        }

        lastSpawnTime = Time.time;

        // �� ���̺꿡�� spawnPerWave �������� ���� �õ�
        for (int i = 0; i < spawnPerWave; i++)
        {
            // ��ȯ ��ġ ����
            Transform chosenPoint = ChooseSpawnPoint();
            Vector3 pos = chosenPoint ? chosenPoint.position : transform.position;
            Quaternion rot = chosenPoint ? chosenPoint.rotation : transform.rotation;

            GameObject m = Instantiate(minionPrefab, pos, rot);

            aliveMinions.Add(m);
            totalSpawnedCount++;

            // ������ġ: �ʰ��ϸ� �ߴ�
            if (aliveMinions.Count >= maxMinionsAlive) break;
            if (totalSpawnedCount >= maxTotalSpawn) break;
        }
    }

    Transform ChooseSpawnPoint()
    {
        // ���� ���� ����Ʈ �� ���� ����.
        // spawnPoints�� ��� ������ null�� ��ȯ �� ���� ��ġ ���
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

        // firstSpawnDelay ���ĺ��� ù ���̺갡 �������� Ÿ�̹� �ʱ�ȭ
        // ����:
        //   lastSpawnTime = Time.time - spawnInterval + firstSpawnDelay
        // �̷��� �ϸ�:
        //   firstSpawnDelay�� ���� ���� Update���� �ٷ� ���̺갡 ����
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

        // ���� ����Ʈ ���� ǥ��
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
            // ���� ����Ʈ ������ �ڱ� ��ġ
            Gizmos.DrawSphere(transform.position, gizmoRadius);
        }
    }
}