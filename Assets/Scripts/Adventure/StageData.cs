// Scripts/Adventure/StageData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    public string stageId;
    public Sprite background;                     // (선택) 배경
    public List<CharacterData2> enemyTemplates;   // 기본 적 구성
    // 필요 시: 웨이브/배치/AI 등 확장 필드 추가
}
