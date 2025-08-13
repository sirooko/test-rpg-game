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
    public string skillName;               // ��ų �̸�
    [TextArea] public string description;  // ��ų ����

    public SkillTargetType targetType;     // ��ų ���

    public SkillType type;                 // ��ų Ÿ��
    public int power;                      // ������ or ȸ����
    public int manaCost;                   // ���� �Ҹ�

    //public bool targetAll;                 // ���� ����
    public bool isMagic;                   // ���� or ����

    public StatusEffectType statusEffect;  // �����̻� ȿ��
    [Range(0, 100)] public int statusChance; // �����̻� Ȯ�� (%)

    public Sprite icon;                    // ������ (UI��)
}
