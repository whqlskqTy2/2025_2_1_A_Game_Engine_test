using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyExample : MonoBehaviour
{
    public Health health;
    public GameObject deathVFX;

    void Awake()
    {
        if (!health) health = GetComponent<Health>();
        health.onDeath.AddListener(OnDead);
    }

    void OnDead()
    {
        if (deathVFX) Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
