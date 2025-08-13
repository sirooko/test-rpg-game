using System.Linq;
using UnityEngine;

public enum AppScreen { Base, Adventure, Battle, Result }

public class UIFlowManager : MonoBehaviour
{
    public static UIFlowManager Instance { get; private set; }

    [Header("Top-level roots")]
    [SerializeField] GameObject baseRoot;       // MainCanvas
    [SerializeField] GameObject adventureRoot;  // AdventurePanel (없으면 만들어줘)
    [SerializeField] GameObject battleRoot;     // BattleCanvas

    [Header("Battle sub-roots (under BattleCanvas)")]
    [SerializeField] GameObject battlePlayRoot;    // BattlePanel
    [SerializeField] GameObject battleResultRoot;  // BattleResultPanel

    public enum AppScreen
    {
        Base,
        MapList,      // 성문 → 맵 리스트
        TeamSelect,   // 팀 편성(소환/보유 중 선택)
        Adventure,    // AdventureStageUI (이벤트 진행)
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

        // 전투 시작
        BattleManager.Instance?.StartBattle(stage);
    }

    public void ShowBattleResult()
    {
        if (battlePlayRoot) battlePlayRoot.SetActive(false);
        if (battleResultRoot) battleResultRoot.SetActive(true);
    }

    public void CloseBattleToBase()
    {
        // 전투가 끝났을 때 런타임 생성물 정리
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
