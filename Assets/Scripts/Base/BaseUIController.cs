// 상단 TopBar 텍스트 바인딩 (Day / Time / Stones)
using UnityEngine.UI;
using UnityEngine;
//using Project.Core;

namespace Project.Base.UI
{
    public class BaseUIController : MonoBehaviour
    {
        [Header("TopBar Texts")]
        [SerializeField] private Text dayText;
        [SerializeField] private Text timeText;
        [SerializeField] private Text stonesText;

        [Header("Refs")]
        [SerializeField] private GameDateManager dateMgr;
        [SerializeField] private CurrencyManager currencyMgr;

        void Reset()
        {
            dateMgr = FindAnyObjectByType<GameDateManager>();
            currencyMgr = FindAnyObjectByType<CurrencyManager>();
        }

        void OnEnable()
        {
            if (dateMgr != null) dateMgr.OnChanged += OnDateChanged;
            if (currencyMgr != null) currencyMgr.OnChanged += OnStonesChanged;
            RefreshAll();
        }

        void OnDisable()
        {
            if (dateMgr != null) dateMgr.OnChanged -= OnDateChanged;
            if (currencyMgr != null) currencyMgr.OnChanged -= OnStonesChanged;
        }

        void RefreshAll()
        {
            if (dateMgr != null) OnDateChanged(dateMgr.Day, dateMgr.CurrentTime);
            if (currencyMgr != null) OnStonesChanged(currencyMgr.Stones);
        }

        void OnDateChanged(int day, TimeSlot time)
        {
            dayText.text = $"Day {day}";
            timeText.text = time switch
            {
                TimeSlot.Morning => "아침",
                TimeSlot.Noon => "점심",
                _ => "저녁",
            };
        }

        void OnStonesChanged(int value) => stonesText.text = $"{value} 마력석";
    }
}
