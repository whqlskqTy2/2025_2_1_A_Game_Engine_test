using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // �� ����θ� �±�/Follow���� �ڵ� ����
    public string targetTag = "Treasure";

    [Header("Refs")]
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Detect")]
    public float detectRange = 18f;
    public float shootRange = 16f;
    public LayerMask lineOfSightMask = 0; // �ʿ� ������ 0(����)

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
            // EnemyFollow�� �ִٸ� �� Ÿ���� ����
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

        // ���� ȸ��
        Vector3 to = target.position - transform.position;
        to.y = 0; // ���� ȸ����
        if (to.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(to);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * turnSpeed);
        }

        // ��Ÿ� �� �þ� üũ
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

        // ���� ���̾ ������ true
        if (lineOfSightMask.value == 0) return true;

        return !Physics.Raycast(origin, dir, d, lineOfSightMask);
    }

    void Shoot()
    {
        // 1) Ÿ�� �������� ȸ���� ����� (�ڰ��߻����� forward�� ������)
        Vector3 dir = (target.position - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        // 2) ������ ����
        var go = Instantiate(projectilePrefab, firePoint.position, rot);

        // 3) �װ� ���� Projectile(Fire ȣ����) �켱 ����
        var p1 = go.GetComponent<Projectile>();
        if (p1 != null)
        {
            p1.owner = Projectile.OwnerType.Enemy; // �� źȯ
            p1.speed = projectileSpeed;            // Shooter ������ �����
            p1.damage = projectileDamage;
            p1.Fire(dir);                           // �� �ݵ�� Fire ȣ��
            return;                                 // ���⼭ ��
        }

        // 4) ���� TreasureProjectile(�ڰ��߻�/Init ���)�� �״�� ����
        var p2 = go.GetComponent<TreasureProjectile>();
        if (p2 != null)
        {
            // �ڰ��߻� �����̸� ȸ���� ���絵 �˾Ƽ� �̵�
            // Init ����̶�� �Ʒ� �ּ� �����ؼ� ���
            // p2.Init(dir, projectileSpeed, projectileDamage);
            return;
        }

        // 5) Ȥ�� �ƹ� ��ũ��Ʈ�� ������(�Ǽ� ������) �ּ��� ������ ������
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