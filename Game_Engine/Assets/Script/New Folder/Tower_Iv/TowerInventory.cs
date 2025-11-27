using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerInventory : MonoBehaviour
{
    public Dictionary<TowerType, int> items = new Dictionary<TowerType, int>();

    // UI에 알려줄 이벤트
    public event Action<TowerInventory> OnInventoryChanged;

    public void Add(TowerType type, int count = 1)
    {
        if (!items.ContainsKey(type)) items[type] = 0;
        items[type] += count;
        Debug.Log($"[TowerInventory] +{count} {type} (총 {items[type]})");

        //  여기 반드시 호출
        OnInventoryChanged?.Invoke(this);
    }

    public bool Consume(TowerType type, int count = 1)
    {
        if (!items.TryGetValue(type, out var have) || have < count)
            return false;

        items[type] = have - count;
        Debug.Log($"[TowerInventory] -{count} {type} (총 {items[type]})");

        if (items[type] == 0)
            items.Remove(type);

        //  여기도 호출
        OnInventoryChanged?.Invoke(this);
        return true;
    }
}