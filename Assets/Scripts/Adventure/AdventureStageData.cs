using UnityEngine;

[CreateAssetMenu(fileName = "NewStage", menuName = "ScriptableObjects/AdventureStage")]
public class AdventureStageData : ScriptableObject
{
    public int stageNumber;
    [TextArea]
    public string description;  // 이벤트 설명

    public string choice1Text;
    public string choice2Text;

    public StageResult choice1Result;
    public StageResult choice2Result;
}

[System.Serializable]
public class StageResult
{
    public bool triggersCombat;
    public int rewardAmount;
    public string resultMessage;
    // TODO: 확장 가능 (예: 성공확률, 상태이상, 분기 등)
}
