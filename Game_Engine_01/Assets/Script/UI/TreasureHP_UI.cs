using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreasureHP_UI : MonoBehaviour
{
    [Header("References")]
    public TreasureHealth treasure; // ���� ������Ʈ
    public Slider hpBar;             // HP �����̴�
    public TMP_Text hpText;          // HP �ؽ�Ʈ

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