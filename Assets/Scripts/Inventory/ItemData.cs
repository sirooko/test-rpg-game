using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("�⺻ ����(���� ����)")]
    public string itemName;
    public string description;
    public ItemType type;
    public int healAmount;
    public bool targetAll = false;
    public StatusEffectType statusToHeal;
    public Sprite icon;
    public bool isUsableInBattle;

    [Header("����/���� (�߰�)")]
    public int price = 0;
    public int maxStack = 99;

    [Header("�Һ� ������ Ȯ�� �ɼ� (�߰�)")]
    public int mpRestore = 0;
    public int stressReduce = 0;
    public bool reviveIfDead = false;
    public int reviveHP = 1;
    public bool cureAllStatuses = false;

    [Header("��ų�� (�߰�)")]
    public SkillData skillToLearn;

    [Header("��� ���� ���ʽ� (���� �߰�)")]
    public int atkBonus;
    public int defBonus;
    public int magBonus;
    public int mdefBonus;
    public int agiBonus;

    // ------- �з� ���� -------
    public bool IsConsumableType =>
        type == ItemType.Consumable ||
        type == ItemType.HealHP ||
        type == ItemType.HealMP ||
        type == ItemType.Revive ||
        type == ItemType.HealStatus;

    public bool IsSkillBookType => type == ItemType.SkillBook;
    public bool IsEquipmentType => type == ItemType.Equipment;

    // ------- ���� ���� (���� ���) -------
    /// <summary>
    /// �Һ���/ȸ���� ������ ȿ�� ���� (����/���� ����).
    /// CombatCharacter�� ClearAllStatuses/RemoveStatus/IsAlive ������Ƽ�� �ִٰ� ����.
    /// </summary>
    public void ApplyConsumableEffect(CombatCharacter target)
    {
        if (target == null || !IsConsumableType) return;

        switch (type)
        {
            case ItemType.Consumable:
                // HP ȸ��
                if (target.IsAlive() && healAmount > 0)
                    target.Heal(healAmount);

                // MP ȸ��
                if (target.IsAlive() && (mpRestore > 0))
                    target.RecoverMP(mpRestore);

                // ��Ʈ���� ����: CombatCharacter�� ��Ʈ������ ���ٸ� �ϴ� ����/����
                // ���� ���� �ʿ� ��Ʈ���� �ʵ�/�޼��尡 ����� ���⼭ ȣ��

                // �����̻� ����
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
                    // ���� ������ ȣȯ: healAmount�� MP�� ���� ��� ���
                    int mp = (mpRestore > 0) ? mpRestore : healAmount;
                    if (mp > 0) target.RecoverMP(mp);
                }
                break;

            case ItemType.Revive:
                // ����� ���: ��Ȱ
                if (!target.IsAlive() && (reviveIfDead || healAmount > 0 || reviveHP > 0))
                {
                    target.isAlive = true;
                    int hp = (reviveHP > 0) ? reviveHP : Mathf.Max(1, healAmount);
                    target.currentHP = Mathf.Clamp(hp, 1, target.MaxHP);
                    target.ui?.UpdateStats(); // ��Ȱ ���� UI �ݿ�
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

    /// ��ų�� ���. ���� �� true.
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
