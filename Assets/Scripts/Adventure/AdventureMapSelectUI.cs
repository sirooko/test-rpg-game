using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureMapSelectUI : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject mapSlotPrefab;
    public List<AdventureMapData> maps;

    public GameObject teamSelectPanel; // �� �� ���� UI ���� �߰�

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
        Debug.Log($"�� ����: {map.mapName}");
        // ����: �� ���� �� ���� ���� UI�� ����

        // ���õ� �� �����͸� �� ���� UI�� ����
    if (teamSelectUI != null)
        {
            teamSelectUI.SetSelectedMap(map);  // �� ���� �߰��ؾ� ���� �ذ�
            teamSelectPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("teamSelectUI�� ������� �ʾҽ��ϴ�!");
        }
    }
}
