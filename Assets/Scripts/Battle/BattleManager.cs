using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class BattleManager : MonoBehaviour
{
    // 🔷 싱글톤
    public static BattleManager Instance { get; private set; }

    public Text logText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // 🔷 팀 구성
    public List<CombatCharacter> playerTeam = new List<CombatCharacter>();
    public List<CombatCharacter> enemyTeam = new List<CombatCharacter>();

    private Queue<CombatCharacter> turnQueue = new Queue<CombatCharacter>();

    public CombatCharacter currentCharacter;

    private bool isTurnRunning = false;

    private BattleResult lastResult;  // ✅ 최종 결과를 여기에 저장

    // BattleManager.cs (필드)
    private bool isBattleEnded = false;

    [SerializeField] private GameObject resultPanel;   // 결과 패널 (비활성화로 두기)
    [SerializeField] private ResultPanel resultPanelUI; // resultPanel에 붙어있는 스크립트
    [SerializeField] private Text resultText;          // "승리!" / "패배..."
    [SerializeField] private string nextSceneName = "BaseScene";

    [SerializeField] private bool autoStartOnAwake = false;

    private void Start()
    {
        if (autoStartOnAwake)
            StartCoroutine(InitBattle());
    }

    public void StartBattle(StageData stage = null)
    {
        StopAllCoroutines();
        isBattleEnded = false;
        isTurnRunning = false;

        Cleanup(); // 혹시 이전 전투 잔재가 있으면 정리
        StartCoroutine(InitBattle()); // stage를 쓰려면 InitBattle(stage) 오버로드
    }

    public void Cleanup()
    {
        // 코루틴/상태 정리
        StopAllCoroutines();
        isTurnRunning = false;

        // 캐릭터 오브젝트 제거
        foreach (var c in playerTeam) if (c != null) Destroy(c.gameObject);
        foreach (var e in enemyTeam) if (e != null) Destroy(e.gameObject);
        playerTeam.Clear();
        enemyTeam.Clear();

        // UI 정리
        BattleUIManager.Instance?.ClearAllCharacterUI();
    }

    // 🔷 전투 시작 초기화(test)
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private CharacterData2 testPlayerData;
    [SerializeField] private CharacterData2 testEnemyData;
    [SerializeField] private SkillData testSkill;

    [SerializeField] private GameObject characterUIPrefab;
    [SerializeField] private Transform playerPanel;
    [SerializeField] private Transform enemyPanel;

    // 각 CombatCharacter 생성 후 UI도 함께 생성
    private void CreateCharacterUI(CombatCharacter character, bool isEnemy)
    {
        Transform parentPanel = isEnemy ? enemyPanel : playerPanel;

        GameObject uiObj = Instantiate(characterUIPrefab, parentPanel);
        CombatCharacterUI ui = uiObj.GetComponent<CombatCharacterUI>();

        if (ui == null)
        {
            Debug.LogError("❌ characterUIPrefab에 CombatCharacterUI 컴포넌트가 없습니다!");
            return;
        }

        character.ui = ui;  // 연결
        ui.Setup(character, character.characterData.battleSprite, isEnemy);

        // 🔹 BattleUIManager에 등록!
        BattleUIManager.Instance.characterUIList.Add(ui);
    }


    private IEnumerator InitBattle()
    {
        playerTeam = new List<CombatCharacter>();
        enemyTeam = new List<CombatCharacter>();

        Debug.Log("⚔ 5대5 InitBattle 시작");

        // 1. 플레이어 캐릭터 5명 생성
        for (int i = 0; i < 5; i++)
        {
            GameObject playerObj = Instantiate(playerPrefab);
            CombatCharacter playerCombat = playerObj.GetComponent<CombatCharacter>();

            if (playerCombat == null)
            {
                Debug.LogError($"❌ PlayerPrefab에 CombatCharacter 컴포넌트가 없습니다! (Index: {i})");
                continue;
            }

            playerCombat.InitFromData(testPlayerData, true);
            playerCombat.skills.Add(testSkill); // 테스트 스킬 추가
            playerTeam.Add(playerCombat);

            CreateCharacterUI(playerCombat, isEnemy: false);
        }

        // 2. 적 캐릭터 5명 생성
        for (int i = 0; i < 5; i++)
        {
            GameObject enemyObj = Instantiate(enemyPrefab);
            CombatCharacter enemyCombat = enemyObj.GetComponent<CombatCharacter>();

            if (enemyCombat == null)
            {
                Debug.LogError($"❌ EnemyPrefab에 CombatCharacter 컴포넌트가 없습니다! (Index: {i})");
                continue;
            }

            enemyCombat.InitFromData(testEnemyData, false);
            enemyCombat.skills.Add(testSkill); // 테스트 스킬 추가
            enemyTeam.Add(enemyCombat);

            CreateCharacterUI(enemyCombat, isEnemy: true);
        }

        // 3. 대기 후 턴 시작
        yield return new WaitForSeconds(1f);
        InitTurnQueue();
        StartCoroutine(HandleTurns());

        // 4. 디버그 출력
        Debug.Log($"PlayerTeam Count: {playerTeam.Count}");
        foreach (var p in playerTeam)
        {
            Debug.Log($"✔ Player: {p.characterData.characterName}");
        }

        Debug.Log($"EnemyTeam Count: {enemyTeam.Count}");
        foreach (var e in enemyTeam)
        {
            Debug.Log($"✔ Enemy: {e.characterData.characterName}");
        }
    }

    // 🔹 턴 순서를 민첩(AGI) 순으로 초기화
    private void InitTurnQueue()
    {
        Debug.Log("🌀 [InitTurnQueue] 턴 큐 초기화 시작");

        var allCharacters = playerTeam.Concat(enemyTeam)
                                       .Where(c => c != null && c.isAlive)
                                       .OrderByDescending(c => c.AGI) // 민첩순 내림차순
                                       .ToList();

        turnQueue = new Queue<CombatCharacter>(allCharacters);

        foreach (var c in allCharacters)
        {
            Debug.Log($"👉 [InitTurnQueue] {c.characterData.characterName} 등록됨 (AGI: {c.AGI}, isPlayer: {c.isPlayer})");
        }

        Debug.Log($"✅ [InitTurnQueue] 큐 구성 완료 (총 {turnQueue.Count}명)");
    }

    // 🔷 턴 핸들링
    // 🔷 턴 전체 순환 처리 코루틴
    private IEnumerator HandleTurns()
    {
        if (isTurnRunning || isBattleEnded) yield break;
        isTurnRunning = true;

        Debug.Log("🔁 [HandleTurns] 전투 턴 시작");

        while (!isBattleEnded)
        {
            // 라운드 시작 전 생존 체크
            int aliveP = playerTeam.Count(c => c != null && c.IsAlive());
            int aliveE = enemyTeam.Count(c => c != null && c.IsAlive());
            Debug.Log($"[AliveCheck] begin round  P:{aliveP}, E:{aliveE}, ended:{isBattleEnded}");

            if (aliveP == 0 || aliveE == 0) break;

            // AGI 기준 턴 큐 구성
            InitTurnQueue();

            // 큐 소진까지 라운드 진행
            while (turnQueue.Count > 0 && !isBattleEnded)
            {
                currentCharacter = turnQueue.Dequeue();
                if (currentCharacter == null || !currentCharacter.IsAlive())
                {
                    // 죽었거나 무효면 스킵
                    continue;
                }

                // 상태이상(DoT 등) 처리 — 반드시 내부에서 TakeDamage() 사용하도록 수정되어 있어야 함
                currentCharacter.ProcessStatusEffects();

                // DOT 등으로 방금 죽었으면 스킵
                if (!currentCharacter.IsAlive())
                {
                    Debug.Log($"☠️ DOT로 사망: {currentCharacter.characterData.characterName} - 턴 스킵");
                    continue;
                }

                // 기절 처리 (턴 소모)
                var stun = currentCharacter.activeStatusEffects
                    .FirstOrDefault(e => e.effectType == StatusEffectType.Stun);

                if (stun != null)
                {
                    Debug.Log($"💫 {currentCharacter.characterData.characterName} 기절 - 턴 스킵");
                    BattleLogManager.Instance.ShowLog($"{currentCharacter.characterData.characterName}은(는) 기절 상태입니다! 턴을 스킵합니다.");
                    yield return new WaitForSeconds(1f);

                    // 기절 턴 소모
                    stun.remainingTurns--;
                    if (stun.remainingTurns <= 0)
                        currentCharacter.activeStatusEffects.Remove(stun);

                    // ❗ 여기서 절대 NextTurn() 호출하지 말 것
                    currentCharacter.hasActed = true;
                    continue;
                }

                // 실제 행동
                if (currentCharacter.isPlayer)
                {
                    Debug.Log($"🧑‍🎮 플레이어 턴: {currentCharacter.characterData.characterName}");
                    currentCharacter.hasActed = false;
                    BattleUIManager.Instance.ShowMainOptions(currentCharacter);

                    // 전투 종료 중에도 빠져나오도록 가드
                    yield return new WaitUntil(() => currentCharacter.hasActed || isBattleEnded);
                    Debug.Log("✅ 플레이어 행동 완료");
                }
                else
                {
                    Debug.Log($"👾 적 턴: {currentCharacter.characterData.characterName}");
                    currentCharacter.hasActed = false;

                    // EnemyTurn 내부에서도 NextTurn() 부르지 않도록! (hasActed만 true로 세팅)
                    yield return StartCoroutine(EnemyTurn(currentCharacter));
                    yield return new WaitUntil(() => currentCharacter.hasActed || isBattleEnded);
                    Debug.Log("✅ 적 행동 완료");
                }

                if (isBattleEnded) break;
                yield return new WaitForSeconds(0.3f); // 턴 간 간격
            }

            // 라운드 종료 로그
            aliveP = playerTeam.Count(c => c != null && c.IsAlive());
            aliveE = enemyTeam.Count(c => c != null && c.IsAlive());
            Debug.Log($"[AliveCheck] end round    P:{aliveP}, E:{aliveE}, ended:{isBattleEnded}");

            // 다음 라운드 전 잠깐 대기
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("🏁 전투 종료 조건 만족 - EndBattle 호출(HandleTurns)");
        if (!isBattleEnded) EndBattle();   // 여기에서만 호출
        isTurnRunning = false;
    }

    // 🔷 플레이어 턴 종료 호출
    // EndPlayerTurn도 재시동 방지 (보통 HandleTurns가 계속 돌고 있으면 필요 없음)
    public void EndPlayerTurn()
    {
        if (isBattleEnded) return;         // ✅
        currentCharacter.hasActed = true;
        BattleUIManager.Instance.DisableAllActionButtons();
    }


    // 🔷 전투 종료 처리
    // 전투 종료
    private void EndBattle()
    {
        if (isBattleEnded) return;
        isBattleEnded = true;

        BattleUIManager.Instance?.HideAllButtons();
        isTurnRunning = false;
        turnQueue.Clear();

        bool playerWon = playerTeam.Any(p => p != null && p.IsAlive());
        lastResult = BuildBattleResult(playerWon);

        // 결과 패널 ON (BattleCanvas 내부 전환)
        UIFlowManager.Instance.ShowBattleResult();

        AdventureManager.Instance.OnBattleFinished(lastResult);

        //// 결과 UI에 바인딩이 필요하면 여기서
        //resultPanelUI?.Show(lastResult, () => UIFlowManager.Instance.CloseBattleToBase());
    }

    // BuildBattleResult는 의존성 없이 동작하게(드랍/경험치는 임시)
    [SerializeField] private DropTable dropTableRef;
    private BattleResult BuildBattleResult(bool playerWon)
    {
        var result = new BattleResult
        {
            isVictory = playerWon,
            stoneReward = playerWon ? 100 : 0,   // 예시: 승리 시 100
            drops = new List<DropReward>(),
            partyResults = new List<PartyMemberResult>()
        };

        // 파티원 결과 기록
        foreach (var p in playerTeam)
        {
            if (p == null) continue;
            result.partyResults.Add(new PartyMemberResult
            {
                id = p.characterData ? p.characterData.characterId : null,
                name = p.characterData ? p.characterData.characterName : p.name,
                hp = p.currentHP,
                isDead = !p.IsAlive(),
                stressDelta = playerWon ? -5 : +10 // 예시
            });
        }

        if (playerWon && dropTableRef != null)
        {
            // 예: 최대 1개만 드랍 (원하면 루프 돌려 여러 개 가능)
            var drop = dropTableRef.GetRandomDrop();
            if (drop != null) result.drops.Add(drop);
        }

        return result;
    }

    private IEnumerator ClearLogRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (logText != null)
            logText.text = "";
    }


    // 🔷 대상 선택 표시
    public enum ActionType
    {
        Attack,
        Skill,
        Item
    }

    public void ShowTargetSelection(ActionType actionType, SkillData selectedSkill = null, ItemData selectedItem = null)
    {
        if (isBattleEnded) return;

        List<CombatCharacter> targets = null;

        switch (actionType)
        {
            case ActionType.Attack:
                targets = enemyTeam.FindAll(t => t.IsAlive());
                break;

            case ActionType.Skill:
                if (selectedSkill == null)
                {
                    Debug.LogError("❌ 스킬이 null입니다!");
                    return;
                }

                switch (selectedSkill.targetType)
                {
                    case SkillTargetType.Enemy:
                        targets = enemyTeam.FindAll(t => t.IsAlive());
                        break;

                    case SkillTargetType.Ally:
                        targets = playerTeam.FindAll(t => t.IsAlive());
                        break;

                    case SkillTargetType.Self:
                        UseSkill(currentCharacter, currentCharacter, selectedSkill);
                        currentCharacter.hasActed = true;
                        return;

                    case SkillTargetType.AllEnemies:
                        {
                            var allEnemies = enemyTeam.Where(t => t.IsAlive()).ToList();
                            if (allEnemies.Count == 0) { Debug.Log("⚠️ 대상 없음"); EndPlayerTurn(); return; }

                            UseSkillAoE(currentCharacter, selectedSkill, allEnemies);
                            currentCharacter.hasActed = true;
                            return;
                        }

                    case SkillTargetType.AllAllies:
                        {
                            var allAllies = playerTeam.Where(t => t.IsAlive()).ToList();
                            if (allAllies.Count == 0) { Debug.Log("⚠️ 대상 없음"); EndPlayerTurn(); return; }

                            UseSkillAoE(currentCharacter, selectedSkill, allAllies);
                            currentCharacter.hasActed = true;
                            return;
                        }
                }
                break;

            case ActionType.Item:
                targets = playerTeam.FindAll(t => t.IsAlive());
                break;
        }

        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("❌ 유효한 대상이 없습니다.");
            return;
        }

        // ✅ 단일 대상 선택 UI
        BattleUIManager.Instance.ShowTargetSelection(targets, (CombatCharacter target) =>
        {
            switch (actionType)
            {
                case ActionType.Attack:
                    ExecuteAttack(currentCharacter, target);
                    break;
                case ActionType.Skill:
                    UseSkill(currentCharacter, target, selectedSkill);
                    break;
                case ActionType.Item:
                    UseItem(currentCharacter, target, selectedItem);
                    break;
            }
            currentCharacter.hasActed = true;
        });
    }

    // 🔷 공격 처리
    public void ExecuteAttack(CombatCharacter attacker, CombatCharacter target)
    {
        if (attacker == null || target == null || !attacker.isAlive || !target.isAlive)
            return;

        int effectiveATK = attacker.ATK;

        if (attacker.HasStatus(StatusEffectType.Weaken))
        {
            effectiveATK = Mathf.Max(1, attacker.ATK / 2);
            BattleLogManager.Instance.ShowLog($"{attacker.characterData.characterName}은(는) 쇠약 상태로 인해 공격력이 감소했습니다!");
        }

        int damage = Mathf.Max(1, effectiveATK - target.DEF);

        BattleLogManager.Instance.ShowLog($"{attacker.characterData.characterName}이(가) {target.characterData.characterName}에게 {damage} 피해!");

        target.TakeDamage(damage);

        BattleUIManager.Instance.HideAllButtons();  // ✅ UI 숨기기
        EndPlayerTurn();  // ✅ 턴 종료 (이 안에서 HandleTurns 호출 ❌)
    }

    public void UseSkill(CombatCharacter user, CombatCharacter target, SkillData skill)
    {
        if (user.currentMP < skill.manaCost)
        {
            BattleLogManager.Instance.ShowLog($"{user.characterData.characterName}은(는) MP가 부족합니다!");
            EndPlayerTurn();
            return;
        }
        user.currentMP = Mathf.Max(0, user.currentMP - skill.manaCost);
        user.ui?.UpdateStats();

        if (target == null && (skill.targetType == SkillTargetType.Enemy || skill.targetType == SkillTargetType.Ally))
        { EndPlayerTurn(); return; }

        switch (skill.type)
        {
            case SkillType.Damage:
                int rawATK = user.ATK;

                if (!skill.isMagic && user.HasStatus(StatusEffectType.Weaken))
                {
                    rawATK = Mathf.Max(1, rawATK / 2);
                    BattleLogManager.Instance.ShowLog($"{user.characterData.characterName}은(는) 쇠약 상태로 물리 데미지가 감소합니다!");
                }

                int rawDamage = skill.isMagic
                    ? Mathf.Max(1, user.MAG - target.RES + skill.power)
                    : Mathf.Max(1, rawATK - target.DEF + skill.power);

                ApplyDamage(target, rawDamage);
                BattleLogManager.Instance.ShowLog($"{user.characterData.characterName}의 스킬 {skill.skillName} → {target.characterData.characterName}에게 {rawDamage} 피해!");
                break;


            case SkillType.Heal:
                int heal = Mathf.Max(1, skill.power + user.MAG);
                target.Heal(heal);  // ✅ 핵심 수정 포인트!
                BattleLogManager.Instance.ShowLog($"{user.characterData.characterName}의 스킬 {skill.skillName} → {target.characterData.characterName}을 {heal} 회복!");
                break;
            case SkillType.Buff:
                BattleLogManager.Instance.ShowLog($"{target.characterData.characterName}에게 버프 효과!");
                break;

            case SkillType.Debuff:
                BattleLogManager.Instance.ShowLog($"{target.characterData.characterName}에게 디버프 효과!");
                break;

            case SkillType.Cleanse:
                {
                    // 대상은 targetType에 따라 이미 아군/적이 넘어옴(우린 보통 Ally/Self/AllAllies로 설계)
                    if (TryCleanse(target, skill.statusEffect))
                    {
                        BattleLogManager.Instance.ShowLog(
                            $"{target.characterData.characterName}의 {skill.statusEffect} 상태가 해제되었습니다.");
                    }
                    else
                    {
                        BattleLogManager.Instance.ShowLog("해제할 상태이상이 없습니다.");
                    }
                    break;
                }
        }

        // 상태이상 적용
        if (skill.statusEffect != StatusEffectType.None)
        {
            int duration = 1;

            switch (skill.statusEffect)
            {
                case StatusEffectType.Stun:
                    duration = 1;
                    break;
                case StatusEffectType.Bleed:
                case StatusEffectType.Poison:
                    duration = Random.Range(1, 5);
                    break;
                default:
                    duration = 2;
                    break;
            }

            target.ApplyStatusEffect(skill.statusEffect, duration);
        }

        // 사망 체크
        //if (target.currentHP <= 0)
        //{
        //    target.currentHP = 0;
        //    target.isAlive = false;
        //    BattleLogManager.Instance.ShowLog($"{target.characterData.characterName}이(가) 쓰러졌습니다!");
        //}

        EndPlayerTurn();
    }

    private void UseSkillAoE(CombatCharacter user, SkillData skill, List<CombatCharacter> targets)
    {
        if (isBattleEnded) return;

        if (targets == null || targets.Count == 0) { EndPlayerTurn(); return; }

        switch (skill.type)
        {
            case SkillType.Damage:
                {
                    foreach (var t in targets.Where(t => t.IsAlive()))
                    {
                        int rawATK = user.ATK;
                        if (!skill.isMagic && user.HasStatus(StatusEffectType.Weaken))
                        {
                            rawATK = Mathf.Max(1, rawATK / 2);
                        }
                        int dmg = skill.isMagic
                            ? Mathf.Max(1, user.MAG - t.RES + skill.power)
                            : Mathf.Max(1, rawATK - t.DEF + skill.power);

                        ApplyDamage(t, dmg);
                        BattleLogManager.Instance.ShowLog($"{user.characterData.characterName}의 [{skill.skillName}] → {t.characterData.characterName}에게 {dmg} 피해!");

                        // 상태이상(있으면) 적용
                        if (skill.statusEffect != StatusEffectType.None)
                            ApplySkillStatusToTarget(t, skill.statusEffect);
                    }
                    break;
                }

            case SkillType.Heal:
                {
                    foreach (var t in targets.Where(t => t.IsAlive()))
                    {
                        int heal = Mathf.Max(1, skill.power + user.MAG);
                        t.Heal(heal);
                        BattleLogManager.Instance.ShowLog($"{user.characterData.characterName}의 [{skill.skillName}] → {t.characterData.characterName} {heal} 회복!");
                    }
                    break;
                }

            case SkillType.Buff:
                {
                    foreach (var t in targets.Where(t => t.IsAlive()))
                    {
                        // 필요 시 버프 로직 구현
                        BattleLogManager.Instance.ShowLog($"{t.characterData.characterName}에게 버프 효과!");
                        if (skill.statusEffect != StatusEffectType.None)
                            ApplySkillStatusToTarget(t, skill.statusEffect);
                    }
                    break;
                }

            case SkillType.Debuff:
                {
                    foreach (var t in targets.Where(t => t.IsAlive()))
                    {
                        BattleLogManager.Instance.ShowLog($"{t.characterData.characterName}에게 디버프 효과!");
                        if (skill.statusEffect != StatusEffectType.None)
                            ApplySkillStatusToTarget(t, skill.statusEffect);
                    }
                    break;
                }

            case SkillType.Cleanse:
                {
                    int removedCount = TryCleanseMany(targets, skill.statusEffect);
                    if (removedCount > 0)
                        BattleLogManager.Instance.ShowLog($"[{skill.skillName}] {skill.statusEffect} 해제: {removedCount}명");
                    else
                        BattleLogManager.Instance.ShowLog("해제할 상태이상이 없습니다.");
                    break;
                }
        }

        BattleUIManager.Instance.HideAllButtons();
        BattleUIManager.Instance.UpdateAllCharacterUI();

        EndPlayerTurn();
    }

    private void ApplySkillStatusToTarget(CombatCharacter target, StatusEffectType effect)
    {
        int duration = 1;
        switch (effect)
        {
            case StatusEffectType.Stun: duration = 1; break;
            case StatusEffectType.Bleed:
            case StatusEffectType.Poison: duration = Random.Range(1, 5); break;
            default: duration = 2; break;
        }
        target.ApplyStatusEffect(effect, duration);
    }

    public void UseItem(CombatCharacter user, CombatCharacter target, ItemData item)
    {
        if (isBattleEnded) return;
        if (user == null || target == null) { return; }
        if (!target.isAlive) { BattleLogManager.Instance.ShowLog("대상이 행동 불가 상태입니다."); return; }

        // 재고 확인
        if (InventoryManager.Instance != null && !InventoryManager.Instance.HasItem(item))
        {
            BattleLogManager.Instance.ShowLog("아이템이 없습니다!");
            // ❌ 턴 종료하지 않고 목록으로 복귀
            BattleUIManager.Instance.ShowItemList(user);
            return;
        }

        bool effectApplied = false;

        switch (item.type)
        {
            case ItemType.HealHP:
                {
                    int need = target.MaxHP - target.currentHP;
                    int healed = Mathf.Clamp(item.healAmount, 0, need);
                    if (healed > 0)
                    {
                        target.Heal(healed);
                        BattleLogManager.Instance.ShowLog($"{target.characterData.characterName}의 HP가 {healed} 회복되었습니다!");
                        effectApplied = true;
                    }
                    else
                    {
                        BattleLogManager.Instance.ShowLog("이미 HP가 가득 찼습니다.");
                    }
                    break;
                }

            case ItemType.HealMP:
                {
                    int before = target.currentMP;
                    target.RecoverMP(item.healAmount);
                    int gained = target.currentMP - before;
                    if (gained > 0)
                    {
                        BattleLogManager.Instance.ShowLog($"{target.characterData.characterName}의 MP가 {gained} 회복되었습니다!");
                        effectApplied = true;
                    }
                    else
                    {
                        BattleLogManager.Instance.ShowLog("이미 MP가 가득 찼습니다.");
                    }
                    break;
                }

            case ItemType.HealStatus:
                {
                    if (target.TryRemoveStatus(item.statusToHeal))
                    {
                        BattleLogManager.Instance.ShowLog($"{target.characterData.characterName}의 {item.statusToHeal} 상태가 치료되었습니다.");
                        effectApplied = true;
                    }
                    else
                    {
                        BattleLogManager.Instance.ShowLog("해제할 상태이상이 없습니다.");
                    }
                    break;
                }

                // TODO: 부활/버프/공격형 등 추가 시 여기에 확장
        }

        if (effectApplied)
        {
            // ✅ 효과가 있었을 때만 소비/턴 종료
            InventoryManager.Instance?.ConsumeItem(item);
            BattleUIManager.Instance.HideAllButtons();
            BattleUIManager.Instance.UpdateAllCharacterUI();
            EndPlayerTurn();                 // hasActed = true는 여기서 처리
        }
        else
        {
            // ❌ 효과 없으면 턴 유지 + 아이템 목록으로 복귀
            BattleUIManager.Instance.UpdateAllCharacterUI();
            BattleUIManager.Instance.ShowItemList(user);
            // currentCharacter.hasActed 는 그대로 false
        }
    }

    public void UseItemAoE(CombatCharacter user, ItemData item, List<CombatCharacter> targets)
    {
        if (isBattleEnded) return;
        if (user == null) return;

        if (targets == null || targets.Count == 0)
        {
            BattleLogManager.Instance.ShowLog("사용할 대상이 없습니다.");
            // 목록으로 복귀 (턴 유지)
            BattleUIManager.Instance.ShowItemList(user);
            return;
        }

        if (InventoryManager.Instance != null && !InventoryManager.Instance.HasItem(item))
        {
            BattleLogManager.Instance.ShowLog("아이템이 없습니다!");
            BattleUIManager.Instance.ShowItemList(user);
            return;
        }

        bool anyApplied = false;

        switch (item.type)
        {
            case ItemType.HealHP:
                {
                    foreach (var t in targets)
                    {
                        if (t == null || !t.isAlive) continue;
                        int need = t.MaxHP - t.currentHP;
                        int healed = Mathf.Clamp(item.healAmount, 0, need);
                        if (healed > 0)
                        {
                            t.Heal(healed);
                            BattleLogManager.Instance.ShowLog($"{t.characterData.characterName}의 HP가 {healed} 회복되었습니다!");
                            anyApplied = true;
                        }
                    }
                    break;
                }

            case ItemType.HealMP:
                {
                    foreach (var t in targets)
                    {
                        if (t == null || !t.isAlive) continue;
                        int before = t.currentMP;
                        t.RecoverMP(item.healAmount);
                        int gained = t.currentMP - before;
                        if (gained > 0)
                        {
                            BattleLogManager.Instance.ShowLog($"{t.characterData.characterName}의 MP가 {gained} 회복되었습니다!");
                            anyApplied = true;
                        }
                    }
                    break;
                }

            case ItemType.HealStatus:
                {
                    int removed = 0;
                    foreach (var t in targets)
                    {
                        if (t == null || !t.isAlive) continue;
                        if (t.TryRemoveStatus(item.statusToHeal))
                        {
                            removed++;
                            BattleLogManager.Instance.ShowLog($"{t.characterData.characterName}의 {item.statusToHeal} 상태가 치료되었습니다.");
                            anyApplied = true;
                        }
                    }
                    if (!anyApplied)
                        BattleLogManager.Instance.ShowLog("해제할 상태이상이 없습니다.");
                    break;
                }
        }

        BattleUIManager.Instance.UpdateAllCharacterUI();

        if (anyApplied)
        {
            // ✅ 효과가 1명 이상에게 있었을 때만 소비/턴 종료
            InventoryManager.Instance?.ConsumeItem(item);
            BattleUIManager.Instance.HideAllButtons();
            EndPlayerTurn();
        }
        else
        {
            // ❌ 효과 없으면 턴 유지 + 아이템 목록 복귀
            BattleUIManager.Instance.ShowItemList(user);
        }
    }

    private void ApplyItemEffect(CombatCharacter target, ItemData item)
    {
        switch (item.type)
        {
            case ItemType.HealHP:
                target.Heal(item.healAmount);
                BattleLogManager.Instance.ShowLog($"{target.characterData.characterName}이(가) HP를 {item.healAmount} 회복했습니다.");
                break;

            case ItemType.HealMP:
                target.RecoverMP(item.healAmount);
                BattleLogManager.Instance.ShowLog($"{target.characterData.characterName}이(가) MP를 {item.healAmount} 회복했습니다.");
                break;

            case ItemType.Revive:
                if (!target.isAlive)
                {
                    target.currentHP = Mathf.Min(item.healAmount, target.MaxHP);
                    target.isAlive = true;
                    BattleLogManager.Instance.ShowLog($"{target.characterData.characterName}이(가) 부활했습니다! (HP {item.healAmount})");
                }
                break;

            case ItemType.HealStatus:
                target.TryRemoveStatus(item.statusToHeal);
                BattleLogManager.Instance.ShowLog($"{target.characterData.characterName}의 {item.statusToHeal} 상태가 치료되었습니다.");
                break;

                // 필요시 추가 케이스 작성 가능
        }
    }

    public List<CombatCharacter> GetValidTargets(bool isPlayer)
    {
        if (isPlayer)
        {
            return enemyTeam.Where(c => c.isAlive).ToList();
        }
        else
        {
            return playerTeam.Where(c => c.isAlive).ToList();
        }
    }

    public void Defend(CombatCharacter character)
    {
        character.isDefending = true;
        BattleLogManager.Instance.ShowLog($"{character.characterData.characterName}이(가) 방어 태세를 취합니다.");
        EndPlayerTurn();
    }

    public void ApplyDamage(CombatCharacter target, int amount)
    {
        int finalDamage = amount;
        if (target.isDefending)
        {
            finalDamage = Mathf.Max(1, amount / 2); // 피해 반감
            target.isDefending = false; // 방어는 1회용
        }

        target.TakeDamage(finalDamage);
    }

    public void SkipTurn(CombatCharacter character)
    {
        BattleLogManager.Instance.ShowLog($"{character.characterData.characterName}은(는) 차례를 넘깁니다.");
        EndPlayerTurn();
    }

    IEnumerator EnemyTurn(CombatCharacter enemy)
    {
        if (isBattleEnded) yield break;

        yield return new WaitForSeconds(1f);

        // ✅ 기절 체크 추가
        if (enemy.HasStatus(StatusEffectType.Stun))
        {
            BattleLogManager.Instance.ShowLog($"{enemy.characterData.characterName}은(는) 기절하여 행동할 수 없습니다!");
            yield return new WaitForSeconds(1f);
            enemy.hasActed = true;
            NextTurn(); // 또는 턴 넘기기
            yield break;
        }

        // 행동 로직 (예: 가장 체력 낮은 적 공격)
        CombatCharacter target = GetRandomAlivePlayer();
        if (target != null)
        {
            int damage = Mathf.Max(1, enemy.ATK - target.DEF);
            target.TakeDamage(damage);
            BattleLogManager.Instance.ShowLog($"{enemy.characterData.characterName}의 공격! {target.characterData.characterName}에게 {damage} 데미지!");
        }

        yield return new WaitForSeconds(1f);
        enemy.hasActed = true;
        NextTurn();
    }

    // BattleManager.cs 내부
    private CombatCharacter GetRandomAlivePlayer()
    {
        List<CombatCharacter> alivePlayers = playerTeam.FindAll(p => p.isAlive);
        if (alivePlayers.Count == 0) return null;

        int index = Random.Range(0, alivePlayers.Count);
        return alivePlayers[index];
    }

    // NextTurn은 '새 코루틴 시작'을 아주 조심해야 함
    public void NextTurn()
    {
        if (isBattleEnded) return;         // ✅ 종료 후 재시작 방지
        if (isTurnRunning) return;         // ✅ 이미 돌고 있으면 중복 시작 방지
        StartCoroutine(HandleTurns());
    }

    public void TryEscape(CombatCharacter character)
    {
        // 도망 확률 계산 (예: 50% 확률)
        float escapeChance = 0.5f;

        if (Random.value < escapeChance)
        {
            BattleLogManager.Instance.ShowLog($"{character.characterData.characterName}이(가) 도망에 성공했습니다!");
            EndBattle();  // 전투 종료
        }
        else
        {
            BattleLogManager.Instance.ShowLog($"{character.characterData.characterName}이(가) 도망치려 했지만 실패했습니다!");
            character.hasActed = true;
            StartCoroutine(NextTurnAfterDelay());
        }
    }

    private IEnumerator NextTurnAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        NextTurn();
    }

    public List<CombatCharacter> GetAliveAllies(bool isPlayer)
    {
        if (isPlayer)
            return playerTeam.Where(c => c != null && c.isAlive).ToList();
        else
            return enemyTeam.Where(c => c != null && c.isAlive).ToList();
    }

    // 🔹 사망 콜백: CombatCharacter.TakeDamage()에서 호출
    public void OnCharacterDied(CombatCharacter dead)
    {
        if (dead == null || isBattleEnded) return;

        // 1) 먼저 전투 로직에서 제거
        RemoveFromTurnQueue(dead);
        playerTeam.Remove(dead);
        enemyTeam.Remove(dead);

        // 2) 현재 턴이면 안전하게 넘기기
        if (currentCharacter == dead)
        {
            currentCharacter.hasActed = true;
            NextTurnSafe();
        }

        // 3) 승패 판정 (UI 파괴 전에 끝내기)
        if (IsTeamAllDead(enemyTeam))
        {
            EndBattle();   // isBattleEnded = true
        }
        else if (IsTeamAllDead(playerTeam))
        {
            EndBattle();
        }

        // 4) UI/본체 파괴는 프레임 끝으로 미룸
        if (dead.ui != null)
        {
            var uiGo = dead.ui.gameObject;
            dead.ui = null;
            StartCoroutine(DestroyEndOfFrame(uiGo));
        }

        StartCoroutine(DestroyEndOfFrame(dead.gameObject));

        // 5) 남은 UI는 안전하게 전체 갱신 (가드 포함되어 있어야 함)
        if (!isBattleEnded)
            BattleUIManager.Instance?.UpdateAllCharacterUI();
    }

    private IEnumerator DestroyEndOfFrame(GameObject go)
    {
        yield return null; // 레이아웃/IMGUI 처리 끝난 뒤
        if (go != null) Destroy(go);
    }

    // 🔸 Queue에는 Remove가 없어서 필터링으로 재구성
    private void RemoveFromTurnQueue(CombatCharacter c)
    {
        if (turnQueue == null || turnQueue.Count == 0) return;
        var left = turnQueue.Where(x => x != null && x != c && x.IsAlive());
        turnQueue = new Queue<CombatCharacter>(left);
    }

    // 🔸 현재 코루틴/턴 흐름과 충돌 없게 다음 턴 보장
    private void NextTurnSafe()
    {
        // turnQueue가 비어있다면 재구성
        if (turnQueue.Count == 0)
            InitTurnQueue();

        // HandleTurns는 isTurnRunning 가드가 있으므로 재호출 안전
        StartCoroutine(HandleTurns());
    }

    // 🔸 팀 전멸 체크 유틸
    private bool IsTeamAllDead(List<CombatCharacter> team)
    {
        return team == null || team.Count == 0 || team.TrueForAll(t => t == null || !t.IsAlive());
    }

    // ✅ 컴파일 오류 없이 동작하는 최소 OnResultConfirmed
    // 결과창 OK 버튼 콜백
    private void OnResultConfirmed()
    {
        if (lastResult == null)
        {
            Debug.LogWarning("lastResult is null");
            return;
        }

        // 로그
        Debug.Log($"[Result] Victory={lastResult.isVictory}, " +
                  $"stones={lastResult.stoneReward}, " +
                  $"drops={lastResult.drops?.Count ?? 0}, " +
                  $"partyResults={lastResult.partyResults?.Count ?? 0}");

        // ✅ 보상 지급
        if (lastResult.stoneReward > 0)
            InventoryManager.Instance?.AddStones(lastResult.stoneReward);

        if (lastResult.drops != null)
        {
            foreach (var dr in lastResult.drops)
            {
                if (dr?.item == null) continue;
                InventoryManager.Instance?.AddItem(dr.item, dr.count);
            }
        }

        // ✅ 파티 상태 반영 (이름 또는 ID로 매칭)
        if (lastResult.partyResults != null)
        {
            foreach (var pr in lastResult.partyResults)
            {
                var member = (pr.id != null)
                    ? PartyManager.Instance?.FindCharacterById(pr.id)
                    : PartyManager.Instance?.FindCharacterByName(pr.name);

                if (member != null)
                {
                    member.CurrentHP = pr.hp;
                    member.Stress += pr.stressDelta;
                    if (pr.isDead)
                    {
                        PartyManager.Instance?.RemoveCharacter(member);
                    }
                }
            }
        }

        // ✅ Adventure 흐름으로 되돌리기 (있으면 우선)
        if (AdventureManager.Instance != null)
        {
            AdventureManager.Instance.OnBattleFinished(lastResult);
            return;
        }

        // 🔁 혹은 씬 전환 fallback
        if (!string.IsNullOrEmpty(nextSceneName))
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogWarning("nextSceneName이 비어있습니다. 씬 전환이 설정되지 않았습니다.");
    }

    private bool TryCleanse(CombatCharacter target, StatusEffectType type)
    {
        if (target == null || !target.isAlive) return false;
        // CombatCharacter.RemoveStatusEffect(StatusEffectType) 이 bool을 반환(권장 방식)하도록 이미 수정했음
        return target.TryRemoveStatus(type);
    }

    private int TryCleanseMany(IEnumerable<CombatCharacter> targets, StatusEffectType type)
    {
        int count = 0;
        if (targets == null) return 0;

        foreach (var t in targets)
        {
            if (TryCleanse(t, type))
                count++;
        }
        return count;
    }
}
