// Scripts/Base/Objects/SummonPortalController.cs
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class StringEvent : UnityEvent<string> { }

public class SummonPortalController : MonoBehaviour
{
    [Header("Costs")]
    public int costCommon = 50;
    public int costRare = 150;
    public int costEpic = 450;

    [Header("Hooks")]
    public UnityEvent onPreSummon;      // SummonEffectManager.StartSummonEffect
    public StringEvent onSummonWithKey; // SummonManagerBridge.SummonByKey
    public UnityEvent onPostSummon;     // (선택)

    // 중복 호출/더블클릭 방지
    bool isSummoning;

    public void SummonCommon() => DoSummon(costCommon, "Common");
    public void SummonRare() => DoSummon(costRare, "Rare");
    public void SummonEpic() => DoSummon(costEpic, "Epic");

    void DoSummon(int cost, string key)
    {
        if (isSummoning) return;
        isSummoning = true;

        // 1) 비용 차감(한 번만)
        if (CurrencyManager.Instance && !CurrencyManager.Instance.SpendCurrency(cost))
        {
            Debug.Log("재화 부족: 소환 실패");
            isSummoning = false;
            return;
        }

        // 2) 이펙트 → 실제 소환
        onPreSummon?.Invoke();
        onSummonWithKey?.Invoke(key);

        // 3) 사후 처리(있으면)
        onPostSummon?.Invoke();

        isSummoning = false; // 이펙트가 길다면 onPostSummon 끝에서 false로 내려도 됨
    }
}
