using UnityEngine;

public class BattleEntryBinder : MonoBehaviour
{
    public BattleManager battleManager;
    void OnEnable()
    {
        var ctx = BattleContext.Instance;
        if (ctx == null || ctx.stage == null)
        { Debug.LogError("[BattleEntryBinder] BattleContext ���غ�"); return; }

        battleManager.StartBattle(ctx.stage); // �� ���� ���� ����
    }
}
