using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSelectManager : MonoBehaviour
{
    public Button[] teamSlots; // 팀 슬롯 버튼 5개
    public Button startAdventureButton; // 모험 시작 버튼
    public CharacterListUI characterListUI; // 기존 캐릭터 목록 UI 참조

    

    private CharacterData2[] selectedTeam = new CharacterData2[5]; // 선택된 팀 구성

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
        // 선택 가능한 캐릭터 UI를 띄우고, 선택 후 아래 함수 호출하도록
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
            Debug.Log("1명 이상 선택해야 모험을 시작할 수 있습니다.");
            return;
        }

        Debug.Log($"모험 시작! 선택된 인원: {teamList.Count}명");
        foreach (var c in teamList)
        {
            Debug.Log($"- {c.characterName}");
        }



        // 다음 단계로: 실제 모험 씬 이동 or 시퀀스 실행
        // 예: AdventureManager.Instance.StartAdventure(teamList);
    }

}
