using System.Linq;
using UnityEngine;

public enum AppScreen { Base, Adventure, Battle, Result }

public class UIFlowManager : MonoBehaviour
{
    public static UIFlowManager Instance { get; private set; }

    [Header("Top-level roots")]
    [SerializeField] GameObject baseRoot;       // MainCanvas
    [SerializeField] GameObject adventureRoot;  // AdventurePanel (������ �������)
    [SerializeField] GameObject battleRoot;     // BattleCanvas

    [Header("Battle sub-roots (under BattleCanvas)")]
    [SerializeField] GameObject battlePlayRoot;    // BattlePanel
    [SerializeField] GameObject battleResultRoot;  // BattleResultPanel

    public enum AppScreen
    {
        Base,
        MapList,      // ���� �� �� ����Ʈ
        TeamSelect,   // �� ��(��ȯ/���� �� ����)
        Adventure,    // AdventureStageUI (�̺�Ʈ ����)
        Battle,
        Result
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void ShowOnly(params GameObject[] on)
    {
        var all = new[] { baseRoot, adventureRoot, battleRoot }.Where(x => x != null);
        foreach (var go in all) go.SetActive(on.Contains(go));
    }

    public void ToBase()
    {
        ShowOnly(baseRoot);
    }

    public void ToAdventure()
    {
        ShowOnly(adventureRoot);
    }

    public void ToBattle(StageData stage = null)
    {
        ShowOnly(battleRoot);
        if (battlePlayRoot) battlePlayRoot.SetActive(true);
        if (battleResultRoot) battleResultRoot.SetActive(false);

        // ���� ����
        BattleManager.Instance?.StartBattle(stage);
    }

    public void ShowBattleResult()
    {
        if (battlePlayRoot) battlePlayRoot.SetActive(false);
        if (battleResultRoot) battleResultRoot.SetActive(true);
    }

    public void CloseBattleToBase()
    {
        // ������ ������ �� ��Ÿ�� ������ ����
        BattleManager.Instance?.Cleanup();
        ToBase();
    }

    public void CloseBattleToAdventure()
    {
        BattleManager.Instance?.Cleanup();
        ToAdventure();

        if (AdventureManager.Instance == null)
            Debug.LogWarning("AdventureManager.Instance is null when returning to Adventure.");
    }
}
