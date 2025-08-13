using System;
using UnityEngine;
using UnityEngine.UI; // ← Text 전용

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int currentCurrency = 1000;   // 시작값
    public int Stones => currentCurrency;

    // UI가 구독할 이벤트
    public event Action<int> OnChanged;

    [Header("OPTIONAL - 기존 씬 직결 텍스트 (비워도 동작)")]
    [SerializeField] private Text currencyText;            // ← 레거시 Text

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
    }

    private void Start() => RaiseChanged();

    public void AddCurrency(int amount)
    {
        currentCurrency = Mathf.Max(0, currentCurrency + amount);
        RaiseChanged();
    }

    public bool SpendCurrency(int amount)
    {
        if (currentCurrency < amount)
        {
            Debug.Log("재화 부족");
            return false;
        }
        currentCurrency -= amount;
        RaiseChanged();
        return true;
    }

    private void RaiseChanged()
    {
        OnChanged?.Invoke(currentCurrency);
        if (currencyText != null) currencyText.text = $"💎 {currentCurrency}";
    }
}
