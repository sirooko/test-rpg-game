using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    public string stageId;
    public string displayName;
    public Sprite preview;
    // 추후: 적 웨이브, 드랍 테이블 등 확장
}
