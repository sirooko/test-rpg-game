using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ItemStack
{
    public ItemData item;
    public int count;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Persist")]
    [SerializeField] private bool dontDestroyOnLoad = true;

    [Header("Currency")]
    [SerializeField] private int stones;   // ���¼�

    [Header("Items (for inspector)")]
    [SerializeField] private List<ItemStack> itemsInInspector = new List<ItemStack>();

    // ���� ��ųʸ� (��Ÿ�� ������)
    private Dictionary<ItemData, int> itemDict = new Dictionary<ItemData, int>();

    // �̺�Ʈ (UI ���� � ���)
    public UnityEvent OnInventoryChanged = new UnityEvent();
    public UnityEvent OnStonesChanged = new UnityEvent();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

        // �ν����� ��� �� ��ųʸ��� ����ȭ
        itemDict.Clear();
        foreach (var stack in itemsInInspector)
        {
            if (stack?.item == null || stack.count <= 0) continue;
            if (!itemDict.ContainsKey(stack.item)) itemDict[stack.item] = 0;
            itemDict[stack.item] += stack.count;
        }
    }

    #region Stones
    public int GetStones() => stones;

    public void AddStones(int amount)
    {
        if (amount <= 0) return;
        stones += amount;
        OnStonesChanged?.Invoke();
    }

    public bool SpendStones(int amount)
    {
        if (amount <= 0) return true;
        if (stones < amount) return false;
        stones -= amount;
        OnStonesChanged?.Invoke();
        return true;
    }
    #endregion

    #region Items
    public bool HasItem(ItemData item, int need = 1)
    {
        if (item == null || need <= 0) return false;
        return itemDict.TryGetValue(item, out var cnt) && cnt >= need;
    }

    public int GetCount(ItemData item)
    {
        if (item == null) return 0;
        return itemDict.TryGetValue(item, out var cnt) ? cnt : 0;
    }

    public void AddItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0) return;
        if (!itemDict.ContainsKey(item)) itemDict[item] = 0;
        itemDict[item] += count;
        SyncInspectorList();
        OnInventoryChanged?.Invoke();
    }

    public bool ConsumeItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0) return false;
        if (!itemDict.TryGetValue(item, out var cur) || cur < count) return false;

        cur -= count;
        if (cur <= 0) itemDict.Remove(item);
        else itemDict[item] = cur;

        SyncInspectorList();
        OnInventoryChanged?.Invoke();
        return true;
    }

    public List<ItemData> GetUsableItems()
    {
        var result = new List<ItemData>();
        if (itemDict == null || itemDict.Count == 0) return result;

        foreach (var kv in itemDict)
        {
            var item = kv.Key;
            var count = kv.Value;
            if (item == null) continue;

            // ��� �ְ� ���� ��� �����ϸ� ��Ͽ� �߰�
            if (count > 0 && item.isUsableInBattle)
                result.Add(item);
        }

        // ���� ���� ���� (���ϴ� �������� ���� ����)
        result.Sort((a, b) => string.Compare(a.itemName, b.itemName, System.StringComparison.Ordinal));
        return result;
    }

    private void SyncInspectorList()
    {
        itemsInInspector.Clear();
        foreach (var kv in itemDict)
        {
            itemsInInspector.Add(new ItemStack { item = kv.Key, count = kv.Value });
        }
    }
    #endregion
}
