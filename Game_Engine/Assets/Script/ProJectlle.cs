using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProJectlle : MonoBehaviour
{

    [Header("Projectile Settings")]
    public float speed = 20f;     // �̵� �ӵ�
    public float lifetime = 2f;   // ���� �ð� (��)
    public int damage = 1;        //  �ν����Ϳ��� ���� ���� ����

    void Start()
    {
        // ���� �ð� �� �ڵ� ����
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // ������ �̵�
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enemy ������Ʈ�� �ִ��� Ȯ��
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // ������ ������ ����
            enemy.TakeDamage(damage);

            // ����ü�� �浹 ��� �ı�
            Destroy(gameObject);
        }
    }
}
