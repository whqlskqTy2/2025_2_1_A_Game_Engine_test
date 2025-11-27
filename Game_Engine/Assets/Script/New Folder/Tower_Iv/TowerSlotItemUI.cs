using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerSlotItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI countText;
    public TowerType towerType;

    public void Setup(Sprite icon, int count, TowerType type)
    {
        iconImage.sprite = icon;
        countText.text = count.ToString();
        towerType = type;
    }
}