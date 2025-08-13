using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보(기존 유지)")]
    public string itemName;
    public string description;
    public ItemType type;
    public int healAmount;
    public bool targetAll = false;
    public StatusEffectType statusToHeal;
    public Sprite icon;
    public bool isUsableInBattle;

    [Header("경제/스택 (추가)")]
    public int price = 0;
    public int maxStack = 99;

    [Header("소비 아이템 확장 옵션 (추가)")]
    public int mpRestore = 0;
    public int stressReduce = 0;
    public bool reviveIfDead = false;
    public int reviveHP = 1;
    public bool cureAllStatuses = false;

    [Header("스킬북 (추가)")]
    public SkillData skillToLearn;

    [Header("장비 스탯 보너스 (선택 추가)")]
    public int atkBonus;
    public int defBonus;
    public int magBonus;
    public int mdefBonus;
    public int agiBonus;

    // ------- 분류 헬퍼 -------
    public bool IsConsumableType =>
        type == ItemType.Consumable ||
        type == ItemType.HealHP ||
        type == ItemType.HealMP ||
        type == ItemType.Revive ||
        type == ItemType.HealStatus;

    public bool IsSkillBookType => type == ItemType.SkillBook;
    public bool IsEquipmentType => type == ItemType.Equipment;

    // ------- 적용 헬퍼 (선택 사용) -------
    /// <summary>
    /// 소비형/회복형 아이템 효과 적용 (전투/모험 공용).
    /// CombatCharacter에 ClearAllStatuses/RemoveStatus/IsAlive 프로퍼티가 있다고 가정.
    /// </summary>
    public void ApplyConsumableEffect(CombatCharacter target)
    {
        if (target == null || !IsConsumableType) return;

        switch (type)
        {
            case ItemType.Consumable:
                // HP 회복
                if (target.IsAlive() && healAmount > 0)
                    target.Heal(healAmount);

                // MP 회복
                if (target.IsAlive() && (mpRestore > 0))
                    target.RecoverMP(mpRestore);

                // 스트레스 감소: CombatCharacter에 스트레스가 없다면 일단 보류/무시
                // 추후 전투 쪽에 스트레스 필드/메서드가 생기면 여기서 호출

                // 상태이상 해제
                if (target.IsAlive())
                {
                    if (cureAllStatuses) target.ClearAllStatuses();
                    else if (statusToHeal != StatusEffectType.None) target.TryRemoveStatus(statusToHeal);
                }
                break;

            case ItemType.HealHP:
                if (target.IsAlive() && healAmount > 0)
                    target.Heal(healAmount);
                break;

            case ItemType.HealMP:
                if (target.IsAlive())
                {
                    // 과거 데이터 호환: healAmount를 MP로 쓰던 경우 대비
                    int mp = (mpRestore > 0) ? mpRestore : healAmount;
                    if (mp > 0) target.RecoverMP(mp);
                }
                break;

            case ItemType.Revive:
                // 사망자 대상: 부활
                if (!target.IsAlive() && (reviveIfDead || healAmount > 0 || reviveHP > 0))
                {
                    target.isAlive = true;
                    int hp = (reviveHP > 0) ? reviveHP : Mathf.Max(1, healAmount);
                    target.currentHP = Mathf.Clamp(hp, 1, target.MaxHP);
                    target.ui?.UpdateStats(); // 부활 직후 UI 반영
                }
                break;

            case ItemType.HealStatus:
                if (target.IsAlive())
                {
                    if (cureAllStatuses) target.ClearAllStatuses();
                    else if (statusToHeal != StatusEffectType.None) target.TryRemoveStatus(statusToHeal);
                }
                break;
        }
    }

    /// 스킬북 사용. 성공 시 true.
    public bool ApplySkillBook(CombatCharacter target)
    {
        if (!IsSkillBookType || target == null || skillToLearn == null) return false;
        if (!target.skills.Contains(skillToLearn))
        {
            target.skills.Add(skillToLearn);
            return true;
        }
        return false;
    }
}
