using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)] // 다른 UI들보다 먼저 깔리게
public class AdventureManager : MonoBehaviour
{
    public static AdventureManager Instance { get; private set; }

    [SerializeField] private AdventureStageUI stageUI;
    [SerializeField] private GameObject adventurePanel;

    public bool isVictory;
    public List<PartyMemberResult> partyResults;

    AdventureMapData mapData;

    private int currentStage = 1;
    private int maxStage = 5;

    // 기존: List<CharacterData2> team;
    private List<CharacterInAdventure> team;

    private StageEventData currentEventData; // 현재 스테이지 이벤트

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[AdventureManager] Duplicate detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // DontDestroyOnLoad는 단일 씬/패널 라우팅이면 굳이 필요 X
        // DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void StartAdventure(List<CharacterData2> selectedTeam, AdventureMapData selectedMap)
    {
        mapData = selectedMap;
        currentStage = 1;
        maxStage = Mathf.Min(mapData.maxStage, mapData.stages.Count);

        // 원본 데이터 → 모험용 런타임 캐릭터로 변환
        team = new List<CharacterInAdventure>();
        foreach (var data in selectedTeam)
            team.Add(new CharacterInAdventure(data));

        // UI 초기화
        adventurePanel.SetActive(true);
        stageUI.SetTeamUI(team);
        stageUI.SetMaxStage(maxStage);
        stageUI.SetBackground(mapData.backgroundImage);

        ProceedToStage();
    }

    void ProceedToStage()
    {
        if (currentStage > mapData.stages.Count)
        {
            Debug.Log("더 이상 진행할 스테이지가 없습니다. 모험이 종료됩니다.");
            EndAdventure();
            return;
        }

        currentEventData = mapData.stages[currentStage - 1];

        stageUI.SetupStage(
            currentStage,
            currentEventData.eventDescription,
            currentEventData.option1Text,
            currentEventData.option2Text
        );

        stageUI.choice1Button.onClick.RemoveAllListeners();
        stageUI.choice2Button.onClick.RemoveAllListeners();

        stageUI.choice1Button.onClick.AddListener(() => OnChooseOption(true));
        stageUI.choice2Button.onClick.AddListener(() => OnChooseOption(false));
    }

    void OnChooseOption(bool isOption1)
    {
        bool isFight = isOption1 ? currentEventData.option1IsFight : currentEventData.option2IsFight;

        if (isFight)
        {
            // ✅ 변경: 임시 전투(FakeBattle) 대신 실제 전투 연결
            TriggerBattleForCurrentEvent();
        }
        else
        {
            // 전투 없음 → 간단 페널티/연출 후 다음 스테이지
            foreach (var c in team)
            {
                if (c.isDead) continue;
                c.currentStress += 5;
                Debug.Log($"{c.Name}의 스트레스 +5 → {c.currentStress}");
            }

            stageUI.SetTeamUI(team);
            AdvanceOrEnd();
        }
    }

    // ✅ 추가: 현재 이벤트의 전투를 시작
    void TriggerBattleForCurrentEvent()
    {
        Debug.Log(">> 전투 시작 준비");

        // 1) Adventure UI는 켜둔 채로, 전투 UI를 오버레이처럼 켠다
        //    (UIFlowManager가 BattleCanvas 켜고 BattleManager.StartBattle 호출)
        StageData stage = currentEventData.battleStage; // <-- A안 사용

        if (stage == null)
        {
            Debug.LogWarning("battleStage가 비었습니다. 테스트 전투로 진행합니다.");
        }

        // 2) 전투 진입 (전투는 콜백으로 Adventure로 복귀)
        UIFlowManager.Instance.ToBattle(stage);
    }

    // ✅ 추가: BattleManager가 EndBattle 후 호출하는 콜백
    public void OnBattleFinished(BattleResult result)
    {
        Debug.Log("<< 전투 종료 콜백 수신");

        // 1) 전투 결과를 팀에 반영 (HP/사망/스트레스 등)
        ApplyBattleResultToTeam(result);

        // 2) 전투 UI 닫고 모험 화면으로 복귀
        UIFlowManager.Instance.CloseBattleToAdventure();

        // 3) UI 갱신
        stageUI.SetTeamUI(team);

        // 4) 전원 사망 체크 → 종료
        if (IsTeamAllDead())
        {
            StartCoroutine(EndAdventureWithDelay("모든 팀원이 쓰러졌습니다... 모험 실패!"));
            return;
        }

        // 5) 다음 스테이지로 진행 or 종료
        AdvanceOrEnd();
    }

    // ✅ 추가: 전투 결과를 현재 모험 팀에 반영
    void ApplyBattleResultToTeam(BattleResult result)
    {
        if (result == null)
        {
            Debug.LogWarning("BattleResult가 null입니다. 결과 반영을 건너뜁니다.");
            return;
        }

        // 예시: result에 파티원별 결과가 있을 경우 매칭(이름/ID 기준)
        // result.partyResults: List<PartyMemberResult> 가정
        // PartyMemberResult { string name; int hp; bool isDead; int stressDelta; ... }

        if (result.partyResults != null)
        {
            foreach (var pr in result.partyResults)
            {
                var member = team.Find(t => t.Name == pr.name); // ID가 있다면 ID 매칭 권장
                if (member == null) continue;

                member.currentHP = Mathf.Clamp(pr.hp, 0, member.MaxHP);
                member.isDead = pr.isDead;
                member.currentStress = Mathf.Max(0, member.currentStress + pr.stressDelta);
            }
        }

        // 승리 보상(간단 예시) — 나중에 RewardDistributor로 이동 권장
        if (result.isVictory)
        {
            // 예: 마력석 100 지급
            // InventoryManager.Instance.AddStones(100);
            Debug.Log("보상: 마력석 +100 (예시)");
        }
    }

    // ✅ 추가: 다음 스테이지로 진행 or 종료
    void AdvanceOrEnd()
    {
        currentStage++;

        if (currentStage > mapData.stages.Count)
        {
            StartCoroutine(EndAdventureWithDelay("모험 완료! 수고했습니다."));
        }
        else
        {
            ProceedToStage();
        }
    }

    bool IsTeamAllDead()
    {
        foreach (var c in team)
            if (!c.isDead) return false;
        return true;
    }

    void EndAdventure()
    {
        Debug.Log("모험 종료!");
        adventurePanel.SetActive(false);

        stageUI.choice1Button.onClick.RemoveAllListeners();
        stageUI.choice2Button.onClick.RemoveAllListeners();

        // TODO: 날짜 경과, 자동 저장 등
        // DateManager.Instance.GoToEvening();
        // SaveManager.Instance.AutoSave();
    }

    IEnumerator EndAdventureWithDelay()
    {
        Debug.Log("도망쳤습니다. 당신은 싸움을 피했지만... 모험은 여기서 끝났습니다.");
        yield return new WaitForSeconds(2f);
        EndAdventure();
    }

    IEnumerator EndAdventureWithDelay(string message)
    {
        Debug.Log(message);
        yield return new WaitForSeconds(2f);
        EndAdventure();
    }
}
