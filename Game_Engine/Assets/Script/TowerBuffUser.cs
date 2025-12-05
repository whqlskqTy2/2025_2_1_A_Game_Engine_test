using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuffUser : MonoBehaviour
{
    [Header("참조")]
    public Camera cam;              // 시점 카메라 (없으면 Awake에서 Camera.main)
    public Inventory inventory;     // 플레이어 인벤
    public InventoryUI inventoryUI; // 인벤 UI (슬롯 선택 정보)

    [Header("버프 설정")]
    public float speedMultiplier = 2f;   // 공속 배율
    public float buffDuration = 5f;      // 지속 시간
    public KeyCode useKey = KeyCode.E;   // 사용 키

    [Header("타워 인식 설정")]
    public float useRange = 20f;         // 사용 가능 거리
    public LayerMask towerMask = ~0;     // 타워 레이어(일단 전체 ~0)

    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
            Debug.Log("[TowerBuffUser] Camera 자동 할당: " + cam);
        }

        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
            Debug.Log("[TowerBuffUser] Inventory 자동 할당: " + inventory);
        }

        if (inventoryUI == null)
        {
            inventoryUI = FindObjectOfType<InventoryUI>();
            Debug.Log("[TowerBuffUser] InventoryUI 자동 할당: " + inventoryUI);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(useKey))
        {
            Debug.Log("[TowerBuffUser] 사용 키 입력됨(E)");
            TryUseSpeedItemOnTower();
        }
    }

    void TryUseSpeedItemOnTower()
    {
        if (inventory == null)
        {
            Debug.LogWarning("[TowerBuffUser] inventory 가 없음");
            return;
        }

        if (inventoryUI == null)
        {
            Debug.LogWarning("[TowerBuffUser] inventoryUI 가 없음");
            return;
        }

        Debug.Log("[TowerBuffUser] 현재 selectedIndex = " + inventoryUI.selectedIndex);

        // 슬롯 선택 안 되어 있으면 사용 불가
        if (inventoryUI.selectedIndex < 0)
        {
            Debug.Log("[TowerBuffUser] 선택된 인벤 슬롯이 없음");
            return;
        }

        // 현재 슬롯 아이템 타입 가져오기
        ItemType type = inventoryUI.GetInventorySlot();
        Debug.Log("[TowerBuffUser] 선택된 아이템 타입: " + type);

        if (type != ItemType.TowerSpeedUp)
        {
            Debug.Log("[TowerBuffUser] 선택된 아이템이 TowerSpeedUp 이 아님");
            return;
        }

        if (cam == null)
        {
            Debug.LogWarning("[TowerBuffUser] cam 이 없음");
            return;
        }

        // 카메라 중앙에서 레이 쏴서 타워 맞추기
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (!Physics.Raycast(ray, out RaycastHit hit, useRange, towerMask))
        {
            Debug.Log("[TowerBuffUser] 레이캐스트 결과: 아무것도 맞지 않음");
            return;
        }

        Debug.Log("[TowerBuffUser] 레이캐스트 히트: " + hit.collider.name);

        // 맞은 오브젝트에서 Tower 찾기 (부모까지)
        Tower tower = hit.collider.GetComponent<Tower>();
        if (tower == null)
            tower = hit.collider.GetComponentInParent<Tower>();

        if (tower == null)
        {
            Debug.Log("[TowerBuffUser] 맞은 오브젝트에 Tower 컴포넌트 없음");
            return;
        }

        Debug.Log("[TowerBuffUser] 대상 타워: " + tower.name);

        // 인벤토리에서 버프 아이템 1개 소비
        bool consumed = inventory.Consume(type, 1);
        Debug.Log("[TowerBuffUser] Consume 결과: " + consumed);

        if (!consumed)
        {
            Debug.Log("[TowerBuffUser] 인벤토리에 TowerSpeedUp 이 충분하지 않음");
            return;
        }

        // 타워에 공속 버프 적용
        tower.ApplyAttackSpeedBuff(speedMultiplier, buffDuration);
        Debug.Log("[TowerBuffUser] 타워에 공속 버프 적용 완료!");
    }
}