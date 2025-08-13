using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 필요 없으면 제거
    }

    // 런타임 파티 모델
    [System.Serializable]
    public class PartyMember
    {
        public string id;              // CharacterData2.characterId 권장 (없으면 null)
        public string name;            // CharacterData2.characterName
        public CharacterData2 dataRef; // 원본 데이터 (스탯/아이콘 등)

        // 전투/모험 상태
        public int CurrentHP;
        public int Stress;

        public bool IsDead => CurrentHP <= 0;

        public PartyMember(CharacterData2 d)
        {
            dataRef = d;
            id = d != null ? d.characterId : null;
            name = d != null ? d.characterName : "Unknown";
            CurrentHP = d != null ? d.maxHP : 1;
            Stress = 0;
        }

        public PartyMember(CharacterInAdventure adv)
        {
            dataRef = adv != null ? adv.originalData : null;
            id = dataRef != null ? dataRef.characterId : null;
            name = adv != null ? adv.Name : (dataRef != null ? dataRef.characterName : "Unknown");
            CurrentHP = adv != null ? adv.currentHP : (dataRef != null ? dataRef.maxHP : 1);
            Stress = adv != null ? adv.currentStress : 0;
        }
    }

    // 현재 파티(최대 5명 가정)
    [SerializeField] private List<PartyMember> currentParty = new List<PartyMember>(5);
    public IReadOnlyList<PartyMember> CurrentParty => currentParty;

    /* -------------------------
     * 파티 구성/초기화
     * ------------------------- */

    // CharacterData2 리스트로 세팅
    public void SetParty(IEnumerable<CharacterData2> members)
    {
        currentParty.Clear();
        if (members == null) return;
        foreach (var m in members)
        {
            if (m == null) continue;
            currentParty.Add(new PartyMember(m));
        }
    }

    // CharacterInAdventure 리스트로 세팅
    public void SetParty(IEnumerable<CharacterInAdventure> members)
    {
        currentParty.Clear();
        if (members == null) return;
        foreach (var m in members)
        {
            if (m == null) continue;
            currentParty.Add(new PartyMember(m));
        }
    }

    // 전투 진입 전 HP를 최대치로/유지 등 정책 선택
    public void HealAllToFull(bool toFull = false)
    {
        foreach (var p in currentParty)
        {
            if (p?.dataRef == null) continue;
            if (toFull) p.CurrentHP = p.dataRef.maxHP;
            p.Stress = Mathf.Max(0, p.Stress);
        }
    }

    /* -------------------------
     * 찾기/삭제 헬퍼
     * ------------------------- */

    public PartyMember FindCharacterById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return currentParty.FirstOrDefault(p => p != null && p.id == id);
    }

    public PartyMember FindCharacterByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return currentParty.FirstOrDefault(p => p != null && p.name == name);
    }

    public void RemoveCharacter(PartyMember member)
    {
        if (member == null) return;
        currentParty.Remove(member);
    }

    public void RemoveCharacterById(string id)
    {
        var m = FindCharacterById(id);
        if (m != null) currentParty.Remove(m);
    }

    public void RemoveCharacterByName(string name)
    {
        var m = FindCharacterByName(name);
        if (m != null) currentParty.Remove(m);
    }

    /* -------------------------
     * BattleManager 전달용
     * ------------------------- */

    // 전투 스폰 시 사용할 원본 데이터 목록
    public List<CharacterData2> GetPartyData()
    {
        return currentParty.Where(p => p?.dataRef != null).Select(p => p.dataRef).ToList();
    }

    // 전투 전에 현재 HP/스트레스 값을 CombatCharacter 초기화에 반영하려면 사용
    public int GetMemberHPForInit(CharacterData2 data, int defaultHP)
    {
        var m = (data != null) ? FindCharacterById(data.characterId) : null;
        return m != null ? Mathf.Clamp(m.CurrentHP, 0, data.maxHP) : defaultHP;
    }

    public int GetMemberStress(CharacterData2 data, int defaultStress = 0)
    {
        var m = (data != null) ? FindCharacterById(data.characterId) : null;
        return m != null ? m.Stress : defaultStress;
    }
}
