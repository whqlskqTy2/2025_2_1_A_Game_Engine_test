using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerShop : MonoBehaviour
{
    public TowerInventory towerInventory;
    private TowerInventoryUI towerInventoryUI;

    [Header("타워 가격")]
    public int longRangePrice = 10;
    public int fastPrice = 8;
    public int aoePrice = 12;

    private void Awake()
    {
        if (towerInventory == null)
            towerInventory = FindObjectOfType<TowerInventory>();

        towerInventoryUI = FindObjectOfType<TowerInventoryUI>();
    }

    public void BuyLongRange() => TryBuyTower(TowerType.LongRange, longRangePrice);
    public void BuyFast() => TryBuyTower(TowerType.Fast, fastPrice);
    public void BuyAOE() => TryBuyTower(TowerType.AoE, aoePrice);

    private void TryBuyTower(TowerType type, int price)
    {
        if (GoldManager.Instance == null)
        {
            Debug.LogError("[TowerShop] GoldManager.Instance 가 없습니다.");
            return;
        }

        if (!GoldManager.Instance.SpendGold(price))
        {
            Debug.Log($"[TowerShop] 골드 부족! 필요: {price}, 현재: {GoldManager.Instance.gold}");
            return;
        }

        towerInventory.Add(type, 1);
        Debug.Log($"[TowerShop] {type} 타워 구매 성공! (가격: {price})");

        // ★ 보험용: 이벤트 말고 직접 UI 새로고침
        if (towerInventoryUI != null)
            towerInventoryUI.Refresh(towerInventory);
    }
}