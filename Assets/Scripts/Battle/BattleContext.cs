// Scripts/Runtime/BattleContext.cs
using System.Collections.Generic;
using UnityEngine;

public class BattleContext : MonoBehaviour
{
    public static BattleContext Instance { get; private set; }

    public List<CharacterInAdventure> playerTeam;
    public StageData stage;
    public string stageId;

    public List<CharacterData2> enemyTemplates; // ★ 추가

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void Set(List<CharacterInAdventure> team, StageData s, List<CharacterData2> enemies = null, string id = null)
    {
        playerTeam = team != null ? new List<CharacterInAdventure>(team) : null;
        stage = s;
        stageId = id;

        // 우선순위: 외부 override > Stage 기본 적 리스트
        if (enemies != null && enemies.Count > 0) enemyTemplates = new List<CharacterData2>(enemies);
        else if (stage != null && stage.enemyTemplates != null && stage.enemyTemplates.Count > 0)
            enemyTemplates = new List<CharacterData2>(stage.enemyTemplates);
        else
            enemyTemplates = null;
    }

    public void Clear()
    {
        playerTeam = null; stage = null; stageId = null; enemyTemplates = null;
    }

    public static void Ensure()
    {
        if (Instance == null) new GameObject("BattleContext").AddComponent<BattleContext>();
    }
}