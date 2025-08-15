// Scripts/Runtime/PartyRuntime.cs
using System.Collections.Generic;

[System.Serializable]
public class RuntimeMember
{
    public CharacterData2 data;
    public int curHP;
    public int curMP;
    public RuntimeMember(CharacterData2 d) { data = d; curHP = d.maxHP; curMP = d.maxMP; }
}

[System.Serializable]
public class PartyRuntime
{
    public List<RuntimeMember> members = new();
    public static PartyRuntime FromCharacters(List<CharacterData2> chars)
    {
        var p = new PartyRuntime();
        foreach (var c in chars) if (c != null) p.members.Add(new RuntimeMember(c));
        return p;
    }
}