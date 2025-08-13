// Scripts/Battle/BattleResult.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleResult
{
    public bool isVictory;

    // 전투 후 파티원별 상태
    public List<PartyMemberResult> partyResults = new();

    // ✅ 추가: 보상 정보
    public int stoneReward;                       // 마력석 보상
    public List<DropReward> drops = new();        // 아이템 드랍
}

// 파티원 결과
[System.Serializable]
public class PartyMemberResult
{
    public string id;      // 가능하면 ID로 매칭
    public string name;    // 없으면 이름으로 매칭

    public int hp;
    public bool isDead;

    public int stressDelta; // 선택
}

// 드랍 보상 (아이템 + 개수)
[System.Serializable]
public class DropReward
{
    public ItemData item;
    public int count;

    public DropReward(ItemData item, int count = 1)
    {
        this.item = item;
        this.count = Mathf.Max(1, count);
    }
}