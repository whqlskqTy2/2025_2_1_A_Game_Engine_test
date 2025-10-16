using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Refs")]
    public Transform cameraPivot;         // 카메라(또는 Camera.main.transform)
    public Transform firePoint;           // 총구 위치 (카메라 앞 약간)
    public GameObject projectilePrefab;   // 발사체 프리팹

    [Header("Shooting")]
    public float projectileSpeed = 22f;
    public int projectileDamage = 1;
    public float shootCooldown = 0.18f;
    public bool holdToFire = true;        // true: 꾹 누르면 연사, false: 클릭마다 1발

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

        //  카메라 기준으로 Ray 쏴서 히트 포인트를 찾는다
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

        //  firePoint에서 그 지점을 향해 방향 재계산 (패럴럭스 보정)
        shootDir = (targetPoint - firePoint.position).normalized;

        //  탄 생성 + 방향 회전 적용
        GameObject go = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDir));

        //  자기 자신과의 충돌 방지 
        var myCol = GetComponentInParent<CharacterController>();
        if (go.TryGetComponent<Collider>(out var projCol) && myCol)
        {
            Physics.IgnoreCollision(projCol, myCol, true);
        }

        //  Rigidbody 속도 부여
        if (go.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = shootDir * projectileSpeed;
            rb.useGravity = false; // 원한다면 직선 탄도로
        }

        //  데미지 전달 세팅 (선택)
        if (go.TryGetComponent<Projectile>(out var proj))
        {
            proj.damage = projectileDamage;
        }

        //  디버그 시각화 (Scene에서 확인용)
        Debug.DrawRay(cameraPivot.position, cameraPivot.forward * 5f, Color.yellow, 1.0f);
        Debug.DrawRay(firePoint.position, shootDir * 5f, Color.cyan, 1.0f);
    }
}