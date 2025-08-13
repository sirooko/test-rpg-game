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
    public GameObject selectionPanel;          // ��ư 10���� �ִ� �г�
    public List<Pane> panes = new();           // �� ���� �г� ����

    void Start() => Show(BaseSection.None);    // ������ ������ ȭ��

    public void Show(BaseSection target)
    {
        // ������ on/off
        if (selectionPanel) selectionPanel.SetActive(target == BaseSection.None);

        // ������ �г� ���
        foreach (var p in panes)
            if (p.panel) p.panel.SetActive(p.section == target && target != BaseSection.None);
    }

    // ��ư���� ���� ���� int/�̸� ������ ����
    public void ShowByIndex(int idx) => Show((BaseSection)idx);
    public void ShowByName(string name)
    {
        if (System.Enum.TryParse(name, out BaseSection s)) Show(s);
    }
}
