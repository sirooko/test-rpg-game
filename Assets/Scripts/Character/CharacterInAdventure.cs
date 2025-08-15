using UnityEngine;

[System.Serializable]
public class CharacterInAdventure
{
    // 원본(불변): 캐릭터 스펙/초기값
    public CharacterData2 originalData;

    // 런타임(가변): 모험/전투 진행 중 변화
    public int currentHP;
    public int currentMP;
    public int currentStress;
    public bool isDead;

    // 표시/폴백용
    [System.NonSerialized] private string _nameFallback = "Unknown";

    // ==== 프록시(원본이 있으면 원본에서 읽고, 없으면 폴백/0) ====
    public string Name => originalData ? originalData.characterName : _nameFallback;
    public string CharacterId => originalData ? originalData.characterId : null;
    public Sprite Portrait => originalData ? originalData.characterSprite : null;
    public Sprite BattleSprite => originalData ? (originalData.battleSprite ?? originalData.characterSprite) : null;

    public int Level => originalData ? originalData.level : 1;
    public int MaxHP => originalData ? originalData.maxHP : currentHP;
    public int MaxMP => originalData ? originalData.maxMP : currentMP;
    public int Attack => originalData ? originalData.attack : 0;
    public int Defense => originalData ? originalData.defense : 0;
    public int Magic => originalData ? originalData.magic : 0;
    public int Resistance => originalData ? originalData.resistance : 0;
    public int Agility => originalData ? originalData.agility : 0;
    public int Talent => originalData ? originalData.talent : 0;
    public int Grade => originalData ? originalData.grade : 0;

    public CharacterInAdventure(CharacterData2 data)
    {
        originalData = data;
        if (data)
        {
            _nameFallback = data.characterName ?? "Unknown";
            currentHP = data.maxHP;
            currentMP = data.maxMP;
            currentStress = 0;           // ← 시작 스트레스를 원본 값으로 쓰고 싶으면 data.stress 로 바꿔도 됨
        }
        else
        {
            currentHP = 1; currentMP = 0; currentStress = 0;
        }
        isDead = false;
    }

    // ==== 상태 변화 유틸 ====
    public void ApplyDamage(int amount)
    {
        if (isDead) return;
        amount = Mathf.Max(0, amount);
        currentHP = Mathf.Max(0, currentHP - amount);
        if (currentHP == 0) isDead = true;
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        amount = Mathf.Max(0, amount);
        currentHP = Mathf.Min(MaxHP, currentHP + amount);
    }

    public void UseMP(int amount) { currentMP = Mathf.Max(0, currentMP - Mathf.Max(0, amount)); }
    public void RestoreMP(int amount) { currentMP = Mathf.Min(MaxMP, currentMP + Mathf.Max(0, amount)); }
    public void AddStress(int amount) { currentStress = Mathf.Max(0, currentStress + amount); }

    /// <summary>
    /// 전투 매니저의 InitFromData(...)에 넘길 스냅샷 SO 생성.
    /// (원본 값 복사 + 현재 스트레스 반영)
    /// </summary>
    public CharacterData2 CreateSnapshotSO()
    {
        var cd = ScriptableObject.CreateInstance<CharacterData2>();
        if (originalData)
        {
            cd.characterName = originalData.characterName;
            cd.characterId = originalData.characterId;
            cd.characterSprite = originalData.characterSprite;
            cd.battleSprite = originalData.battleSprite;
            cd.level = originalData.level;
            cd.maxHP = originalData.maxHP;
            cd.maxMP = originalData.maxMP;
            cd.attack = originalData.attack;
            cd.defense = originalData.defense;
            cd.magic = originalData.magic;
            cd.resistance = originalData.resistance;
            cd.agility = originalData.agility;
            cd.talent = originalData.talent;
            cd.grade = originalData.grade;
        }
        else
        {
            cd.characterName = _nameFallback;
            cd.maxHP = currentHP; cd.maxMP = currentMP;
        }

        // 런타임 스트레스 반영
        cd.stress = currentStress;
        return cd;
    }
}
