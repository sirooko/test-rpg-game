using UnityEngine;

public class StatusEffect
{
    public StatusEffectType effectType;
    public int remainingTurns;

    public StatusEffect(StatusEffectType type, int duration)
    {
        this.effectType = type;
        remainingTurns = duration;

        // �����̻� ���� �� ����
        //switch (type)
        //{
        //    case StatusEffectType.Stun:
        //        remainingTurns = 1; // ���� 1��
        //        break;
        //    case StatusEffectType.Bleed:
        //    case StatusEffectType.Poison:
        //        remainingTurns = Random.Range(1, 5); // 1~4�� ����
        //        break;
        //}
    }

    public void Tick()
    {
        remainingTurns--;
    }

    public bool IsExpired()
    {
        return remainingTurns <= 0;
    }
}
