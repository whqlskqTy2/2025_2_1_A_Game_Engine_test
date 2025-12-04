using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingPanel : MonoBehaviour
{
    public Inventory inventory;
    public List<CraftingRecipe> recipeList;
    public GameObject root;
    public TMP_Text plannedText;
    public Button craftButton;
    public Button clearButton;
    public TMP_Text hintText;

    readonly Dictionary<ItemType, int> planned = new();

    bool isOpen;

    void Start()
    {
        SetOpen(false);
        craftButton.onClick.AddListener(DoCraft);
        clearButton.onClick.AddListener(ClearPlanned);
        RefreshPlannedUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetOpen(!isOpen);
        }
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
        if (root)
            root.SetActive(open);

        if (!open)
            ClearPlanned();
    }

    public void AddPlanned(ItemType type, int count)
    {
        if (!planned.ContainsKey(type))
            planned[type] = 0;

        planned[type] += count;

        RefreshPlannedUI();
        SetHint($"{type} x{count} 추가 완료");
    }

    public void ClearPlanned()
    {
        planned.Clear();
        RefreshPlannedUI();
        SetHint("초기화 완료");
    }

    void RefreshPlannedUI()
    {
        if (!plannedText)
            return;

        if (planned.Count == 0)
        {
            plannedText.text = "우클릭으로 재료를 추가하세요.";
            return;
        }

        var sb = new StringBuilder();

        foreach (var item in planned)
            sb.AppendLine($"{item.Key} x{item.Value}");

        plannedText.text = sb.ToString();
    }

    void SetHint(string msg)
    {
        if (hintText)
            hintText.text = msg;
    }

    void DoCraft()
    {
        if (planned.Count == 0)
        {
            SetHint("재료가 부족합니다.");
            return;
        }

        // 인벤 수량 체크
        foreach (var plannedItem in planned)
        {
            if (inventory.GetCount(plannedItem.Key) < plannedItem.Value)
            {
                SetHint($"{plannedItem.Key} 가 부족합니다.");
                return;
            }
        }

        var matchedProduct = FindMatch(planned);
        if (matchedProduct == null)
        {
            SetHint("일치하는 레시피가 없습니다.");
            return;
        }

        // 재료 소모
        foreach (var itemForConsume in planned)
            inventory.Consume(itemForConsume.Key, itemForConsume.Value);

        // 결과물 지급
        foreach (var p in matchedProduct.outputs)
            inventory.Add(p.type, p.count);

        ClearPlanned();

        SetHint($"조합 완료 : {matchedProduct.displayName}");
    }

    CraftingRecipe FindMatch(Dictionary<ItemType, int> planned)
    {
        foreach (var recipe in recipeList)
        {
            // 필요한 재료를 충분히 갖췄는지
            bool ok = true;
            foreach (var ing in recipe.inputs)
            {
                if (!planned.TryGetValue(ing.type, out int have) || have != ing.count)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
                return recipe;
        }
        return null;
    }
}