using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro�� �� �Ŷ�� ����

public class RoundAnnouncement : MonoBehaviour
{
    [System.Serializable]
    public class TimedMessage
    {
        public float triggerTime = 10f;     // �� �� �ڿ� ���� (���� ���� ����)
        public string messageText = "1 ����";
        public float displayDuration = 3f;  // ȭ�鿡 �� �� ���� ��������
    }

    [Header("UI Ref")]
    public TextMeshProUGUI messageTextUI;   // "1 ����" ������ TMP �ؽ�Ʈ
    public CanvasGroup canvasGroup;        // ���̵� in/out ����� (������ �׳� SetActive ������ε� ����)

    [Header("Messages")]
    public TimedMessage[] messages;        // ���� �� ��� ����. ��: 100�ʿ� "1 ����", 200�ʿ� "2 ����"

    [Header("Behavior")]
    public bool loopThroughMessagesOnce = true; // true�� �� ������ ���
    public bool fadeInOut = true;
    public float fadeSpeed = 5f;                // ���̵� �ӵ�

    private float elapsed;
    private int nextMsgIndex = 0;
    private float hideAtTime = -1f;
    private bool showing = false;

    void Start()
    {
        elapsed = 0f;
        nextMsgIndex = 0;
        hideAtTime = -1f;
        showing = false;

        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
        }
        if (messageTextUI)
        {
            messageTextUI.text = "";
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // ���� ��� �� �� �޽����� ���� �ִٸ�, ���� �޽����� Ʈ���� Ÿ�ӿ� �����ߴ��� Ȯ��
        if (nextMsgIndex < messages.Length)
        {
            var msg = messages[nextMsgIndex];
            if (elapsed >= msg.triggerTime)
            {
                ShowMessage(msg);
                nextMsgIndex++;

                // loopThroughMessagesOnce = false ���,
                // ���߿� �ٽ� ���ٵ簡 �ϴ� ������ Ȯ���� �� ����
            }
        }

        // ���� ǥ�� ���̸� �ð� ���� ������ ����
        if (showing && hideAtTime > 0f && elapsed >= hideAtTime)
        {
            HideMessage();
        }

        // ���̵� ó��
        HandleFade();
    }

    void ShowMessage(TimedMessage msg)
    {
        if (!messageTextUI) return;

        messageTextUI.text = msg.messageText;
        hideAtTime = elapsed + msg.displayDuration;
        showing = true;

        if (canvasGroup)
        {
            // ���̰�
            if (!fadeInOut)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }

    void HideMessage()
    {
        // �ܼ��� ǥ�ø� ����, �ؽ�Ʈ�� ������ ���� �����ص� ��
        showing = false;

        if (!fadeInOut && canvasGroup)
        {
            canvasGroup.alpha = 0f;
        }
    }

    void HandleFade()
    {
        if (!canvasGroup) return;
        if (!fadeInOut) return;

        // showing�̸� alpha -> 1, �ƴϸ� alpha -> 0
        float target = showing ? 1f : 0f;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, fadeSpeed * Time.deltaTime);
    }
}