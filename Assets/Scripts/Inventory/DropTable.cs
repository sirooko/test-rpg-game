using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "ScriptableObjects/DropTable")]
public class DropTable : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public ItemData item;
        [Min(0f)] public float weight = 1f; // °¡ÁßÄ¡ (0ÀÌ¸é µå¶ø ¾ÈµÊ)
        [Min(1)] public int minCount = 1;
        [Min(1)] public int maxCount = 1;
    }

    public List<Entry> entries = new();

    public DropReward GetRandomDrop()
    {
        // ÃÑ °¡ÁßÄ¡
        float total = 0f;
        foreach (var e in entries) if (e != null && e.item != null && e.weight > 0) total += e.weight;
        if (total <= 0f) return null;

        // ·ê·¿ ¼±ÅÃ
        float r = Random.value * total;
        foreach (var e in entries)
        {
            if (e == null || e.item == null || e.weight <= 0) continue;
            if (r < e.weight)
            {
                int count = Random.Range(e.minCount, e.maxCount + 1);
                return new DropReward(e.item, count);
            }
            r -= e.weight;
        }
        return null;
    }
}
