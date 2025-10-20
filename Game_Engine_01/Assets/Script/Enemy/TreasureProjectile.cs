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

    [Header("FX (optional)")]
    public GameObject hitEffect;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // 4초 뒤 자동 삭제
        Destroy(gameObject, lifetime);

        // Rigidbody가 있으면 속도 부여
        if (rb && !rb.isKinematic)
        {
            rb.useGravity = false;
            rb.velocity = transform.forward * speed;
        }
    }

    void Update()
    {
        // Rigidbody를 안 쓰면 Transform 이동
        if (!rb || rb.isKinematic)
            transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // TreasureHealth만 찾음
        TreasureHealth treasure = other.GetComponentInParent<TreasureHealth>();
        if (treasure != null)
        {
            treasure.TakeDamage(damage, transform.position);

            if (hitEffect)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}