using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData2 : ScriptableObject
{
    public string characterName;
    public Sprite battleSprite; // ← 전투에서 사용할 스프라이트
    public Sprite characterSprite;
    public string characterId;
    public int level;
    public int maxHP;
    public int maxMP;
    public int attack;
    public int defense;
    public int magic;
    public int resistance;
    public int agility;
    public int talent;
    public int stress;
    public int grade;  // 1 ~ 5
}
