using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatCharacter : MonoBehaviour
{
    [Header("기본 캐릭터 정보")]
    public CharacterData2 characterData;   // ScriptableObject 참조
    public bool isPlayer = true;
    public bool isEnemy = true;

    [Header("전투 상태")]
    public int currentHP;
    public int currentMP;
    public bool isAlive = true;
    public bool hasActed = false;
    public bool isDefending = false;

    public List<SkillData> skills = new List<SkillData>();  // 보유 스킬 목록

    public List<StatusEffect> activeStatusEffects = new List<StatusEffect>(); // 상태이상 리스트

    public Sprite BattleSprite => characterData.battleSprite;

    public CombatCharacterUI ui;  // 연결된 UI 참조

    // 캐릭터 스탯 접근 속성 (데이터에서 가져옴)
    public int ATK => characterData.attack;
    public int DEF => characterData.defense;
    public int MAG => characterData.magic;
    public int RES => characterData.resistance;
    public int AGI => characterData.agility;
    public int MaxHP => characterData != null ? characterData.maxHP : currentHP;
    public int MaxMP => characterData != null ? characterData.maxMP : currentMP;

    public bool IsAlive() => isAlive && currentHP > 0;  // 둘 다 만족해야 생존

    // ✅ 혼동 방지 프로퍼티 (읽기/쓰기)
    public int CurrentHP { get => currentHP; set { currentHP = Mathf.Clamp(value, 0, MaxHP); ui?.UpdateStats(); } }
    public int CurrentMP { get => currentMP; set { currentMP = Mathf.Clamp(value, 0, MaxMP); ui?.UpdateStats(); } }

    // ✅ 이름 별칭(아이템 헬퍼 호환용)
    public void ClearAllStatuses() => RemoveAllStatusEffects();                    // alias
    public bool TryRemoveStatusEffect(StatusEffectType type) => TryRemoveStatus(type); // alias

    private void Start()
    {
        // 체력 초기화
        currentHP = MaxHP;
        currentMP = MaxMP;
        isAlive = true;
        hasActed = false;

        // (예시용 스킬 추가. 실제로는 BattleManager에서 직접 추가할 수도 있음)
        //skills.Add(new SkillData("화염구", "적에게 마법 피해", SkillType.Damage, 20, false));
    }

    // 전투 초기화
    public void InitFromData(CharacterData2 data, bool isPlayerTeam)
    {
        characterData = data;
        isPlayer = isPlayerTeam;
        isEnemy = !isPlayerTeam;

        // 기본은 풀피/풀MP
        currentHP = data != null ? data.maxHP : 1;
        currentMP = data != null ? data.maxMP : 0;
        isAlive = true;
        hasActed = false;

        ApplySpritesIfAny();
    }

    /// <summary>
    /// 런타임 HP/MP를 넘기는 오버로드 (권장)
    /// </summary>
    public void InitFromData(CharacterData2 data, bool isPlayerTeam, int hpOverride, int mpOverride)
    {
        InitFromData(data, isPlayerTeam); // 기본 세팅 먼저
                                          // 런타임 값으로 덮어쓰기 (클램프)
        currentHP = Mathf.Clamp(hpOverride, 0, MaxHP);
        currentMP = Mathf.Clamp(mpOverride, 0, MaxMP);
        if (currentHP <= 0) { isAlive = false; }
    }

    private void ApplySpritesIfAny()
    {
        // 전투 스프라이트가 있으면 우선 적용 (프로젝트에 맞게 교체)
        var sprite = characterData != null ? (characterData.battleSprite ?? characterData.characterSprite) : null;

        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && sprite != null) sr.sprite = sprite;

        var img = GetComponentInChildren<UnityEngine.UI.Image>();
        if (img != null && sprite != null) img.sprite = sprite;
    }

    // 피해 처리
    // CombatCharacter.cs
    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        damage = Mathf.Max(0, damage);
        currentHP = Mathf.Max(0, currentHP - damage);

        // 🔹 UI 연출/갱신은 파괴 전에
        ui?.PlayHitEffect(damage);
        ui?.UpdateStats();

        if (currentHP == 0)
        {
            isAlive = false;
            BattleLogManager.Instance.ShowLog($"{characterData.characterName}이(가) 쓰러졌습니다!");
            BattleManager.Instance?.OnCharacterDied(this);   // 🔹 콜백
        }
    }

    // HP 회복
    public void Heal(int amount)
    {
        if (ui == null)
            Debug.LogWarning($"❌ UI가 연결되지 않음: {characterData.characterName}");
        else
            Debug.Log($"✅ UI 연결됨: {characterData.characterName}");
        currentHP = Mathf.Min(currentHP + amount, MaxHP);
        Debug.Log($"❤️‍🩹 [{characterData.characterName}] 체력 회복됨: +{amount} → 현재 HP: {currentHP}");
        ui?.UpdateStats(); // ✅ 체력바 갱신
    }


    // MP 회복
    public void RecoverMP(int amount)
    {
        currentMP = Mathf.Min(currentMP + amount, MaxMP);
        ui?.UpdateStats();  // ✅ 마나 갱신
    }

    // 상태 초기화
    public void ResetTurn()
    {
        hasActed = false;
    }

    public void ApplyStatusEffect(StatusEffectType type, int duration)
    {
        var existing = activeStatusEffects.FirstOrDefault(e => e.effectType == type);
        if (existing != null)
        {
            existing.remainingTurns = duration; // 갱신
        }
        else
        {
            activeStatusEffects.Add(new StatusEffect(type, duration));
        }

        BattleLogManager.Instance.ShowLog($"{characterData.characterName}에게 {type} 상태이상 적용 ({duration}턴)!");
    }

    public void ProcessStatusEffects()
    {
        var toRemove = new List<StatusEffect>();

        foreach (var effect in activeStatusEffects)
        {
            switch (effect.effectType)
            {
                case StatusEffectType.Stun:
                    BattleLogManager.Instance.ShowLog($"{characterData.characterName}은(는) 기절 상태입니다.");
                    effect.remainingTurns--;   // ✅ 기절도 턴 감소
                    break;

                case StatusEffectType.Bleed:
                    int bleedDamage = Mathf.Max(1, ATK / 10);
                    TakeDamage(bleedDamage);
                    effect.remainingTurns--;
                    break;

                case StatusEffectType.Poison:
                    // ※ 현재 구현은 MP를 감소시킴(디자인과 일치하는지 확인 필요)
                    int drain = Mathf.Max(1, MAG / 10);
                    currentMP = Mathf.Max(0, currentMP - drain);
                    BattleLogManager.Instance.ShowLog($"{characterData.characterName}이(가) 중독으로 MP {drain} 감소!");
                    ui?.UpdateStats();
                    effect.remainingTurns--;
                    break;
            }

            if (effect.remainingTurns <= 0)
                toRemove.Add(effect);
        }

        foreach (var e in toRemove)
            activeStatusEffects.Remove(e);

        // ❌ 여기서 사망/Destroy 처리 금지 (이미 TakeDamage → OnCharacterDied 경유)
    }

    // 특정 상태이상을 보유 중인지 확인
    public bool HasStatus(StatusEffectType type)
    {
        return activeStatusEffects.Any(e => e.effectType == type && e.remainingTurns > 0);
    }

    // 특정 상태이상 제거
    public void RemoveStatus(StatusEffectType type)
    {
        int removed = activeStatusEffects.RemoveAll(s => s.effectType == type);
        if (removed > 0)
        {
            BattleLogManager.Instance.ShowLog($"{characterData.characterName}의 {type} 해제!");
            ui?.UpdateStats();
        }
    }

    public void RemoveAllStatusEffects()
    {
        if (activeStatusEffects.Count > 0)
        {
            activeStatusEffects.Clear();
            BattleLogManager.Instance.ShowLog($"{characterData.characterName}의 모든 상태이상이 해제되었습니다.");
            ui?.UpdateStats();
        }
    }

    public bool TryRemoveStatus(StatusEffectType type)
    {
        if (!HasStatus(type)) return false;
        RemoveStatus(type);
        return true;
    }
}
