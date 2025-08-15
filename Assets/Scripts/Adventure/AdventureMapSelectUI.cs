using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureMapSelectUI : MonoBehaviour
{
    [Header("Panels & Prefab")]
    public Transform contentPanel;
    public GameObject mapSlotPrefab;
    public GameObject mapListContainer;   // �� �� ����Ʈ �θ�(������ contentPanel.gameObject ���)
    public GameObject teamSelectPanel;

    [Header("Data")]
    public List<AdventureMapData> maps;

    [Header("Team Select")]
    public TeamSelectUI teamSelectUI;

    void OnEnable() => Open();  // ���� �帧 ����

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

        // ���� �׸� ����
        for (int i = contentPanel.childCount - 1; i >= 0; i--)
            Destroy(contentPanel.GetChild(i).gameObject);

        // ���� ����
        foreach (AdventureMapData map in maps)
        {
            GameObject slot = Instantiate(mapSlotPrefab, contentPanel);

            var nameTf = slot.transform.Find("MapName");
            var bgTf = slot.transform.Find("backgroundImage");
            var thumbTf = slot.transform.Find("tumbnail"); // ������ �̸��� �̷��ٸ� ����

            if (nameTf) nameTf.GetComponent<Text>()?.SetTextSafe(map.mapName);
            if (bgTf) bgTf.GetComponent<Image>().sprite = map.backgroundImage;
            if (thumbTf) thumbTf.GetComponent<Image>().sprite = map.thumbnail;

            // �� ĸó ����
            var m = map;
            var btn = slot.GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(() => SelectMap(m));
        }
    }

    void SelectMap(AdventureMapData map)
    {
        Debug.Log($"�� ����: {map.mapName}");

        if (teamSelectUI != null)
        {
            teamSelectUI.SetSelectedMap(map);
            if (mapListContainer) mapListContainer.SetActive(false); // �� ����Ʈ �����
            if (teamSelectPanel) teamSelectPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("teamSelectUI�� ������� �ʾҽ��ϴ�!");
        }
    }
}

// ���� ���� Ȯ��
public static class TextExt
{
    public static void SetTextSafe(this Text t, string s) { if (t) t.text = s ?? ""; }
}
