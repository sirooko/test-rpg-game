using UnityEngine;
using UnityEngine.UI;

public class AdventureMapButton : MonoBehaviour
{
    public Text mapNameText;
    public Image thumbnail;
    private AdventureMapData mapData;

    private TeamSelectUI teamSelectUI;

    public void Initialize(AdventureMapData data, TeamSelectUI teamUI)
    {
        mapData = data;
        teamSelectUI = teamUI;

        mapNameText.text = data.mapName;
        thumbnail.sprite = data.thumbnail;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        teamSelectUI.SetSelectedMap(mapData);               // 🔹 중요!
        teamSelectUI.gameObject.SetActive(true);            // 🔹 팀 선택 UI 열기
    }
}
