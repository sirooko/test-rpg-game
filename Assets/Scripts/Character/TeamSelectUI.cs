using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Transform characterListPanel;         // 보유 캐릭터 목록 영역 (Vertical Layout Group)
    public Transform teamSlotPanel;              // 팀 슬롯 영역 (Horizontal Layout Group)
    public GameObject characterSlotPrefab;       // 캐릭터 슬롯 프리팹 (보유 목록에 사용)
    public GameObject teamSlotPrefab;            // 팀 슬롯 프리팹 (선택된 팀원 표시)
    public Button startAdventureButton;          // 모험 시작 버튼

    public Button closeCharacterListButton;

    public AdventureManager adventureManager; // 인스펙터 연결

    AdventureMapData selectedMapData;

    private List<CharacterData2> currentTeam = new List<CharacterData2>();
    private List<CharacterData2> ownedCharacters;

    public void SetSelectedMap(AdventureMapData map)
    {
        selectedMapData = map;
    }

    private void OnEnable()
    {
        currentTeam.Clear();
        ownedCharacters = CharacterInventoryManager.Instance.GetAllCharacters();
        RefreshUI();

        closeCharacterListButton.onClick.RemoveAllListeners();  // 중복 방지
        closeCharacterListButton.onClick.AddListener(() => characterListPanel.gameObject.SetActive(false));
    }

    void RefreshUI()
    {


        // 기존 캐릭터 슬롯/팀 슬롯 제거
        foreach (Transform child in characterListPanel)
        {
            if (child.name != "CloseButton") // 닫기 버튼 제외
            {
                Destroy(child.gameObject);
            }
        }
        foreach (Transform child in teamSlotPanel)
        {
            if (child.name != "CloseButton") // 닫기 버튼 제외
            {
                Destroy(child.gameObject);
            }
        }

        // 캐릭터 목록 생성
        foreach (var character in ownedCharacters)
        {
            GameObject slotObj = Instantiate(characterSlotPrefab, characterListPanel);
            slotObj.transform.Find("CharacterName").GetComponent<Text>().text = character.characterName;
            slotObj.transform.Find("CharacterImage").GetComponent<Image>().sprite = character.characterSprite;

            Button btn = slotObj.transform.Find("CharacterInformation").GetComponent<Button>();
            btn.onClick.AddListener(() => SelectCharacterForSlot(character));
        }

        /// 팀 슬롯 생성
        for (int i = 0; i < 5; i++)
        {
            int index = i; // 캡처 방지
            GameObject slot = Instantiate(teamSlotPrefab, teamSlotPanel);

            // 슬롯 이미지 설정
            if (i < currentTeam.Count)
            {
                slot.transform.Find("CharacterImage").GetComponent<Image>().sprite = currentTeam[i].characterSprite;
                slot.transform.Find("CharacterImage").gameObject.SetActive(true);
            }
            else
            {
                slot.transform.Find("CharacterImage").gameObject.SetActive(false);
            }

            // 슬롯에 버튼 클릭 이벤트 추가
            Button btn = slot.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnTeamSlotClicked(index));
            }
        }


        // 버튼 상태 갱신
        startAdventureButton.interactable = currentTeam.Count > 0;
    }

    int selectedSlotIndex = -1;

    void OnTeamSlotClicked(int index)
    {
        selectedSlotIndex = index;
        characterListPanel.gameObject.SetActive(true); // 캐릭터 목록 보이기
    }


    void SelectCharacterForSlot(CharacterData2 character)
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= 5)
            return;

        if (currentTeam.Contains(character)) return;

        if (selectedSlotIndex < currentTeam.Count)
        {
            currentTeam[selectedSlotIndex] = character;
        }
        else
        {
            // 빈 슬롯에 추가
            while (currentTeam.Count <= selectedSlotIndex)
                currentTeam.Add(null);

            currentTeam[selectedSlotIndex] = character;
        }

        characterListPanel.gameObject.SetActive(false); // 목록 닫기
        RefreshUI(); // UI 갱신
    }


    // 버튼에 연결
    public void OnClickStartAdventure()
    {
        if (selectedMapData == null)
        {
            Debug.LogError("❌ selectedMapData가 설정되지 않았습니다!");
            return;
        }

        Debug.Log("모험 시작!");
        adventureManager.StartAdventure(currentTeam, selectedMapData);
        gameObject.SetActive(false);  // 팀 선택 UI 닫기
    }
}
