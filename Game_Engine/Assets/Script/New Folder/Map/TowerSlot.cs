using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    [Header("외부 참조")]
    public TowerInventory towerInventory;
    public TowerInventoryUI towerInventoryUI;
    public TowerDatabase towerDatabase;

    [Header("상태")]
    public bool isOccupied = false;
    public GameObject currentTower;

    private void OnMouseDown()
    {
        Debug.Log("[TowerSlot] 슬롯 클릭됨", this);

        if (isOccupied)
        {
            Debug.Log("[TowerSlot] 이미 타워가 설치된 슬롯입니다.", this);
            return;
        }

        if (towerInventory == null || towerInventoryUI == null || towerDatabase == null)
        {
            Debug.LogWarning("[TowerSlot] towerInventory / towerInventoryUI / towerDatabase 중 하나가 비어 있습니다.", this);
            return;
        }

        // 1) UI에서 선택된 타워 타입 가져오기
        if (!towerInventoryUI.TryGetSelectedTowerType(out TowerType selectedType))
        {
            Debug.Log("[TowerSlot] 선택된 타워가 없습니다. (단축키 8/9/0으로 선택해야 함)", this);
            return;
        }
        Debug.Log($"[TowerSlot] 선택된 타워 타입: {selectedType}", this);

        // 2) 인벤에서 1개 소비
        if (!towerInventory.Consume(selectedType, 1))
        {
            Debug.Log($"[TowerSlot] 인벤토리에 {selectedType} 타워 아이템이 없습니다.", this);
            return;
        }
        Debug.Log($"[TowerSlot] 인벤토리에서 {selectedType} 1개 소비 성공", this);

        // 3) 타입에 맞는 프리팹 찾기
        GameObject prefab = towerDatabase.GetPrefab(selectedType);
        if (prefab == null)
        {
            Debug.LogWarning($"[TowerSlot] TowerDatabase에 {selectedType} 프리팹이 없습니다.", this);
            return;
        }

        // 4) 타워 생성
        currentTower = Instantiate(prefab, transform.position, Quaternion.identity);
        isOccupied = true;

        Debug.Log($"[TowerSlot] 타워 설치 완료: {selectedType}", this);
    }
}