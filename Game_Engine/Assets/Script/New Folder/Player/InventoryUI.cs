using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("슬롯 관련 설정")]
    public List<Transform> Slot;
    public GameObject SlotItem;

    // 실제 슬롯에 생성된 아이템들
    private List<GameObject> items = new List<GameObject>();

    [Header("아이템 아이콘 설정")]
    public Sprite dirtIcon;
    public Sprite grassIcon;
    public Sprite waterIcon;

    // 인벤토리 참조
    public Inventory targetInventory;

    public int selectedIndex = -1;

    void OnEnable()
    {
        if (targetInventory != null)
        {
            targetInventory.OnInventoryChanged += UpdateInventory;
            UpdateInventory(targetInventory);
        }
    }

    void OnDisable()
    {
        if (targetInventory != null)
            targetInventory.OnInventoryChanged -= UpdateInventory;
    }

    // UI 다시 그리기
    public void UpdateInventory(Inventory myInven)
    {
        // 기존 슬롯 UI 제거
        foreach (var slotItem in items)
            Destroy(slotItem);
        items.Clear();
        selectedIndex = -1;
        ResetSelection();

        int idx = 0;
        foreach (var item in myInven.items)
        {
            if (idx >= Slot.Count) break;

            var go = Instantiate(SlotItem, Slot[idx]);
            go.transform.localPosition = Vector3.zero;
            items.Add(go);

            SlotItemPrefab sItem = go.GetComponent<SlotItemPrefab>();

            Sprite icon = null;

            // 아이템 수량 표시 (이름 대신)
            string label = item.Value.ToString();

            switch (item.Key)
            {
                case BlockType.Dirt: icon = dirtIcon; break;
                case BlockType.Grass: icon = grassIcon; break;
                case BlockType.Water: icon = waterIcon; break;
            }

            if (sItem != null)
                sItem.ItemSetting(icon, label, item.Key);

            idx++;
        }
    }

    void Update()
    {
        for (int i = 0; i < Mathf.Min(9, Slot.Count); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SetSelectedIndex(i);
        }
    }

    public void SetSelectedIndex(int idx)
    {
        ResetSelection();

        // 같은 슬롯 다시 누르면 선택 해제
        if (selectedIndex == idx)
        {
            selectedIndex = -1;
            return;
        }

        // 슬롯에 아이템이 없으면 선택 불가
        if (idx >= items.Count)
        {
            selectedIndex = -1;
            return;
        }

        SetSelection(idx);
        selectedIndex = idx;
    }

    public void ResetSelection()
    {
        foreach (var slot in Slot)
        {
            slot.GetComponent<Image>().color = Color.white;
        }
    }

    public void SetSelection(int idx)
    {
        Slot[idx].GetComponent<Image>().color = Color.yellow;
    }

    public BlockType GetInventorySlot()
    {
        return items[selectedIndex]
            .GetComponent<SlotItemPrefab>()
            .blockType;
    }
}
