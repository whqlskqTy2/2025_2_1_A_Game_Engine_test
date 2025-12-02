using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackRange = 5f;   // 사거리
    public float fireRate = 1f;      // 초당 공격 횟수
    public int damage = 3;           // 한 번 공격 시 대미지

    [Header("발사 이펙트")]
    public Transform firePoint;          // ★ 총구 위치
    public GameObject projectilePrefab;  // ★ 탄환 프리팹

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            EnemyStatus target = FindTargetInRange();
            if (target != null)
            {
                Shoot(target);
                fireCooldown = 1f / fireRate;
            }
        }
    }

    EnemyStatus FindTargetInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        EnemyStatus closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            EnemyStatus enemy = hit.GetComponent<EnemyStatus>();
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    void Shoot(EnemyStatus target)
    {
        if (target == null) return;

        if (projectilePrefab != null)
        {
            // firePoint가 있으면 그 위치에서, 없으면 타워 위치에서 발사
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

            GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Projectile proj = projObj.GetComponent<Projectile>();

            if (proj != null)
            {
                proj.SetTarget(target, damage);
            }
        }
        else
        {
            // 프리팹 없으면 그냥 히트스캔처럼 즉시 대미지
            target.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}