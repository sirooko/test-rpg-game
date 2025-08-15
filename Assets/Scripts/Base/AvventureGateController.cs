// Scripts/Base/Objects/AdventureGateController.cs
using UnityEngine;
using UnityEngine.Events;

public class AdventureGateController : MonoBehaviour
{
    [Header("Hooks (인스펙터에서 드래그로 연결)")]
    public UnityEvent onOpenMapSelect;   // AdventureUI.OpenMapSelect() 등
    public UnityEvent onStartDefault;    // AdventureManager.StartAdventure("Map_001") 등
    public UnityEvent onPreStart;        // (선택) 사운드/효과
    public UnityEvent onPostStart;       // (선택) 로그/UI

    [Header("옵션")]
    public bool useDefaultIfNoUI = true; // 맵선택 연결이 없으면 기본 시작

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