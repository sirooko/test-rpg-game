// Scripts/Base/Objects/AdventureGateController.cs
using UnityEngine;
using UnityEngine.Events;

public class AdventureGateController : MonoBehaviour
{
    [Header("Hooks (�ν����Ϳ��� �巡�׷� ����)")]
    public UnityEvent onOpenMapSelect;   // AdventureUI.OpenMapSelect() ��
    public UnityEvent onStartDefault;    // AdventureManager.StartAdventure("Map_001") ��
    public UnityEvent onPreStart;        // (����) ����/ȿ��
    public UnityEvent onPostStart;       // (����) �α�/UI

    [Header("�ɼ�")]
    public bool useDefaultIfNoUI = true; // �ʼ��� ������ ������ �⺻ ����

    public void OpenMapSelect()
    {
        onPreStart?.Invoke();

        if (onOpenMapSelect != null && onOpenMapSelect.GetPersistentEventCount() > 0)
            onOpenMapSelect.Invoke();
        else if (useDefaultIfNoUI)
            onStartDefault?.Invoke();

        onPostStart?.Invoke();
    }
}