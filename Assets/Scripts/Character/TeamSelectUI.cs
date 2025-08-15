using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Transform characterListPanel;      // 보유 캐릭터 목록 영역 (Vertical Layout Group)
    public Transform teamSlotPanel;           // 팀 슬롯 영역 (Horizontal Layout Group)
    public GameObject characterSlotPrefab;    // 보유 캐릭터 프리팹
    public GameObject teamSlotPrefab;         // 팀 슬롯 프리팹
    public Button startAdventureButton;       // 모험 시작 버튼
    public Button closeCharacterListButton;   // 캐릭터 목록 닫기 버튼

    [Header("Refs")]
    public AdventureManager adventureManager; // 인스펙터에서 연결

    [Header("Settings")]
    [SerializeField] private int maxTeamSize = 5;

    private AdventureMapData selectedMapData;
    private readonly List<CharacterData2> currentTeam = new List<CharacterData2>();
    private List<CharacterData2> ownedCharacters;
    private int selectedSlotIndex = -1;

    // 외부에서 맵 지정
    public void SetSelectedMap(AdventureMapData map)
    {
        selectedMapData = map;
        // 필요하면 맵 바뀔 때 팀 초기화:
        // currentTeam.Clear();
        // RefreshUI();
    }

    private void OnEnable()
    {
        selectedSlotIndex = -1;

        // 보유 캐릭터 로드
        if (CharacterInventoryManager.Instance != null)
            ownedCharacters = CharacterInventoryManager.Instance.GetAllCharacters();
        else
            ownedCharacters = new List<CharacterData2>();

        RefreshUI();

        if (closeCharacterListButton != null)
        {
            closeCharacterListButton.onClick.RemoveAllListeners();
            closeCharacterListButton.onClick.AddListener(() =>
                characterListPanel.gameObject.SetActive(false));
        }
    }

    private void RefreshUI()
    {
        if (!characterListPanel || !teamSlotPanel) return;

        // 기존 슬롯 제거 (CloseButton 제외)
        ClearChildrenExcept(characterListPanel, "CloseButton");
        ClearChildrenExcept(teamSlotPanel, "CloseButton");

        // 캐릭터 목록 생성
        if (ownedCharacters != null)
        {
            foreach (var ch in ownedCharacters)
            {
                var slotObj = Instantiate(characterSlotPrefab, characterListPanel);

                var nameTf = slotObj.transform.Find("CharacterName");
                var imageTf = slotObj.transform.Find("CharacterImage");
                var infoBtnT = slotObj.transform.Find("CharacterInformation");

                if (nameTf) nameTf.GetComponent<Text>()?.SetTextSafe(ch.characterName);
                if (imageTf) imageTf.GetComponent<Image>().sprite = ch.characterSprite;

                var c = ch; // ★ 캡처 안전
                var btn = infoBtnT ? infoBtnT.GetComponent<Button>() : slotObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => SelectCharacterForSlot(c));
                }
            }
        }

        // 팀 슬롯 생성
        for (int i = 0; i < maxTeamSize; i++)
        {
            int index = i; // ★ 캡처 안전
            var slot = Instantiate(teamSlotPrefab, teamSlotPanel);

            var imgTf = slot.transform.Find("CharacterImage");
            if (imgTf)
            {
                var img = imgTf.GetComponent<Image>();
                bool hasMember = i < currentTeam.Count && currentTeam[i] != null;
                img.gameObject.SetActive(hasMember);
                if (hasMember) img.sprite = currentTeam[i].characterSprite;
            }

            var btn = slot.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnTeamSlotClicked(index));
            }
        }

        // 버튼 상태
        startAdventureButton.interactable = currentTeam.Any(m => m != null);

        // 캐릭터 목록 기본은 숨김
        if (characterListPanel) characterListPanel.gameObject.SetActive(false);
    }

    private void OnTeamSlotClicked(int index)
    {
        if (index < 0 || index >= maxTeamSize) return;
        selectedSlotIndex = index;
        if (characterListPanel) characterListPanel.gameObject.SetActive(true);
    }

    private void SelectCharacterForSlot(CharacterData2 character)
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= maxTeamSize) return;
        if (character == null) return;

        // 중복 방지
        if (currentTeam.Contains(character)) return;

        // 빈칸을 확보하며 지정
        while (currentTeam.Count <= selectedSlotIndex)
            currentTeam.Add(null);

        currentTeam[selectedSlotIndex] = character;

        if (characterListPanel) characterListPanel.gameObject.SetActive(false);
        RefreshUI();
    }

    // 모험 시작
    public void OnClickStartAdventure()
    {
        if (selectedMapData == null)
        {
            Debug.LogError("❌ selectedMapData가 설정되지 않았습니다!");
            return;
        }
        if (adventureManager == null)
        {
            Debug.LogError("❌ AdventureManager가 연결되지 않았습니다!");
            return;
        }

        // null 없는 팀 구성으로 전달
        var team = currentTeam.Where(m => m != null).ToList();
        if (team.Count == 0)
        {
            Debug.LogWarning("팀이 비어 있습니다.");
            return;
        }

        Debug.Log($"모험 시작! 맵: {selectedMapData.mapName}, 인원: {team.Count}");
        adventureManager.StartAdventure(team, selectedMapData);

        // 팀 선택 UI 닫기
        gameObject.SetActive(false);
    }

    // 유틸
    private void ClearChildrenExcept(Transform parent, string exceptName)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i);
            if (child.name == exceptName) continue;
            Destroy(child.gameObject);
        }
    }
}

//static class TextExt
//{
//    public static void SetTextSafe(this Text t, string s)
//    {
//        if (t) t.text = s ?? "";
//    }
//}
