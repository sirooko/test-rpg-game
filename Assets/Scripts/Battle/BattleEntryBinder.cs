using UnityEngine;

public class BattleEntryBinder : MonoBehaviour
{
    public BattleManager battleManager;
    void OnEnable()
    {
        var ctx = BattleContext.Instance;
        if (ctx == null || ctx.stage == null)
        { Debug.LogError("[BattleEntryBinder] BattleContext 미준비"); return; }

        battleManager.StartBattle(ctx.stage); // ← 현재 서명에 맞춤
    }
}
