using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToTitleButton : MonoBehaviour
{
    [Header("Return Settings")]
    [Tooltip("돌아갈 타이틀 씬 이름 (Build Settings에 등록 필요)")]
    public string titleSceneName = "TitleScene";

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}
        
    