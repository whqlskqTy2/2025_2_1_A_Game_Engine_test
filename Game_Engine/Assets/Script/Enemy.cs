using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // ������������������ FSM ������������������
    public enum EnemyState { Idle, Trace, Attack, RunAway }
    public EnemyState state = EnemyState.Idle;

    // ������������������ �̵�/Ÿ�� ������������������
    public float moveSpeed = 2f;        // ���� �ӵ�
    public float runSpeed = 5.5f;      // ���� �ӵ�
    private Transform player;

    // ������������������ ü�� ������������������
    public int maxHP = 5;
    private int currentHP;
    [Range(0f, 1f)] public float fleeHPPercent = 0.2f; // ü�� 20% ���ϸ� ����

    // ������������������ ���� ������������������
    public float traceRange = 15f;     // ���� ����/���� ����
    public float attackRange = 6f;      // ���� ����
    public float safeRange = 10f;     // ���� �� �� �Ÿ� �̻� �������� Idle�� ����

    // ������������������ ���� ������������������
    public float attackCooldown = 1.5f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private float lastAttackTime;

    // ������������������ UI (���� �����̽� HP ��) ������������������
    [Header("UI")]
    public Slider hpSlider;                 // �ڽ� Canvas�� �ִ� Slider �Ҵ�
    public bool faceCamera = true;          // ü�¹ٰ� ī�޶� ���� ���� ����

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastAttackTime = -attackCooldown;

        currentHP = maxHP;
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        // ���� FSM ���� ��ȯ ����
        switch (state)
        {
            case EnemyState.Idle:
                if (currentHP <= maxHP * fleeHPPercent)
                    state = EnemyState.RunAway;
                else if (dist < traceRange)
                    state = EnemyState.Trace;
                break;

            case EnemyState.Trace:
                if (currentHP <= maxHP * fleeHPPercent)
                    state = EnemyState.RunAway;
                else if (dist < attackRange)
                    state = EnemyState.Attack;
                else if (dist > traceRange)
                    state = EnemyState.Idle;
                else
                    TracePlayer();
                break;

            case EnemyState.Attack:
                if (currentHP <= maxHP * fleeHPPercent)
                    state = EnemyState.RunAway;
                else if (dist > attackRange)
                    state = EnemyState.Trace;
                else
                    AttackPlayer();
                break;

            case EnemyState.RunAway:
                RunAwayFromPlayer();
                // ����� �־����� ����. Idle ����
                if (dist >= safeRange)
                    state = EnemyState.Idle;
                break;
        }
    }

    void LateUpdate()
    {
        // ü�¹ٰ� �׻� ī�޶� ������(����)
        if (faceCamera && hpSlider != null && Camera.main != null)
        {
            Transform t = hpSlider.transform;
            Transform cam = Camera.main.transform;
            t.rotation = Quaternion.LookRotation(t.position - cam.position);
        }
    }

    // ===== ������ ó�� =====
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        if (hpSlider != null) hpSlider.value = currentHP;

        Debug.Log($"{gameObject.name} �ǰ�! ���� ü��: {currentHP}");

        // ü���� �Ӱ�ġ �����̸� ��� ����
        if (currentHP <= maxHP * fleeHPPercent && state != EnemyState.RunAway)
            state = EnemyState.RunAway;

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} ���!");
        Destroy(gameObject);
    }

    // ===== ���� ���� =====
    void TracePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;
        transform.position += dir * moveSpeed * Time.deltaTime;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void RunAwayFromPlayer()
    {
        // �÷��̾� �ݴ� �������� �̵�
        Vector3 dir = (transform.position - player.position).normalized;
        dir.y = 0f;
        transform.position += dir * runSpeed * Time.deltaTime;
        // ���� ���� ���� �÷��̾ ���� �ʰ� ������ ���� ��������
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            ShootProjectile();
        }
        // ���� �߿��� �÷��̾ �ٶ󺸰�
        Vector3 lookDir = (player.position - transform.position);
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // �߻� ���� ���
        Vector3 dir = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null) ep.SetDirection(dir);
    }
}
