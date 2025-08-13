using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterListUI : MonoBehaviour
{
    public Transform contentPanel; // Vertical Layout Group이 붙은 오브젝트
    public GameObject characterSlotPrefab; // CharacterSlot 프리팹 참조
    public CharacterDetailUI detailUI;

    private System.Action<CharacterData2> onCharacterSelected;

    private void OnEnable()
    {
        RefreshCharacterList();
    }

    public void RefreshCharacterList()
    {
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        List<CharacterData2> characters = CharacterInventoryManager.Instance.GetAllCharacters();

        foreach (CharacterData2 character in characters)
        {
            GameObject slotObj = Instantiate(characterSlotPrefab, contentPanel);
            CharacterSlotUI slot = slotObj.GetComponent<CharacterSlotUI>();
            slot.Initialize(character, detailUI);
        }
    }

    public void OpenForTeamSelect(System.Action<CharacterData2> callback)
    {
        onCharacterSelected = callback;
        gameObject.SetActive(true);
    }

    public void SelectCharacterForTeam(CharacterData2 character)
    {
        onCharacterSelected?.Invoke(character);
        gameObject.SetActive(false);
    }
}
