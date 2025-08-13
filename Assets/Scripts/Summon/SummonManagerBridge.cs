// Scripts/Summon/SummonManagerBridge.cs
using UnityEngine;
using UnityEngine.Events;

public class SummonManagerBridge : MonoBehaviour
{
    [Header("Map rarity-key → actual summon methods (fill in Inspector)")]
    public UnityEvent onCommon;  // 예: SummonManager.SummonCommon()
    public UnityEvent onRare;    // 예: SummonManager.SummonRare()
    public UnityEvent onEpic;    // 예: SummonManager.SummonEpic()

    /// <summary>
    /// SummonPortalController에서 rarityKey("Common"/"Rare"/"Epic")를 받아 실제 메서드 실행
    /// </summary>
    public void SummonByKey(string key)
    {
        key = (key ?? "").Trim().ToLowerInvariant();
        switch (key)
        {
            case "common": onCommon?.Invoke(); break;
            case "rare": onRare?.Invoke(); break;
            case "epic": onEpic?.Invoke(); break;
            default:
                Debug.LogWarning($"[SummonManagerBridge] Unknown rarity key: {key}");
                break;
        }
    }
}
