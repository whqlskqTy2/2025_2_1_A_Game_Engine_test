using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Detect")]
    public float detectRange = 18f;
    public float shootRange = 16f;
    public LayerMask lineOfSightMask;   // ��/���� ���̾�

    [Header("Shooting")]
    public float shootCooldown = 1.0f;
    public float projectileSpeed = 18f;
    public int projectileDamage = 1;

    [Header("Aiming")]
    public float turnSpeed = 10f;       // �÷��̾� ������ ȸ�� �ӵ�

    Transform player;
    float lastShootTime = -999f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > detectRange) return;

        // �ٶ󺸱�
        Vector3 toPlayer = (player.position - transform.position);
        Vector3 flat = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (flat.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(flat.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * turnSpeed);
        }

        // ��Ÿ� & �þ� üũ
        if (dist <= shootRange && Time.time - lastShootTime >= shootCooldown)
        {
            if (HasLineOfSight())
            {
                lastShootTime = Time.time;
                ShootAtPlayer();
            }
        }
    }

    bool HasLineOfSight()
    {
        if (!firePoint) return false;
        Vector3 dir = (player.position + Vector3.up * 1.2f) - firePoint.position;
        if (Physics.Raycast(firePoint.position, dir.normalized, out RaycastHit hit, shootRange, lineOfSightMask, QueryTriggerInteraction.Ignore))
        {
            // ���ο������Ʈ ����
            return false;
        }
        return true;
    }

    void ShootAtPlayer()
    {
        if (!projectilePrefab || !firePoint) return;

        Vector3 targetPos = player.position + Vector3.up * 1.2f;
        Vector3 dir = (targetPos - firePoint.position).normalized;

        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
        var proj = go.GetComponent<Projectile>();
        if (proj)
        {
            proj.owner = Projectile.OwnerType.Enemy;
            proj.speed = projectileSpeed;
            proj.damage = projectileDamage;
            proj.Fire(dir);
        }
    }
}