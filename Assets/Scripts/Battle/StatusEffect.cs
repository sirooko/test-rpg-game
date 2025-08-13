using UnityEngine;

public class StatusEffect
{
    public StatusEffectType effectType;
    public int remainingTurns;

    public StatusEffect(StatusEffectType type, int duration)
    {
        this.effectType = type;
        remainingTurns = duration;

        // 상태이상별 지속 턴 설정
        //switch (type)
        //{
        //    case StatusEffectType.Stun:
        //        remainingTurns = 1; // 고정 1턴
        //        break;
        //    case StatusEffectType.Bleed:
        //    case StatusEffectType.Poison:
        //        remainingTurns = Random.Range(1, 5); // 1~4턴 랜덤
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
