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
        DontDestroyOnLoad(gameObject); // �ʿ� ������ ����
    }

    // ��Ÿ�� ��Ƽ ��
    [System.Serializable]
    public class PartyMember
    {
        public string id;              // CharacterData2.characterId ���� (������ null)
        public string name;            // CharacterData2.characterName
        public CharacterData2 dataRef; // ���� ������ (����/������ ��)

        // ����/���� ����
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

    // ���� ��Ƽ(�ִ� 5�� ����)
    [SerializeField] private List<PartyMember> currentParty = new List<PartyMember>(5);
    public IReadOnlyList<PartyMember> CurrentParty => currentParty;

    /* -------------------------
     * ��Ƽ ����/�ʱ�ȭ
     * ------------------------- */

    // CharacterData2 ����Ʈ�� ����
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

    // CharacterInAdventure ����Ʈ�� ����
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

    // ���� ���� �� HP�� �ִ�ġ��/���� �� ��å ����
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
     * ã��/���� ����
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
     * BattleManager ���޿�
     * ------------------------- */

    // ���� ���� �� ����� ���� ������ ���
    public List<CharacterData2> GetPartyData()
    {
        return currentParty.Where(p => p?.dataRef != null).Select(p => p.dataRef).ToList();
    }

    // ���� ���� ���� HP/��Ʈ���� ���� CombatCharacter �ʱ�ȭ�� �ݿ��Ϸ��� ���
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
