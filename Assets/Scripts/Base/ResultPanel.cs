// ResultPanel.cs
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text detailText;
    [SerializeField] private Button okButton;

    public BattleResult LastResult { get; private set; }

    public void Show(BattleResult result, Action onOkClicked)
    {
        LastResult = result;

        bool won = result != null && result.isVictory;
        if (titleText != null)
            titleText.text = won ? "���� �¸�!" : "�й�...";

        // ���¼�
        int stones = result?.stoneReward ?? 0;

        // ��� ������ ǥ�� (ItemName xCount)
        string dropsText = "-";
        if (result?.drops != null && result.drops.Count > 0)
        {
            var names = result.drops
                .Where(d => d != null && d.item != null)
                .Select(d => $"{d.item.itemName} x{Mathf.Max(1, d.count)}")
                .ToList();

            if (names.Count > 0) dropsText = string.Join(", ", names);
        }

        // �����: partyResults���� isDead == true�� ���
        string casualtiesText = "-";
        if (result?.partyResults != null && result.partyResults.Count > 0)
        {
            var deadNames = result.partyResults
                .Where(p => p != null && p.isDead)
                .Select(p => string.IsNullOrEmpty(p.name) ? p.id : p.name)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            if (deadNames.Count > 0) casualtiesText = string.Join(", ", deadNames);
        }

        if (detailText != null)
        {
            detailText.text =
                $"ȹ�� ���¼�: {stones}\n" +
                $"��� ������: {dropsText}\n" +
                $"�����: {casualtiesText}";
        }

        gameObject.SetActive(true);

        if (okButton != null)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(() =>
            {
                onOkClicked?.Invoke();
                gameObject.SetActive(false);
            });
        }
    }
}