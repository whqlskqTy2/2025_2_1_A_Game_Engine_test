using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToTitleButton : MonoBehaviour
{
    [Header("Return Settings")]
    [Tooltip("���ư� Ÿ��Ʋ �� �̸� (Build Settings�� ��� �ʿ�)")]
    public string titleSceneName = "TitleScene";

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}
        
    