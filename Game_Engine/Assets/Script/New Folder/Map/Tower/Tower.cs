using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("공격 설정")]
    public float attackRange = 5f;   // 사거리
    public float fireRate = 1f;      // 초당 공격 횟수
    public int damage = 3;           // 한 번 공격 시 대미지

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
        // 사거리 안의 모든 콜라이더를 찾는다.
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        EnemyStatus closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            EnemyStatus enemy = hit.GetComponent<EnemyStatus>();
            if (enemy == null) continue; // 적이 아니면 무시

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

        // 여기서는 그냥 즉시 대미지
        target.TakeDamage(damage);

        // TODO: 나중에 여기서 이펙트, 총알 프리팹, 사운드 재생 등 추가
    }

    // 씬에서 사거리 보이게
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}