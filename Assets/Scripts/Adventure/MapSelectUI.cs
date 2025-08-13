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
                Debug.Log($"맵 선택: {map.mapName}");
                // 여기서 팀 선택 UI로 넘기거나 AdventureManager 호출
                // TeamSelectUI.Open(map); 등
            });
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
