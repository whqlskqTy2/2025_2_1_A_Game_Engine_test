using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSeller : MonoBehaviour
{
    [Header("참조")]
    public Inventory playerInventory;     // 플레이어 Inventory
    public InventoryUI inventoryUI;       // 인벤 UI (선택, 없으면 비워도 됨)

    [Header("판매 단가 (1개당 골드)")]
    public int dirtPrice = 1;
    public int grassPrice = 2;
    public int waterPrice = 3;

    [Header("상점 상호작용 키")]
    public KeyCode sellKey = KeyCode.F;

    private bool playerInRange = false;

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(sellKey))
        {
            SellAllResources();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("상점 범위 진입: F 키로 자원 판매 가능");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("상점 범위 이탈");
        }
    }

    public void SellAllResources()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("ResourceSeller: playerInventory 가 설정되지 않았습니다.");
            return;
        }

        if (GoldManager.Instance == null)
        {
            Debug.LogWarning("ResourceSeller: GoldManager 인스턴스가 없습니다.");
            return;
        }

        int totalGold = 0;

        // 딕셔너리 수정하면서 돌면 에러나니까, 키를 따로 복사
        List<BlockType> keys = new List<BlockType>(playerInventory.items.Keys);

        foreach (BlockType type in keys)
        {
            int count = playerInventory.items[type];
            if (count <= 0) continue;

            int price = GetPrice(type);
            if (price <= 0) continue;

            int gain = price * count;
            totalGold += gain;

            // 인벤토리에서 해당 타입 제거
            playerInventory.items.Remove(type);
            Debug.Log($"[Sell] {type} x{count} → {gain} Gold");
        }

        if (totalGold > 0)
        {
            GoldManager.Instance.AddGold(totalGold);
        }
        else
        {
            Debug.Log("[Sell] 판매할 자원이 없습니다.");
        }

        // 인벤토리 UI만 다시 그린다 (Inventory 이벤트는 내부 Add/Consume 때만 실행)
        if (inventoryUI != null)
        {
            inventoryUI.UpdateInventory(playerInventory);
        }
    }

    private int GetPrice(BlockType type)
    {
        switch (type)
        {
            case BlockType.Dirt: return dirtPrice;
            case BlockType.Grass: return grassPrice;
            case BlockType.Water: return waterPrice;
            default: return 0;
        }
    }
}