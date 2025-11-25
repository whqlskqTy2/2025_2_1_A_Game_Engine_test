using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    [Header("플레이어 골드")]
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
        Debug.Log($"[Gold] +{amount} → 현재 골드: {gold}");
        // 나중에 골드 UI 있으면 여기서 갱신 호출
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount)
        {
            Debug.Log("[Gold] 골드 부족");
            return false;
        }

        gold -= amount;
        Debug.Log($"[Gold] -{amount} → 현재 골드: {gold}");
        return true;
    }
}