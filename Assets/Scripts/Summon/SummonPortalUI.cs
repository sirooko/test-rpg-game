using UnityEngine;
using UnityEngine.UI;

public class SummonPortalUI : MonoBehaviour
{
    public void OnBasicSummon()
    {
        Debug.Log("�⺻ ���� ��ȯ �õ�!");
        // ��ȯ ���� �� ��� ó�� �߰� ����
    }

    public void OnAdvancedSummon()
    {
        Debug.Log("��� ���� ��ȯ �õ�!");
        // ��� ��ȯ ���� �� ��� ó�� �߰� ����
    }

    public void OnUpgradePortal()
    {
        Debug.Log("��Ż ���׷��̵� �õ�!");
        // ���׷��̵� Ȯ�� �� ��� ����
    }

    public void OnExit()
    {
        gameObject.SetActive(false);
    }
}
