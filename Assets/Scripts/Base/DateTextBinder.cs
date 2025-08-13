// Scripts/Base/UI/DateTextBinder.cs
using UnityEngine;
using UnityEngine.UI;

public class DateTextBinder : MonoBehaviour
{
    [SerializeField] private GameDateManager dateMgr;
    [SerializeField] private Text dayText;
    [SerializeField] private Text timeText;

    void Reset() => dateMgr = FindAnyObjectByType<GameDateManager>();

    void OnEnable()
    {
        if (dateMgr != null)
        {
            dateMgr.OnChanged += OnDateChanged;
            OnDateChanged(dateMgr.Day, dateMgr.CurrentTime);
        }
    }

    void OnDisable()
    {
        if (dateMgr != null) dateMgr.OnChanged -= OnDateChanged;
    }

    void OnDateChanged(int day, TimeSlot slot)
    {
        if (dayText) dayText.text = $"Day {day}";
        if (timeText) timeText.text = slot == TimeSlot.Morning ? "아침"
                                 : slot == TimeSlot.Noon ? "점심"
                                 : "저녁";
    }
}
