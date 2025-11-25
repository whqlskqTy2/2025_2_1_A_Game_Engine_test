using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    [Header("ÇÃ·¹ÀÌ¾î °ñµå")]
    public int gold = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[Gold] +{amount} ¡æ ÇöÀç °ñµå: {gold}");
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount)
        {
            Debug.Log("[Gold] °ñµå ºÎÁ·");
            return false;
        }

        gold -= amount;
        Debug.Log($"[Gold] -{amount} ¡æ ÇöÀç °ñµå: {gold}");
        return true;
    }
}