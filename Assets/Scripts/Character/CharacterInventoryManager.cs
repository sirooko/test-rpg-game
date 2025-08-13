using System.Collections.Generic;
using UnityEngine;

public class CharacterInventoryManager : MonoBehaviour
{
    public static CharacterInventoryManager Instance;

    private List<CharacterData2> ownedCharacters = new List<CharacterData2>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void AddCharacter(CharacterData2 character)
    {
        ownedCharacters.Add(character);
        Debug.Log($"{character.characterName} ��(��) ���� ��Ͽ� �߰��߽��ϴ�.");
    }

    public List<CharacterData2> GetAllCharacters()
    {
        return ownedCharacters;
    }
}
