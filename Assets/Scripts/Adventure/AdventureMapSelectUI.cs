using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureMapSelectUI : MonoBehaviour
{
    [Header("Panels & Prefab")]
    public Transform contentPanel;
    public GameObject mapSlotPrefab;
    public GameObject mapListContainer;   // ★ 맵 리스트 부모(없으면 contentPanel.gameObject 사용)
    public GameObject teamSelectPanel;

    [Header("Data")]
    public List<AdventureMapData> maps;

    [Header("Team Select")]
    public TeamSelectUI teamSelectUI;

    void OnEnable() => Open();  // 기존 흐름 유지

    public void Open()
    {
        if (mapListContainer == null && contentPanel != null)
            mapListContainer = contentPanel.gameObject;

        if (mapListContainer) mapListContainer.SetActive(true);
        if (teamSelectPanel) teamSelectPanel.SetActive(false);

        RebuildList();
    }

    void RebuildList()
    {
        if (contentPanel == null || mapSlotPrefab == null) return;

        // 기존 항목 정리
        for (int i = contentPanel.childCount - 1; i >= 0; i--)
            Destroy(contentPanel.GetChild(i).gameObject);

        // 슬롯 생성
        foreach (AdventureMapData map in maps)
        {
            GameObject slot = Instantiate(mapSlotPrefab, contentPanel);

            var nameTf = slot.transform.Find("MapName");
            var bgTf = slot.transform.Find("backgroundImage");
            var thumbTf = slot.transform.Find("tumbnail"); // 프리팹 이름이 이렇다면 유지

            if (nameTf) nameTf.GetComponent<Text>()?.SetTextSafe(map.mapName);
            if (bgTf) bgTf.GetComponent<Image>().sprite = map.backgroundImage;
            if (thumbTf) thumbTf.GetComponent<Image>().sprite = map.thumbnail;

            // ★ 캡처 안전
            var m = map;
            var btn = slot.GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(() => SelectMap(m));
        }
    }

    void SelectMap(AdventureMapData map)
    {
        Debug.Log($"맵 선택: {map.mapName}");

        if (teamSelectUI != null)
        {
            teamSelectUI.SetSelectedMap(map);
            if (mapListContainer) mapListContainer.SetActive(false); // ★ 리스트 숨기기
            if (teamSelectPanel) teamSelectPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("teamSelectUI가 연결되지 않았습니다!");
        }
    }
}

// 작은 편의 확장
public static class TextExt
{
    public static void SetTextSafe(this Text t, string s) { if (t) t.text = s ?? ""; }
}
