using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDetailUI : MonoBehaviour
{
    public Image characterImage;
    public Text characterName;
    public Text statText;

    public void SetCharacterData(CharacterData2 data)
    {
        characterImage.sprite = data.characterSprite;
        characterName.text = data.characterName;

        statText.text = $"LV. {data.level}\n체력: {data.maxHP}\n마나: {data.maxMP}\n공격력: {data.attack}\n방어력: {data.defense}\n주문력: {data.magic}\n정신력: {data.resistance}\n스피드: {data.agility}\n재능: {data.talent}\n스트레스: {data.stress}";
    }
}
