// Scripts/Battle/EnemyGroupData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Enemy Group")]
public class EnemyGroupData : ScriptableObject
{
    public List<CharacterData2> enemies;   // 적 템플릿(간단히 CharacterData2 재사용)
    // 필요 시 AI/배치/레벨 스케일 필드 추가
}
