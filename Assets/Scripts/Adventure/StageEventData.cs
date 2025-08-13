using UnityEngine;

[CreateAssetMenu(fileName = "StageEvent", menuName = "ScriptableObjects/StageEvent")]
public class StageEventData : ScriptableObject
{
    [TextArea]
    public string eventDescription;
    public string option1Text;
    public string option2Text;

    public bool option1IsFight;  // �ɼ� 1 ���� �� ���� ����
    public bool option2IsFight;  // �ɼ� 2 ���� �� ���� ����

    public StageData battleStage;
}
