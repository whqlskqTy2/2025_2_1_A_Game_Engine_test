using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairDot : MonoBehaviour
{
    [Header("Dot Style")]
    public Color dotColor = Color.white;
    public float dotSize = 6f;

    private Image dotImage;

    void Awake()
    {
        // 자신이 Canvas 하위에 존재해야 함
        dotImage = gameObject.AddComponent<Image>();
        dotImage.color = dotColor;

        // 내장 리소스 대신 직접 1픽셀 흰색 텍스처 생성
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        //  텍스처로 Sprite 생성
        dotImage.sprite = Sprite.Create(
            tex,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            100f
        );
        dotImage.type = Image.Type.Simple;

        // 중앙 배치
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(dotSize, dotSize);
    }
}
