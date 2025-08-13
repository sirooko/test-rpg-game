using UnityEngine;
using UnityEngine.Events;

public enum TimePhase { Morning, Noon, Evening }

public class DateManager : MonoBehaviour
{
    public static DateManager Instance;

    public int currentDay = 1;
    public TimePhase currentPhase = TimePhase.Morning;

    public UnityEvent OnTimeChanged; // UI 갱신 등에 사용

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 시간대를 한 단계 진행. 저녁 → 다음 날 아침
    /// </summary>
    public void AdvanceTime()
    {
        switch (currentPhase)
        {
            case TimePhase.Morning:
                currentPhase = TimePhase.Noon;
                break;
            case TimePhase.Noon:
                currentPhase = TimePhase.Evening;
                break;
            case TimePhase.Evening:
                currentPhase = TimePhase.Morning;
                currentDay++;
                break;
        }

        OnTimeChanged?.Invoke(); // UI 등 업데이트 트리거
        Debug.Log($"[Time] {currentDay}일차 {currentPhase}로 변경됨");
    }

    public string GetFormattedDate()
    {
        return $"{currentDay}일차 - {currentPhase}";
    }
}
