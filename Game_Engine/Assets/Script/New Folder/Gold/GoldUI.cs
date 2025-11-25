using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    private void Update()
    {
        if (GoldManager.Instance == null) return;
        if (goldText == null) return;

        goldText.text = $"Gold : {GoldManager.Instance.gold}";
    }
}