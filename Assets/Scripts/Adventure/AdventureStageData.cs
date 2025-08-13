using UnityEngine;

[CreateAssetMenu(fileName = "NewStage", menuName = "ScriptableObjects/AdventureStage")]
public class AdventureStageData : ScriptableObject
{
    public int stageNumber;
    [TextArea]
    public string description;  // �̺�Ʈ ����

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
    // TODO: Ȯ�� ���� (��: ����Ȯ��, �����̻�, �б� ��)
}
