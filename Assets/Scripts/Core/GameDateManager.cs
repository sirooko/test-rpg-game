// Scripts/Core/GameDate/GameDateManager.cs
using System;
using UnityEngine;

public enum TimeSlot { Morning, Noon, Evening }

public class GameDateManager : MonoBehaviour
{
    [SerializeField] private int day = 1;
    [SerializeField] private TimeSlot time = TimeSlot.Morning;

    public int Day => day;
    public TimeSlot CurrentTime => time;

    public event Action<int, TimeSlot> OnChanged;

    void Start() => RaiseChanged();

    public void AdvanceTimeSlot()
    {
        if (time == TimeSlot.Evening) { day++; time = TimeSlot.Morning; }
        else { time++; }
        RaiseChanged();
    }

    private void RaiseChanged() => OnChanged?.Invoke(day, time);
}
