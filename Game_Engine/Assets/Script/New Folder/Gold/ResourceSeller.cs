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

    private void Start()
    {
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerHarvester>().inventory;
            Debug.Log("[ResourceSeller] 인벤토리를 자동으로 PlayerHarvester에서 가져옴.");
        }
    }
    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(sellKey))
        {
            SellAllResources();
        }
        if (playerInventory != null)
            Debug.Log($"[체크] ResourceSeller가 보고 있는 인벤 수량: {playerInventory.items.Count}");

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
        Debug.Log("[Sell] SellAllResources 호출됨.");

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

        var snapshot = new List<KeyValuePair<ItemType, int>>(playerInventory.items);
        Debug.Log($"[Sell] 현재 인벤토리 아이템 개수: {snapshot.Count}");

        foreach (var pair in snapshot)
        {
            ItemType type = pair.Key;
            int count = pair.Value;

            Debug.Log($"[Sell] 루프 진입: {type} x{count}");

            if (count <= 0) continue;
            int price = GetPrice(type);
            if (price <= 0) continue;

            int gain = price * count;
            totalGold += gain;

            playerInventory.Consume(type, count);

            Debug.Log($"[Sell] {type} x{count} → {gain} Gold");
        }

        Debug.Log($"[Sell] 총 골드 획득: {totalGold}");

        // 여기서 무조건 AddGold 호출
        GoldManager.Instance.AddGold(totalGold);
        Debug.Log("[Sell] AddGold 호출 완료");

        if (inventoryUI != null)
        {
            inventoryUI.UpdateInventory(playerInventory);
        }
    }
    private int GetPrice(ItemType type)
    {
        switch (type)
        {
            case ItemType.Dirt: return dirtPrice;
            case ItemType.Grass: return grassPrice;
            case ItemType.Water: return waterPrice;
            default: return 0;
        }
    }

}
