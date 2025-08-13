using UnityEngine;
using UnityEngine.UI;

public class DateUIController : MonoBehaviour
{
    public Text dateText; // ���� ��¥ + �ð��� ǥ�� �ؽ�Ʈ

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
