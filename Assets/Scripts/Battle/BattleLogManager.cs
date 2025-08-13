using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleLogManager : MonoBehaviour
{
    public static BattleLogManager Instance { get; private set; }

    [Header("Log UI")]
    public Text logText;                 // UnityEngine.UI.Text 사용
    public float clearDelay = 3f;        // 로그 자동 삭제 시간 (초)

    private Coroutine clearCoroutine;

    private void Awake()
    {
        // 싱글톤 설정
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
    /// 전투 로그 출력 (자동 제거 포함)
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
    /// 로그 즉시 초기화
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
