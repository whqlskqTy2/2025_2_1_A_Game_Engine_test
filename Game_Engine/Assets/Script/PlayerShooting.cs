using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string name;
        public GameObject projectilePrefab; // 발사체
        public float projectileSpeed = 20f; // 속도
        public float lifetime = 2f;         // 생존 시간
        public int damage = 1;              // 무기별 데미지
    }

    [Header("Weapon Settings")]
    public List<Weapon> weapons = new List<Weapon>(); // 무기 리스트
    public Transform firePoint;                       // 발사 위치

    private Camera cam;
    private int currentWeaponIndex = 0;
    private Weapon CurrentWeapon => weapons[currentWeaponIndex];

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // 좌클릭 → 발사
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        // Z → 무기 교체
        if (Input.GetKeyDown(KeyCode.Z))
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
            Debug.Log("무기 변경: " + CurrentWeapon.name);
        }
    }

    void Shoot()
    {
        if (CurrentWeapon == null || CurrentWeapon.projectilePrefab == null) return;

        // 화면 마우스 → Ray
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = ray.GetPoint(50f);
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        // Projectile 생성
        GameObject proj = Instantiate(
            CurrentWeapon.projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        
      

        // Rigidbody 물리 이동을 사용하는 경우
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * CurrentWeapon.projectileSpeed;
        }

        // 일정 시간 후 삭제 (projScript에서 Destroy 관리 중이면 생략 가능)
        Destroy(proj, CurrentWeapon.lifetime);
    }
}
