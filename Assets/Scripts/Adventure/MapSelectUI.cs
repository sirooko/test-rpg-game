using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapSelectUI : MonoBehaviour
{
    public Transform buttonParent;
    public GameObject mapButtonPrefab;
    public List<AdventureMapData> allMaps;

    public void OpenMapSelect()
    {
        gameObject.SetActive(true);
        RefreshMapButtons();
    }

    void RefreshMapButtons()
    {
        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        foreach (AdventureMapData map in allMaps)
        {
            GameObject btnObj = Instantiate(mapButtonPrefab, buttonParent);
            btnObj.GetComponentInChildren<Text>().text = map.mapName;

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"�� ����: {map.mapName}");
                // ���⼭ �� ���� UI�� �ѱ�ų� AdventureManager ȣ��
                // TeamSelectUI.Open(map); ��
            });
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
