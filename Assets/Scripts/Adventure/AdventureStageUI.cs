using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureStageUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Text stageInfoText;
    public Text eventText;
    public Button choice1Button;
    public Button choice2Button;
    public Text choice1Text;
    public Text choice2Text;
    public Image backgroundImage;

    public AdventureCharacterUI[] adventureTeamSlots;

    private int currentStage = 1;
    private int maxStage;

    public void SetMaxStage(int value)
    {
        maxStage = value;
    }

    public void SetBackground(Sprite bg)
    {
        backgroundImage.sprite = bg;
    }

    public void SetupStage(int stageNumber, string eventDescription, string option1, string option2)
    {
        currentStage = stageNumber;
        stageInfoText.text = $"스테이지 {currentStage}/{maxStage}";
        eventText.text = eventDescription;

        choice1Text.text = option1;
        choice2Text.text = option2;

        choice1Button.onClick.RemoveAllListeners();
        choice2Button.onClick.RemoveAllListeners();
    }

    public void SetTeamUI(List<CharacterInAdventure> team)
    {
        for (int i = 0; i < adventureTeamSlots.Length; i++)
        {
            if (i < team.Count)
            {
                adventureTeamSlots[i].gameObject.SetActive(true);
                adventureTeamSlots[i].UpdateUI(team[i]);
            }
            else
            {
                adventureTeamSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
