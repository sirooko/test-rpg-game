using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static BattleManager;
using static UnityEditor.Progress;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;

    public CombatCharacter currentCharacter;

    [Header("UI 버튼")]
    public Button[] actionButtons;               // 총 6개의 버튼 (고정)
    public Text[] actionButtonTexts;             // 각 버튼의 텍스트 컴포넌트

    public GameObject characterUIPrefab;
    public Transform playerPanel;
    public Transform enemyPanel;

    private CombatCharacter selectedTarget;

    public List<CombatCharacterUI> characterUIList = new List<CombatCharacterUI>();

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ClearAllCharacterUI()
    {
        if (characterUIList == null) return;
        for (int i = characterUIList.Count - 1; i >= 0; i--)
        {
            if (characterUIList[i] != null)
                Destroy(characterUIList[i].gameObject);
            characterUIList.RemoveAt(i);
        }
    }

    // 버튼 1개 설정
    public void SetButton(int index, string text, UnityAction action, bool interactable = true)
    {
        if (index < 0 || index >= actionButtons.Length)
            return;

        actionButtons[index].interactable = interactable;
        actionButtonTexts[index].text = text;
        actionButtons[index].onClick.RemoveAllListeners();

        if (action != null)
            actionButtons[index].onClick.AddListener(action);
    }

    // 모든 버튼 비활성화
    public void HideAllButtons()
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].gameObject.SetActive(false);
        }
    }

    // 모든 버튼 활성화
    public void ShowAllButtons()
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].gameObject.SetActive(true);
        }
    }

    // 기본 행동 선택 (공격, 방어, 스킬, 아이템, 차례 넘기기, 도망가기)
    public void ShowMainOptions(CombatCharacter character)
    {
        currentCharacter = character;
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].gameObject.SetActive(true);
        }

        SetButton(0, "공격", () => BattleManager.Instance.ShowTargetSelection(ActionType.Attack));
        SetButton(1, "방어", () => { BattleManager.Instance.Defend(character); HideAllButtons(); });

        // 🔹 Silence 상태면 스킬 버튼 비활성화
        if (character.HasStatus(StatusEffectType.Silence))
        {
            SetButton(2, "스킬 (침묵)", () =>
            {
                BattleLogManager.Instance.ShowLog($"{character.characterData.characterName}은(는) 침묵 상태라 스킬을 사용할 수 없습니다!");
            }, false);
        }
        else
        {
            SetButton(2, "스킬", () => ShowSkillList(character));
        }

        SetButton(3, "아이템", () => ShowItemList(character));
        SetButton(4, "차례 넘기기", () => { BattleManager.Instance.SkipTurn(character); HideAllButtons(); });
        SetButton(5, "도망가기", () => { BattleManager.Instance.TryEscape(character); HideAllButtons(); });
    }



    // 스킬 목록 보여주기
    public void ShowSkillList(CombatCharacter character)
    {
        HideAllButtons();

        List<SkillData> skills = character.skills;

        for (int i = 0; i < 5; i++)
        {
            if (i < skills.Count)
            {
                SkillData skill = skills[i];
                SetButton(i, skill.skillName, () =>
                {
                    BattleManager.Instance.ShowTargetSelection(
                        BattleManager.ActionType.Skill,
                        selectedSkill: skill
                    );
                });
            }
            else
            {
                SetButton(i, "", null, false); // 비활성화
            }

            actionButtons[i].gameObject.SetActive(true);
        }

        // 6번 버튼은 뒤로가기
        SetButton(5, "뒤로가기", () =>
        {
            ShowMainOptions(character);
        });
        actionButtons[5].gameObject.SetActive(true);
    }



    // 아이템 목록 보여주기
    public void ShowItemList(CombatCharacter user)
    {
        HideAllButtons();

        var usableItems = InventoryManager.Instance?.GetUsableItems() ?? new List<ItemData>();
        int maxActionSlots = actionButtons.Length - 1; // 마지막은 '뒤로가기' 예약

        for (int i = 0; i < maxActionSlots; i++)
        {
            if (i < usableItems.Count)
            {
                var item = usableItems[i];

                SetButton(i, item.itemName, () =>
                {
                    // 🔹 아이템의 타겟 진영 결정 (회복/해제 = 아군, 그 외 = 적)
                    bool targetAllies = (item.type == ItemType.HealHP || item.type == ItemType.HealMP || item.type == ItemType.HealStatus);

                    if (item.targetAll)
                    {
                        // 🔸 광역: 한 번에 처리(소비도 1회)
                        var targets = targetAllies
                            ? BattleManager.Instance.GetAliveAllies(user.isPlayer)       // 아군
                            : BattleManager.Instance.GetAliveAllies(!user.isPlayer);     // 적군

                        BattleManager.Instance.UseItemAoE(user, item, targets);
                        // EndPlayerTurn은 AoE 내부에서 처리
                    }
                    else
                    {
                        // 🔸 단일: 대상 선택 UI → 선택 후 1회 처리
                        ShowTargetSelection(user, item, (target) =>
                        {
                            BattleManager.Instance.UseItem(user, target, item);
                            // EndPlayerTurn은 UseItem 내부에서 처리
                        });
                    }
                });

                actionButtons[i].gameObject.SetActive(true);
            }
            else
            {
                SetButton(i, "", null, false);
                actionButtons[i].gameObject.SetActive(false);
            }
        }

        // 뒤로가기 버튼
        SetButton(maxActionSlots, "뒤로가기", () =>
        {
            ShowMainOptions(user);
        });
        actionButtons[maxActionSlots].gameObject.SetActive(true);
    }

    public void HideItemUI()
    {
        HideAllButtons();
        ShowMainOptions(BattleManager.Instance.currentCharacter);
    }

    public void ShowTargetSelection(CombatCharacter user, ItemData item, System.Action<CombatCharacter> onTargetSelected)
    {
        HideAllButtons();

        List<CombatCharacter> targets;

        if (item.type == ItemType.HealHP || item.type == ItemType.HealMP || item.type == ItemType.HealStatus)
        {
            // 회복계 아이템은 아군
            targets = BattleManager.Instance.GetValidTargets(isPlayer: !user.isEnemy);
        }
        else
        {
            // 추후 공격 아이템 등을 위해 남겨둠 (현재는 없음)
            targets = BattleManager.Instance.GetValidTargets(isPlayer: user.isEnemy);
        }

        for (int i = 0; i < actionButtons.Length - 1; i++)
        {
            if (i < targets.Count)
            {
                var target = targets[i];
                SetButton(i, target.characterData.characterName, () =>
                {
                    onTargetSelected.Invoke(target);
                }, target.isAlive);
            }
            else
            {
                SetButton(i, "", null, false);
            }

            actionButtons[i].gameObject.SetActive(true);
        }

        // 뒤로가기 버튼
        SetButton(actionButtons.Length - 1, "뒤로가기", () =>
        {
            ShowItemList(user);
        });
        actionButtons[^1].gameObject.SetActive(true);
    }


    // 대상 선택 UI 보여주기
    public void ShowTargetSelection(List<CombatCharacter> targets, Action<CombatCharacter> onTargetSelected)
    {
        //Debug.Log("타겟 선택 호출됨 - 아이템: " + item.itemName);
        HideAllButtons();

        for (int i = 0; i < actionButtons.Length - 1; i++)  // 마지막은 뒤로가기 버튼
        {
            if (i < targets.Count)
            {
                int index = i;
                actionButtons[i].gameObject.SetActive(true);
                actionButtons[i].GetComponentInChildren<Text>().text = targets[i].characterData.characterName;

                actionButtons[i].onClick.RemoveAllListeners();
                actionButtons[i].onClick.AddListener(() => onTargetSelected(targets[index]));
            }
            else
            {
                actionButtons[i].gameObject.SetActive(false);
            }
        }

        ShowBackButton(() => ShowMainOptions(BattleManager.Instance.currentCharacter));
    }

    public void ShowTargetSelection(CombatCharacter user, SkillData skill, System.Action<CombatCharacter> onTargetSelected)
    {
        HideAllButtons();

        List<CombatCharacter> targets = new();

        switch (skill.targetType)
        {
            case SkillTargetType.Enemy:
            case SkillTargetType.AllEnemies:
                targets = BattleManager.Instance.GetValidTargets(isPlayer: user.isEnemy);  // 적
                break;

            case SkillTargetType.Ally:
            case SkillTargetType.AllAllies:
                targets = BattleManager.Instance.GetValidTargets(isPlayer: !user.isEnemy); // 아군
                break;

            case SkillTargetType.Self:
                // 자신에게만 사용 → 버튼 없이 자동 실행
                onTargetSelected.Invoke(user);
                return;
        }

        for (int i = 0; i < actionButtons.Length - 1; i++)
        {
            if (i < targets.Count)
            {
                var target = targets[i];
                SetButton(i, target.characterData.characterName, () =>
                {
                    onTargetSelected.Invoke(target);
                }, target.isAlive);
            }
            else
            {
                SetButton(i, "", null, false);
            }

            actionButtons[i].gameObject.SetActive(true);
        }

        // 뒤로가기 버튼
        SetButton(actionButtons.Length - 1, "뒤로가기", () =>
        {
            ShowSkillList(user);  // 스킬 목록으로 복귀
        });
        actionButtons[^1].gameObject.SetActive(true);
    }

    // 돌아가기 버튼만 남기고 나머지 비활성화
    public void ShowBackButton(UnityEngine.Events.UnityAction onClickAction)
    {
        int backIndex = actionButtons.Length - 1; // 보통 6번 (index 5)
        actionButtons[backIndex].gameObject.SetActive(true);
        actionButtons[backIndex].GetComponentInChildren<Text>().text = "뒤로가기";
        actionButtons[backIndex].onClick.RemoveAllListeners();
        actionButtons[backIndex].onClick.AddListener(onClickAction);
    }


    public void OnAttackConfirmed()
    {
        if (selectedTarget == null || BattleManager.Instance.currentCharacter == null)
            return;

        BattleManager.Instance.ExecuteAttack(BattleManager.Instance.currentCharacter, selectedTarget);
        BattleManager.Instance.currentCharacter.hasActed = true;

        HideAllButtons();
        selectedTarget = null;

        // 🔽 다음 캐릭터의 턴으로 넘어가기
        BattleManager.Instance.NextTurn(); // 또는 NextTurn(), ProceedTurn(), AdvanceTurn() 등 실제 함수명에 맞게
    }


    public void OnTargetButtonClicked(CombatCharacter target)
    {
        selectedTarget = target;

        // 선택 확인 로그
        Debug.Log($"🎯 대상 선택됨: {target.characterData.characterName}");

        // 곧바로 공격 실행 (또는 별도 '공격 확인' 버튼에서 호출 가능)
        OnAttackConfirmed();
    }

    public void DisableAllActionButtons()
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].interactable = false;
            actionButtons[i].onClick.RemoveAllListeners();
            actionButtons[i].GetComponentInChildren<Text>().text = "";
        }
    }

    public void UpdateAllCharacterUI()
    {
        if (characterUIList == null) return;
        for (int i = characterUIList.Count - 1; i >= 0; i--)
        {
            var ui = characterUIList[i];
            if (ui == null)
            {
                characterUIList.RemoveAt(i);
                continue;
            }
            ui.UpdateStats();
        }
    }
}
