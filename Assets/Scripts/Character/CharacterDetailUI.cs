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

        statText.text = $"LV. {data.level}\nü��: {data.maxHP}\n����: {data.maxMP}\n���ݷ�: {data.attack}\n����: {data.defense}\n�ֹ���: {data.magic}\n���ŷ�: {data.resistance}\n���ǵ�: {data.agility}\n���: {data.talent}\n��Ʈ����: {data.stress}";
    }
}
