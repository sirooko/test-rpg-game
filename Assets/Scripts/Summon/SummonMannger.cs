using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SummonManager : MonoBehaviour
{
    public CharacterData2[] possibleCharacters;

    public GameObject summonResultUI;
    public Image resultImage;
    public Text resultName;

    public int summonCost = 100; // 마력석 소모량


    public void SummonCharacter()
    {
        // ① 재화 확인 및 차감
        if (!CurrencyManager.Instance.SpendCurrency(summonCost))
            {
                Debug.Log("소환 실패: 재화 부족");
                return;
            }

        // ② 정상적으로 소환 진행

        // 랜덤 선택
        int rand = Random.Range(0, possibleCharacters.Length);
        CharacterData2 template = possibleCharacters[rand];

        // 새로운 인스턴스 생성 및 데이터 복사
        CharacterData2 newCharacter = ScriptableObject.CreateInstance<CharacterData2>();
        CopyCharacterData(template, newCharacter);

        // 리스트에 추가
        CharacterInventoryManager.Instance.AddCharacter(newCharacter);

        // 결과 UI 표시
        resultImage.sprite = newCharacter.characterSprite;
        resultName.text = newCharacter.characterName;
        summonResultUI.SetActive(true);
    }

    public void CloseResultUI()
    {
        summonResultUI.SetActive(false);
    }

    // 데이터 복사 함수
    void CopyCharacterData(CharacterData2 from, CharacterData2 to)
    {
        to.characterName = from.characterName;
        to.characterSprite = from.characterSprite;
        to.level = from.level;
        to.maxHP = from.maxHP;
        to.maxMP = from.maxMP;
        to.attack = from.attack;
        to.defense = from.defense;
        to.magic = from.magic;
        to.resistance = from.resistance;
        to.agility = from.agility;
        to.talent = from.talent;
        to.stress = from.stress;
        to.grade = from.grade;
    }
}
