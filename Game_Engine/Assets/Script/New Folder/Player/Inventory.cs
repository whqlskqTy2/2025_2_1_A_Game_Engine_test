using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;              //  꼭 추가

public class Inventory : MonoBehaviour
{
    public Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();
    InventoryUI invenUI;

    void Start()
    {
        invenUI = FindObjectOfType<InventoryUI>();
    }

    public int GetCount(ItemType id)
    {
        items.TryGetValue(id, out var count);
        return count;
    }
    // 인벤토리 변경 시 알려줄 이벤트
    public event Action<Inventory> OnInventoryChanged;

    public void Add(ItemType type, int count = 1)
    {
        if (!items.ContainsKey(type)) items[type] = 0;
        items[type] += count;
        Debug.Log($"[Inventory] +{count} {type} (총 {items[type]})");

        OnInventoryChanged?.Invoke(this);   //  여기 추가
    }

    public bool Consume(ItemType type, int count = 1)
    {
        if (!items.TryGetValue(type, out var have) || have < count) return false;
        items[type] = have - count;
        Debug.Log($"[Inventory] -{count} {type} (총 {items[type]})");
        if (items[type] == 0)
        {
            items.Remove(type);
            invenUI.selectedIndex = -1;
            invenUI.ResetSelection();
        }

        OnInventoryChanged?.Invoke(this);   // 여기도 있으면 좋음
        return true;
    }
}