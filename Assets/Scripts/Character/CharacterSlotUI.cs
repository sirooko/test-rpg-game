using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    public Image characterImage;
    public Text characterName;
    private CharacterData2 characterData;
    private CharacterDetailUI detailUI;

    // 외부에서 데이터 주입
    public void Initialize(CharacterData2 data, CharacterDetailUI detail)
    {
        characterData = data;
        detailUI = detail;

        characterImage.sprite = data.characterSprite;
        characterName.text = data.characterName;
    }

    // 버튼에 연결할 함수
    public void OnClickSlot()
    {
        if (detailUI != null && characterData != null)
        {
            detailUI.SetCharacterData(characterData);
        }
    }
}
