using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // ← 비워두면 태그/Follow에서 자동 세팅
    public string targetTag = "Treasure";

    [Header("Refs")]
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Detect")]
    public float detectRange = 18f;
    public float shootRange = 16f;
    public LayerMask lineOfSightMask = 0; // 필요 없으면 0(무시)

    [Header("Shooting")]
    public float shootCooldown = 1f;
    public float projectileSpeed = 18f;
    public int projectileDamage = 1;

    [Header("Aiming")]
    public float turnSpeed = 10f;

    float lastShootTime = -999f;

    void Start()
    {
        if (!target)
        {
            // EnemyFollow가 있다면 그 타겟을 재사용
            var follow = GetComponent<EnemyFollow>();
            if (follow && follow.target) target = follow.target;
        }
        if (!target)
        {
            var t = GameObject.FindGameObjectWithTag(targetTag);
            if (t) target = t.transform;
        }
    }

    void Update()
    {
        if (!target || !projectilePrefab || !firePoint) return;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist > detectRange) return;

        // 조준 회전
        Vector3 to = target.position - transform.position;
        to.y = 0; // 수평 회전만
        if (to.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(to);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * turnSpeed);
        }

        // 사거리 및 시야 체크
        if (dist <= shootRange && Time.time - lastShootTime >= shootCooldown)
        {
            if (lineOfSightMask.value != 0)
            {
                if (!HasLineOfSight()) return;
            }

            Shoot();
            lastShootTime = Time.time;
        }
    }

    bool HasLineOfSight()
    {
        Vector3 origin = firePoint.position;
        Vector3 dir = (target.position - origin).normalized;
        float d = Vector3.Distance(origin, target.position);

        // 막는 레이어가 없으면 true
        if (lineOfSightMask.value == 0) return true;

        return !Physics.Raycast(origin, dir, d, lineOfSightMask);
    }

    void Shoot()
    {
        // 1) 타겟 방향으로 회전값 만들기 (자가발사형도 forward로 나가게)
        Vector3 dir = (target.position - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        // 2) 프리팹 생성
        var go = Instantiate(projectilePrefab, firePoint.position, rot);

        // 3) 네가 만든 Projectile(Fire 호출형) 우선 지원
        var p1 = go.GetComponent<Projectile>();
        if (p1 != null)
        {
            p1.owner = Projectile.OwnerType.Enemy; // 적 탄환
            p1.speed = projectileSpeed;            // Shooter 값으로 덮어쓰기
            p1.damage = projectileDamage;
            p1.Fire(dir);                           // ★ 반드시 Fire 호출
            return;                                 // 여기서 끝
        }

        // 4) 기존 TreasureProjectile(자가발사/Init 방식)도 그대로 지원
        var p2 = go.GetComponent<TreasureProjectile>();
        if (p2 != null)
        {
            // 자가발사 버전이면 회전만 맞춰도 알아서 이동
            // Init 방식이라면 아래 주석 해제해서 사용
            // p2.Init(dir, projectileSpeed, projectileDamage);
            return;
        }

        // 5) 혹시 아무 스크립트도 없으면(실수 방지용) 최소한 앞으로 나가게
        var rb = go.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.useGravity = false;
            rb.velocity = dir * projectileSpeed;
        }
        else
        {
            go.transform.position += dir * (projectileSpeed * Time.deltaTime);
        }
    }
}