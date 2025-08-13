using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSelectManager : MonoBehaviour
{
    public Button[] teamSlots; // �� ���� ��ư 5��
    public Button startAdventureButton; // ���� ���� ��ư
    public CharacterListUI characterListUI; // ���� ĳ���� ��� UI ����

    

    private CharacterData2[] selectedTeam = new CharacterData2[5]; // ���õ� �� ����

    private void Start()
    {
        for (int i = 0; i < teamSlots.Length; i++)
        {
            int index = i;
            teamSlots[i].onClick.AddListener(() => SelectCharacter(index));
        }

        startAdventureButton.onClick.AddListener(StartAdventure);
    }

    void SelectCharacter(int slotIndex)
    {
        // ���� ������ ĳ���� UI�� ����, ���� �� �Ʒ� �Լ� ȣ���ϵ���
        characterListUI.OpenForTeamSelect((selected) =>
        {
            selectedTeam[slotIndex] = selected;
            teamSlots[slotIndex].GetComponentInChildren<TextMeshProUGUI>().text = selected.characterName;
        });
    }

    void StartAdventure()
    {
        List<CharacterData2> teamList = new List<CharacterData2>();

        foreach (var member in selectedTeam)
        {
            if (member != null)
                teamList.Add(member);
        }

        if (teamList.Count == 0)
        {
            Debug.Log("1�� �̻� �����ؾ� ������ ������ �� �ֽ��ϴ�.");
            return;
        }

        Debug.Log($"���� ����! ���õ� �ο�: {teamList.Count}��");
        foreach (var c in teamList)
        {
            Debug.Log($"- {c.characterName}");
        }



        // ���� �ܰ��: ���� ���� �� �̵� or ������ ����
        // ��: AdventureManager.Instance.StartAdventure(teamList);
    }

}
