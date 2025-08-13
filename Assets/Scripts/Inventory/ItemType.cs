public enum ItemType
{
    Consumable = 0,
    Key = 1,

    // 회복/소비 계열 (값 고정)
    HealHP = 2,   // HP 회복
    HealMP = 3,   // MP 회복
    Revive = 4,   // 부활
    HealStatus = 5,   // 상태 이상 해제

    // 기능적으로 다른 계열 (값 고정)
    SkillBook = 10,
    Equipment = 11
}
