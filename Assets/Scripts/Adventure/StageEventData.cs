using UnityEngine;

[CreateAssetMenu(fileName = "StageEvent", menuName = "ScriptableObjects/StageEvent")]
public class StageEventData : ScriptableObject
{
    [TextArea]
    public string eventDescription;
    public string option1Text;
    public string option2Text;

    public bool option1IsFight;  // 옵션 1 선택 시 전투 여부
    public bool option2IsFight;  // 옵션 2 선택 시 전투 여부

    public StageData battleStage;
}
