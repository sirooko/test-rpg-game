using UnityEngine;
using UnityEngine.UI;

public class SummonManager : MonoBehaviour
{
    public CharacterData2[] possibleCharacters;

    public GameObject summonResultUI;
    public Image resultImage;
    public Text resultName;

    public int summonCost = 100; // (레거시) 내부 비용. 외부에서 비용 처리하면 사용 안 함.

    // ✅ 외부에서 비용을 이미 차감한 경우 이 함수를 호출하세요.
    public void SummonCharacterNoCost()
    {
        SummonCore();
    }

    // (레거시) 내부에서 비용까지 처리하는 버전 — 기존 코드 유지
    public void SummonCharacter()
    {
        // ① 재화 확인 및 차감 (내부 처리)
        if (!CurrencyManager.Instance.SpendCurrency(summonCost))
        {
            Debug.Log("소환 실패: 재화 부족");
            return;
        }
        SummonCore();
    }

    // 공통 소환 본체
    private void SummonCore()
    {
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

    public void CloseResultUI() => summonResultUI.SetActive(false);

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