using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    public Image characterImage;
    public Text characterName;
    private CharacterData2 characterData;
    private CharacterDetailUI detailUI;

    // �ܺο��� ������ ����
    public void Initialize(CharacterData2 data, CharacterDetailUI detail)
    {
        characterData = data;
        detailUI = detail;

        characterImage.sprite = data.characterSprite;
        characterName.text = data.characterName;
    }

    // ��ư�� ������ �Լ�
    public void OnClickSlot()
    {
        if (detailUI != null && characterData != null)
        {
            detailUI.SetCharacterData(characterData);
        }
    }
}
