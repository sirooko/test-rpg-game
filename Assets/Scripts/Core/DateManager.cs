using UnityEngine;
using UnityEngine.Events;

public enum TimePhase { Morning, Noon, Evening }

public class DateManager : MonoBehaviour
{
    public static DateManager Instance;

    public int currentDay = 1;
    public TimePhase currentPhase = TimePhase.Morning;

    public UnityEvent OnTimeChanged; // UI ���� � ���

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// �ð��븦 �� �ܰ� ����. ���� �� ���� �� ��ħ
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

        OnTimeChanged?.Invoke(); // UI �� ������Ʈ Ʈ����
        Debug.Log($"[Time] {currentDay}���� {currentPhase}�� �����");
    }

    public string GetFormattedDate()
    {
        return $"{currentDay}���� - {currentPhase}";
    }
}
