using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHarvester : MonoBehaviour
{
    public float rayDistance = 5f;
    public LayerMask hitMask = ~0;
    public int toolDamage = 1;
    public float hitCooldown = 0.15f;

    private float _nextHitTime;
    private Camera _cam;
    public Inventory inventory;
    InventoryUI invenUI;

    void Awake()
    {
        _cam = Camera.main;

   
        if (inventory == null)
            inventory = GetComponent<Inventory>();

        invenUI = FindAnyObjectByType<InventoryUI>();
    }
    void Update()
    {
        // -------------------------------
        // 1) 블록 채굴 (왼쪽 클릭)
        // -------------------------------
        if (Input.GetMouseButton(0) && Time.time >= _nextHitTime)
        {
            // 선택된 슬롯이 없다면 → 채굴모드
            if (invenUI.selectedIndex < 0)
            {
                _nextHitTime = Time.time + hitCooldown;

                Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.Raycast(ray, out var hit, rayDistance, hitMask))
                {
                    var block = hit.collider.GetComponent<Block>();
                    if (block != null)
                    {
                        block.Hit(toolDamage, inventory);
                    }
                }
            }
        }

        // -------------------------------
        // 2) 블록 설치 (오른쪽 클릭)
        // -------------------------------
        if (Input.GetMouseButtonDown(1))
        {
            // 선택된 슬롯이 없으면 설치 불가
            if (invenUI.selectedIndex < 0)
                return;

            Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out var hit, rayDistance, hitMask))
            {
                Vector3Int placePos = AdjacentCellOnHitFace(hit);

                ItemType selected = invenUI.GetInventorySlot();

                // 인벤토리에서 1개 소비 성공하면 설치
                if (inventory.Consume(selected, 1))
                {
                    FindObjectOfType<NoiseVoxelMap2>().PlaceTile(placePos, selected);

                    // UI 갱신
                    invenUI.UpdateInventory(inventory);
                }
            }
        }
    }

    // -------------------------------
    // ■ 설치 위치 계산 함수
    // -------------------------------
    static Vector3Int AdjacentCellOnHitFace(RaycastHit hit)
    {
        // 맞은 블록의 중심 좌표
        Vector3 baseCenter = hit.collider.transform.position;

        // 부딪힌 면 방향으로 1칸 이동
        Vector3 adjCenter = baseCenter + hit.normal;

        // 그리드(정수 좌표)로 변환
        return Vector3Int.RoundToInt(adjCenter);
    }
}