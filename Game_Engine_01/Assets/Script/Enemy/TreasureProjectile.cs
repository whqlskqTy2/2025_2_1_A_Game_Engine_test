using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class TreasureProjectile : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 18f;
    public float lifetime = 4f;

    [Header("Damage")]
    public int damage = 1;

    Rigidbody rb; // �־ �ǰ� ��� ��

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);

        // Rigidbody ���� ������, ������ Transform��
        if (rb && !rb.isKinematic)
        {
            rb.useGravity = false;
            rb.velocity = transform.forward * speed;
        }
    }

    void Update()
    {
        // Rigidbody �� ���� ��쿡�� ���� �̵�
        if (!rb || rb.isKinematic)
            transform.position += transform.forward * speed * Time.deltaTime;
    }

    // �ʿ�� �浹 ó�� (IDamageable ȣ��)
    void OnTriggerEnter(Collider other)
    {
        var dmg = other.GetComponentInParent<IDamageable>() ?? other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
    }
}