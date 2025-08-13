using UnityEngine;
using UnityEngine.UI;
using Game;

public class AdventureUI : MonoBehaviour
{
    [Header("Test Stages")]
    [SerializeField] private StageData[] stages;
    [SerializeField] private Button[] stageButtons; // 1:1 ���� ���� (���� �׽�Ʈ)

    void Awake()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int idx = i;
            stageButtons[i].onClick.AddListener(() =>
            {
                if (idx < stages.Length)
                {
                    GameContext.Instance.SelectedStage = stages[idx];
                    SceneFlow.Instance.LoadBattle();
                }
            });
        }
    }
}
