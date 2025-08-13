using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SummonManager : MonoBehaviour
{
    public CharacterData2[] possibleCharacters;

    public GameObject summonResultUI;
    public Image resultImage;
    public Text resultName;

    public int summonCost = 100; // ���¼� �Ҹ�


    public void SummonCharacter()
    {
        // �� ��ȭ Ȯ�� �� ����
        if (!CurrencyManager.Instance.SpendCurrency(summonCost))
            {
                Debug.Log("��ȯ ����: ��ȭ ����");
                return;
            }

        // �� ���������� ��ȯ ����

        // ���� ����
        int rand = Random.Range(0, possibleCharacters.Length);
        CharacterData2 template = possibleCharacters[rand];

        // ���ο� �ν��Ͻ� ���� �� ������ ����
        CharacterData2 newCharacter = ScriptableObject.CreateInstance<CharacterData2>();
        CopyCharacterData(template, newCharacter);

        // ����Ʈ�� �߰�
        CharacterInventoryManager.Instance.AddCharacter(newCharacter);

        // ��� UI ǥ��
        resultImage.sprite = newCharacter.characterSprite;
        resultName.text = newCharacter.characterName;
        summonResultUI.SetActive(true);
    }

    public void CloseResultUI()
    {
        summonResultUI.SetActive(false);
    }

    // ������ ���� �Լ�
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
