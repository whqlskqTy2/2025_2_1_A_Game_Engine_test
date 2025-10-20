using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTreasure : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                 // �� ���� ���� Transform ����
    public string treasureTag = "Treasure";  // target ��������� �±׷� ã��

    [Header("Attack")]
    public GameObject projectilePrefab;
    public Transform muzzlePoint;
    public float projectileSpeed = 12f;
    public float attackRange = 10f;
    public float attackCooldown = 1.2f;
    public int damagePerShot = 1;

    float lastAttackTime = -999f;

    void Start()
    {
        if (target == null)
        {
            GameObject t = GameObject.FindGameObjectWithTag(treasureTag);
            if (t) target = t.transform;
        }
    }

    void Update()
    {
        if (!target) return;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            ShootAtTarget();
            lastAttackTime = Time.time;
        }
    }

    void ShootAtTarget()
    {
        if (!projectilePrefab || !muzzlePoint || !target) return;

        GameObject go = Instantiate(projectilePrefab, muzzlePoint.position, Quaternion.identity);
        var proj = go.GetComponent<TreasureProjectile>();
        if (proj == null) proj = go.AddComponent<TreasureProjectile>(); // ������ �ڵ��߰�

       
    }
}
