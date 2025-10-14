using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string name;
        public GameObject projectilePrefab; // �߻�ü
        public float projectileSpeed = 20f; // �ӵ�
        public float lifetime = 2f;         // ���� �ð�
        public int damage = 1;              // ���⺰ ������
    }

    [Header("Weapon Settings")]
    public List<Weapon> weapons = new List<Weapon>(); // ���� ����Ʈ
    public Transform firePoint;                       // �߻� ��ġ

    private Camera cam;
    private int currentWeaponIndex = 0;
    private Weapon CurrentWeapon => weapons[currentWeaponIndex];

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // ��Ŭ�� �� �߻�
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        // Z �� ���� ��ü
        if (Input.GetKeyDown(KeyCode.Z))
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
            Debug.Log("���� ����: " + CurrentWeapon.name);
        }
    }

    void Shoot()
    {
        if (CurrentWeapon == null || CurrentWeapon.projectilePrefab == null) return;

        // ȭ�� ���콺 �� Ray
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = ray.GetPoint(50f);
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        // Projectile ����
        GameObject proj = Instantiate(
            CurrentWeapon.projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        
      

        // Rigidbody ���� �̵��� ����ϴ� ���
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * CurrentWeapon.projectileSpeed;
        }

        // ���� �ð� �� ���� (projScript���� Destroy ���� ���̸� ���� ����)
        Destroy(proj, CurrentWeapon.lifetime);
    }
}
