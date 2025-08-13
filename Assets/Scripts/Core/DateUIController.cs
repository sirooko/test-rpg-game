using UnityEngine;
using UnityEngine.UI;

public class DateUIController : MonoBehaviour
{
    public Text dateText; // 현재 날짜 + 시간대 표시 텍스트

    private void Start()
    {
        UpdateUI();
        DateManager.Instance.OnTimeChanged.AddListener(UpdateUI);
    }

    void UpdateUI()
    {
        if (DateManager.Instance != null)
        {
            dateText.text = DateManager.Instance.GetFormattedDate();
        }
    }
}
