using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleLogManager : MonoBehaviour
{
    public static BattleLogManager Instance { get; private set; }

    [Header("Log UI")]
    public Text logText;                 // UnityEngine.UI.Text ���
    public float clearDelay = 3f;        // �α� �ڵ� ���� �ð� (��)

    private Coroutine clearCoroutine;

    private void Awake()
    {
        // �̱��� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// ���� �α� ��� (�ڵ� ���� ����)
    /// </summary>
    public void ShowLog(string message)
    {
        Debug.Log("[BattleLog] " + message);

        if (logText != null)
        {
            logText.text = message;

            if (clearCoroutine != null)
                StopCoroutine(clearCoroutine);

            clearCoroutine = StartCoroutine(ClearLogRoutine(clearDelay));
        }
    }

    /// <summary>
    /// �α� ��� �ʱ�ȭ
    /// </summary>
    public void ClearLog()
    {
        if (logText != null)
        {
            logText.text = "";
        }
    }

    private IEnumerator ClearLogRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearLog();
    }
}
