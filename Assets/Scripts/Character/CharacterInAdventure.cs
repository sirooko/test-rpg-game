// Scripts/Adventure/CharacterInAdventure.cs
[System.Serializable]
public class CharacterInAdventure
{
    public string Name;
    public int currentHP;
    public int currentStress;
    public bool isDead;

    public CharacterData2 originalData;

    public int MaxHP => originalData != null ? originalData.maxHP : currentHP; // ✅ 추가

    public CharacterInAdventure(CharacterData2 data)
    {
        originalData = data;
        Name = data.characterName;
        currentHP = data.maxHP;
        currentStress = 0;
        isDead = false;
    }
}