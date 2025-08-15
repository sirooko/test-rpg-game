// Scripts/Adventure/StageEventData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "StageEvent", menuName = "ScriptableObjects/StageEvent")]
public class StageEventData : ScriptableObject
{
    [TextArea] public string eventDescription;

    // ── 기존 필드들(에셋 호환용)
    public string option1Text;
    public string option2Text;
    public bool option1IsFight;
    public bool option2IsFight;
    public StageData battleStage; // ← 새 필드가 비었을 때 폴백

    // ── 신규: 선택지별 전투/적/비전투 결과
    public StageData option1Stage;                       // 전투라면 이 스테이지 사용
    public StageData option2Stage;
    public CharacterData2[] option1EnemiesOverride;      // 있으면 이 리스트가 우선
    public CharacterData2[] option2EnemiesOverride;

    [System.Serializable]
    public struct NonCombatOutcome
    {
        public int stressDelta;  // 팀 전원 스트레스 ±
        public int hpDelta;      // 팀 전원 HP ±
        public int stonesDelta;  // 재화 ±
    }
    public NonCombatOutcome option1Outcome;              // 전투가 아닌 경우
    public NonCombatOutcome option2Outcome;
}
