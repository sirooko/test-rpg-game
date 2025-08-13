// CurrencyTextBinder.cs (UnityEngine.UI.Text 사용)
using UnityEngine;
using UnityEngine.UI;

public class CurrencyTextBinder : MonoBehaviour
{
    [SerializeField] private Text text;

    void OnEnable()
    {
        TryHook();
    }

    void OnDisable()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnChanged -= UpdateText;
    }

    void TryHook()
    {
        var mgr = CurrencyManager.Instance ?? FindAnyObjectByType<CurrencyManager>();
        if (mgr != null)
        {
            // 중복 구독 방지용으로 한 번 제거 후 등록
            mgr.OnChanged -= UpdateText;
            mgr.OnChanged += UpdateText;
            UpdateText(mgr.Stones);
        }
        else
        {
            Debug.LogWarning("[CurrencyTextBinder] CurrencyManager not found.");
        }
    }

    void UpdateText(int value)
    {
        if (text) text.text = $"💎 {value}";
    }
}