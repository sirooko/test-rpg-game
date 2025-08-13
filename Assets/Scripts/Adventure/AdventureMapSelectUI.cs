using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureMapSelectUI : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject mapSlotPrefab;
    public List<AdventureMapData> maps;

    public GameObject teamSelectPanel; // ← 팀 선택 UI 참조 추가

    public TeamSelectUI teamSelectUI;

    private void OnEnable()
    {
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        foreach (AdventureMapData map in maps)
        {
            GameObject slot = Instantiate(mapSlotPrefab, contentPanel);
            slot.transform.Find("MapName").GetComponent<Text>().text = map.mapName;
            slot.transform.Find("backgroundImage").GetComponent<Image>().sprite = map.backgroundImage;
            slot.transform.Find("tumbnail").GetComponent<Image>().sprite = map.thumbnail;


            Button btn = slot.GetComponent<Button>();
            btn.onClick.AddListener(() => SelectMap(map));
        }
    }

    void SelectMap(AdventureMapData map)
    {
        Debug.Log($"맵 선택: {map.mapName}");
        // 이후: 팀 선택 → 모험 시작 UI로 연결

        // 선택된 맵 데이터를 팀 선택 UI에 전달
    if (teamSelectUI != null)
        {
            teamSelectUI.SetSelectedMap(map);  // 이 라인 추가해야 오류 해결
            teamSelectPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("teamSelectUI가 연결되지 않았습니다!");
        }
    }
}
