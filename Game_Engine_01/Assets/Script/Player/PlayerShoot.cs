using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Refs")]
    public Transform cameraPivot;         // ī�޶�(�Ǵ� Camera.main.transform)
    public Transform firePoint;           // �ѱ� ��ġ (ī�޶� �� �ణ)
    public GameObject projectilePrefab;   // �߻�ü ������

    [Header("Shooting")]
    public float projectileSpeed = 22f;
    public int projectileDamage = 1;
    public float shootCooldown = 0.18f;
    public bool holdToFire = true;        // true: �� ������ ����, false: Ŭ������ 1��

    float lastShootTime = -999f;

    void Update()
    {
        bool wantToShoot = holdToFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        if (!wantToShoot) return;

        if (Time.time - lastShootTime < shootCooldown) return;
        lastShootTime = Time.time;

        Shoot();
    }

    void Shoot()
    {
        if (!projectilePrefab || !firePoint || !cameraPivot) return;

        //  ī�޶� �������� Ray ���� ��Ʈ ����Ʈ�� ã�´�
        Vector3 shootDir = cameraPivot.forward;
        Vector3 targetPoint;

        Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, ~0, QueryTriggerInteraction.Ignore))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = cameraPivot.position + cameraPivot.forward * 500f;
        }

        //  firePoint���� �� ������ ���� ���� ���� (�з����� ����)
        shootDir = (targetPoint - firePoint.position).normalized;

        //  ź ���� + ���� ȸ�� ����
        GameObject go = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDir));

        //  �ڱ� �ڽŰ��� �浹 ���� 
        var myCol = GetComponentInParent<CharacterController>();
        if (go.TryGetComponent<Collider>(out var projCol) && myCol)
        {
            Physics.IgnoreCollision(projCol, myCol, true);
        }

        //  Rigidbody �ӵ� �ο�
        if (go.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = shootDir * projectileSpeed;
            rb.useGravity = false; // ���Ѵٸ� ���� ź����
        }

        //  ������ ���� ���� (����)
        if (go.TryGetComponent<Projectile>(out var proj))
        {
            proj.damage = projectileDamage;
        }

        //  ����� �ð�ȭ (Scene���� Ȯ�ο�)
        Debug.DrawRay(cameraPivot.position, cameraPivot.forward * 5f, Color.yellow, 1.0f);
        Debug.DrawRay(firePoint.position, shootDir * 5f, Color.cyan, 1.0f);
    }
}