using System.Collections.Generic;
using UnityEngine;

public enum BaseSection
{
    None, CastleDoor, SummonPortal, GoddessStatue, DemonicMerchant,
    Storage, Castle, TrainingRoom, RestRoom, Pub, SecretGarden
}

public class BaseScreenRouter : MonoBehaviour
{
    [System.Serializable] public struct Pane { public BaseSection section; public GameObject panel; }

    [Header("Panels")]
    public GameObject selectionPanel;          // 버튼 10개가 있는 패널
    public List<Pane> panes = new();           // 각 섹션 패널 매핑

    void Start() => Show(BaseSection.None);    // 시작은 선택지 화면

    public void Show(BaseSection target)
    {
        // 선택지 on/off
        if (selectionPanel) selectionPanel.SetActive(target == BaseSection.None);

        // 나머지 패널 토글
        foreach (var p in panes)
            if (p.panel) p.panel.SetActive(p.section == target && target != BaseSection.None);
    }

    // 버튼에서 쓰기 쉽게 int/이름 버전도 제공
    public void ShowByIndex(int idx) => Show((BaseSection)idx);
    public void ShowByName(string name)
    {
        if (System.Enum.TryParse(name, out BaseSection s)) Show(s);
    }
}
