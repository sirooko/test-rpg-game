// Scripts/Summon/SummonManagerBridge.cs
using UnityEngine;
using UnityEngine.Events;

public class SummonManagerBridge : MonoBehaviour
{
    [Header("Map rarity-key �� actual summon methods (fill in Inspector)")]
    public UnityEvent onCommon;  // ��: SummonManager.SummonCommon()
    public UnityEvent onRare;    // ��: SummonManager.SummonRare()
    public UnityEvent onEpic;    // ��: SummonManager.SummonEpic()

    /// <summary>
    /// SummonPortalController���� rarityKey("Common"/"Rare"/"Epic")�� �޾� ���� �޼��� ����
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
