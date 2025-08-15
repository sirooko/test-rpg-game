// Scripts/Runtime/GameSession.cs
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public AdventureMapData currentMap { get; private set; }
    public PartyRuntime currentParty { get; private set; }

    BattleRequest pendingBattle;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void BeginAdventure(System.Collections.Generic.List<CharacterData2> team, AdventureMapData map)
    {
        currentMap = map;
        currentParty = PartyRuntime.FromCharacters(team); // 선택한 팀을 런타임 파티로
    }

    public void RequestBattle(EnemyGroupData group, string stageId = null)
    {
        pendingBattle = new BattleRequest { playerParty = currentParty, enemyGroup = group, stageId = stageId };
    }

    public BattleRequest ConsumeBattleRequest()
    {
        var r = pendingBattle;
        pendingBattle = null;
        return r;
    }
}