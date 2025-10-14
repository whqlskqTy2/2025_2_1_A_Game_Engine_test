using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // ───────── FSM ─────────
    public enum EnemyState { Idle, Trace, Attack, RunAway }
    public EnemyState state = EnemyState.Idle;

    // ───────── 이동/타겟 ─────────
    public float moveSpeed = 2f;        // 추적 속도
    public float runSpeed = 5.5f;      // 도망 속도
    private Transform player;

    // ───────── 체력 ─────────
    public int maxHP = 5;
    private int currentHP;
    [Range(0f, 1f)] public float fleeHPPercent = 0.2f; // 체력 20% 이하면 도망

    // ───────── 범위 ─────────
    public float traceRange = 15f;     // 추적 시작/유지 범위
    public float attackRange = 6f;      // 공격 범위
    public float safeRange = 10f;     // 도망 중 이 거리 이상 벌어지면 Idle로 복귀

    // ───────── 공격 ─────────
    public float attackCooldown = 1.5f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private float lastAttackTime;

    // ───────── UI (월드 스페이스 HP 바) ─────────
    [Header("UI")]
    public Slider hpSlider;                 // 자식 Canvas에 있는 Slider 할당
    public bool faceCamera = true;          // 체력바가 카메라를 보게 할지 여부

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

        // ── FSM 상태 전환 ──
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
                // 충분히 멀어지면 안전. Idle 복귀
                if (dist >= safeRange)
                    state = EnemyState.Idle;
                break;
        }
    }

    void LateUpdate()
    {
        // 체력바가 항상 카메라를 보도록(선택)
        if (faceCamera && hpSlider != null && Camera.main != null)
        {
            Transform t = hpSlider.transform;
            Transform cam = Camera.main.transform;
            t.rotation = Quaternion.LookRotation(t.position - cam.position);
        }
    }

    // ===== 데미지 처리 =====
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        if (hpSlider != null) hpSlider.value = currentHP;

        Debug.Log($"{gameObject.name} 피격! 현재 체력: {currentHP}");

        // 체력이 임계치 이하이면 즉시 도망
        if (currentHP <= maxHP * fleeHPPercent && state != EnemyState.RunAway)
            state = EnemyState.RunAway;

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        Destroy(gameObject);
    }

    // ===== 상태 동작 =====
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
        // 플레이어 반대 방향으로 이동
        Vector3 dir = (transform.position - player.position).normalized;
        dir.y = 0f;
        transform.position += dir * runSpeed * Time.deltaTime;
        // 도망 때는 굳이 플레이어를 보지 않게 방향을 진행 방향으로
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
        // 공격 중에도 플레이어를 바라보게
        Vector3 lookDir = (player.position - transform.position);
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // 발사 방향 계산
        Vector3 dir = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null) ep.SetDirection(dir);
    }
}
