using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("슬롯 관련 설정")]
    public List<Transform> Slot;
    public GameObject SlotItem;

    private List<GameObject> items = new List<GameObject>();

    [Header("아이템 아이콘 설정")]
    public Sprite dirtIcon;
    public Sprite grassIcon;
    public Sprite waterIcon;
    public Sprite diamondIcon;

    //  인벤토리 참조
    public Inventory targetInventory;

    void OnEnable()
    {
        if (targetInventory != null)
        {
            targetInventory.OnInventoryChanged += UpdateInventory;
            UpdateInventory(targetInventory);   // 시작할 때 한 번 그리기
        }
    }

    void OnDisable()
    {
        if (targetInventory != null)
            targetInventory.OnInventoryChanged -= UpdateInventory;
    }

    public void UpdateInventory(Inventory myInven)
    {
        // 기존 슬롯 삭제
        foreach (var slotItem in items)
        {
            Destroy(slotItem);
        }
        items.Clear();

        int idx = 0;
        foreach (var item in myInven.items)
        {
            if (idx >= Slot.Count) break;

            var go = Instantiate(SlotItem, Slot[idx].transform);
            go.transform.localPosition = Vector3.zero;
            items.Add(go);

            SlotItemPrefab sItem = go.GetComponent<SlotItemPrefab>();

            Sprite icon = null;
            string label = "";

            switch (item.Key)
            {
                case BlockType.Dirt:
                    icon = dirtIcon;
                    label = "Dirt";
                    break;
                case BlockType.Grass:
                    icon = grassIcon;
                    label = "Grass";
                    break;
                case BlockType.Water:
                    icon = waterIcon;
                    label = "Water";
                    break;
            }

            if (sItem != null)
                sItem.ItemSetting(icon, label);

            idx++;
        }
    }
}