using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreasureHP_UI : MonoBehaviour
{
    [Header("References")]
    public TreasureHealth treasure; // 보물 오브젝트
    public Slider hpBar;             // HP 슬라이더
    public TMP_Text hpText;          // HP 텍스트

    void Start()
    {
        if (treasure == null)
        {
            GameObject t = GameObject.FindGameObjectWithTag("Treasure");
            if (t) treasure = t.GetComponent<TreasureHealth>();
        }
    }

    void Update()
    {
        if (treasure == null || hpBar == null) return;

        float ratio = treasure.NormalizedHP;
        hpBar.value = ratio;

        if (hpText)
            hpText.text = $"Treasure HP: {treasure.CurrentHP} / {treasure.maxHP}";
    }
}