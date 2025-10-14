using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProJectlle : MonoBehaviour
{

    [Header("Projectile Settings")]
    public float speed = 20f;     // 이동 속도
    public float lifetime = 2f;   // 생존 시간 (초)
    public int damage = 1;        //  인스펙터에서 직접 설정 가능

    void Start()
    {
        // 일정 시간 뒤 자동 삭제
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 앞으로 이동
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enemy 컴포넌트가 있는지 확인
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // 적에게 데미지 전달
            enemy.TakeDamage(damage);

            // 투사체는 충돌 즉시 파괴
            Destroy(gameObject);
        }
    }
}
