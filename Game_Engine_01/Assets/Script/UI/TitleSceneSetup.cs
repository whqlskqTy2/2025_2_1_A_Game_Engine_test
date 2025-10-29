using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneSetup : MonoBehaviour
{
    void Start()
    {
        // 타이틀(혹은 엔딩) 씬 들어왔을 때 마우스/시간 복구
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }
}