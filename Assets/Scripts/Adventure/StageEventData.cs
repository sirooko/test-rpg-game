// Scripts/Adventure/StageEventData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "StageEvent", menuName = "ScriptableObjects/StageEvent")]
public class StageEventData : ScriptableObject
{
    [TextArea] public string eventDescription;

    // ���� ���� �ʵ��(���� ȣȯ��)
    public string option1Text;
    public string option2Text;
    public bool option1IsFight;
    public bool option2IsFight;
    public StageData battleStage; // �� �� �ʵ尡 ����� �� ����

    // ���� �ű�: �������� ����/��/������ ���
    public StageData option1Stage;                       // ������� �� �������� ���
    public StageData option2Stage;
    public CharacterData2[] option1EnemiesOverride;      // ������ �� ����Ʈ�� �켱
    public CharacterData2[] option2EnemiesOverride;

    [System.Serializable]
    public struct NonCombatOutcome
    {
        public int stressDelta;  // �� ���� ��Ʈ���� ��
        public int hpDelta;      // �� ���� HP ��
        public int stonesDelta;  // ��ȭ ��
    }
    public NonCombatOutcome option1Outcome;              // ������ �ƴ� ���
    public NonCombatOutcome option2Outcome;
}
