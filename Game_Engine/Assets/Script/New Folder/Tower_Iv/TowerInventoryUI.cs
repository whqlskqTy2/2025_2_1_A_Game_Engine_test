using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerInventoryUI : MonoBehaviour
{
    [Header("슬롯들")]
    public List<Transform> slots;
    [Header("슬롯에 넣을 아이템 프리팹")]
    public GameObject slotItemPrefab;

    [Header("타워 아이콘")]
    public Sprite longRangeIcon;
    public Sprite fastIcon;
    public Sprite aoeIcon;

    [Header("타워 인벤토리")]
    public TowerInventory targetInventory;

    private List<GameObject> spawnedItems = new List<GameObject>();
    public int selectedIndex = -1;

    private void Awake()
    {
        // 인스펙터에서 안 넣었으면 자동으로 TowerInventory 찾기
        if (targetInventory == null)
            targetInventory = FindObjectOfType<TowerInventory>();
    }

    private void OnEnable()
    {
        if (targetInventory != null)
        {
            targetInventory.OnInventoryChanged += Refresh;
            Refresh(targetInventory);
        }
    }

    private void OnDisable()
    {
        if (targetInventory != null)
            targetInventory.OnInventoryChanged -= Refresh;
    }

    public void Refresh(TowerInventory inven)
    {
        if (inven == null)
        {
            Debug.LogError("[TowerInventoryUI] Refresh 호출됐는데 inven 이 null 입니다.");
            return;
        }

        if (slots == null || slots.Count == 0)
        {
            Debug.LogError("[TowerInventoryUI] slots 가 비어있습니다. 인스펙터에서 슬롯들을 연결했는지 확인하세요.");
            return;
        }

        if (slotItemPrefab == null)
        {
            Debug.LogError("[TowerInventoryUI] slotItemPrefab 이 설정되지 않았습니다.");
            return;
        }

        // 기존 UI 삭제
        foreach (var go in spawnedItems)
            if (go != null) Destroy(go);
        spawnedItems.Clear();

        selectedIndex = -1;
        ResetSlotColors();

        int i = 0;
        foreach (var pair in inven.items)
        {
            if (i >= slots.Count) break;

            var slotTf = slots[i];
            if (slotTf == null)
            {
                Debug.LogError($"[TowerInventoryUI] slots[{i}] 가 null 입니다. Panel 안의 슬롯 오브젝트를 제대로 넣었는지 확인하세요.");
                i++;
                continue;
            }

            Debug.Log($"[TowerInventoryUI] 슬롯 {i} 에 {pair.Key} x{pair.Value} 표시", slotTf);

            // 프리팹 생성
            var go = Instantiate(slotItemPrefab, slotTf);
            if (go == null)
            {
                Debug.LogError("[TowerInventoryUI] Instantiate 결과가 null 입니다. slotItemPrefab 을 확인하세요.");
                i++;
                continue;
            }

            go.name = $"TowerSlotItem_{pair.Key}";
            var rt = go.transform as RectTransform;
            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
            }

            spawnedItems.Add(go);

            // 프리팹 안의 UI 컴포넌트
            var ui = go.GetComponent<TowerSlotItemUI>();
            if (ui == null)
            {
                Debug.LogError("[TowerInventoryUI] slotItemPrefab 에 TowerSlotItemUI 컴포넌트가 없습니다.", go);
                i++;
                continue;
            }

            Sprite icon = null;
            switch (pair.Key)
            {
                case TowerType.LongRange: icon = longRangeIcon; break;
                case TowerType.Fast: icon = fastIcon; break;
                case TowerType.AoE: icon = aoeIcon; break;
            }

            if (icon == null)
            {
                Debug.LogWarning($"[TowerInventoryUI] {pair.Key} 에 연결된 아이콘이 없습니다. 인스펙터에서 아이콘 스프라이트를 넣어주세요.", this);
            }

            ui.Setup(icon, pair.Value, pair.Key);

            i++;
        }

        Debug.Log($"[TowerInventoryUI] Refresh 끝, 생성된 UI 수: {spawnedItems.Count}");
    }

    // ↓ 단축키 부분은 그대로 두면 됨
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetSelectedIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SetSelectedIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha0)) SetSelectedIndex(2);
    }

    public void SetSelectedIndex(int index)
    {
        ResetSlotColors();

        if (index < 0 || index >= spawnedItems.Count)
        {
            selectedIndex = -1;
            return;
        }

        slots[index].GetComponent<Image>().color = Color.yellow;
        selectedIndex = index;
    }

    private void ResetSlotColors()
    {
        foreach (var s in slots)
        {
            var img = s.GetComponent<Image>();
            if (img != null) img.color = Color.white;
        }
    }

    public bool TryGetSelectedTowerType(out TowerType type)
    {
        type = default;

        if (selectedIndex < 0 || selectedIndex >= spawnedItems.Count)
            return false;

        var ui = spawnedItems[selectedIndex].GetComponent<TowerSlotItemUI>();
        if (ui == null) return false;

        type = ui.towerType;
        return true;
    }
}