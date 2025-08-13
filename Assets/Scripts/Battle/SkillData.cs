using UnityEngine;

public enum SkillType
{
    Damage,
    Heal,
    Buff,
    Debuff,
    Cleanse
}

public enum StatusEffectType
{
    None,
    Stun,
    Poison,
    Bleed,
    Silence,
    Weaken
}

public enum SkillTargetType
{
    Enemy,
    Ally,
    Self,
    AllEnemies,
    AllAllies
}


[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObjects/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;               // 스킬 이름
    [TextArea] public string description;  // 스킬 설명

    public SkillTargetType targetType;     // 스킬 대상

    public SkillType type;                 // 스킬 타입
    public int power;                      // 데미지 or 회복량
    public int manaCost;                   // 마나 소모

    //public bool targetAll;                 // 광역 여부
    public bool isMagic;                   // 물리 or 마법

    public StatusEffectType statusEffect;  // 상태이상 효과
    [Range(0, 100)] public int statusChance; // 상태이상 확률 (%)

    public Sprite icon;                    // 아이콘 (UI용)
}
