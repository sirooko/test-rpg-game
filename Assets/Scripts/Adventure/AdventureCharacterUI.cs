using UnityEngine;
using UnityEngine.UI;

public class AdventureCharacterUI : MonoBehaviour
{
    public Image characterImage;
    public Text nameText;
    public Slider hpBar;
    public Slider mpBar;          // 현재 Adventure 데이터에 MP가 없다면 자동 비활성화
    public Text stressText;
    public Image stressIcon;

    public void UpdateUI(CharacterInAdventure data)
    {
        if (data == null)
        {
            Debug.LogWarning("[AdventureCharacterUI] data is null");
            return;
        }

        // 현재 구조: CharacterInAdventure.originalData 사용
        CharacterData2 baseData = data.originalData;

        // 이름
        if (nameText != null)
            nameText.text = baseData != null ? baseData.characterName : (data.Name ?? "Unknown");

        // 이미지 (battleSprite 우선, 없으면 characterSprite 시도)
        if (characterImage != null)
        {
            Sprite sp = null;
            if (baseData != null)
            {
                // 프로젝트에 따라 둘 중 하나만 있을 수 있음
                sp = baseData.battleSprite != null ? baseData.battleSprite : baseData.characterSprite;
            }
            characterImage.sprite = sp;
        }

        // MP 바는 Adventure 데이터에 없으므로 기본적으로 숨김
        if (mpBar != null)
            mpBar.gameObject.SetActive(false);

        if (data.isDead)
        {
            if (hpBar != null)
            {
                hpBar.maxValue = baseData != null ? baseData.maxHP : 1;
                hpBar.value = 0;
            }

            if (stressText != null)
                stressText.text = "사망";

            if (characterImage != null)
                characterImage.color = Color.gray;

            if (stressIcon != null)
                stressIcon.enabled = false;
        }
        else
        {
            // HP
            if (hpBar != null)
            {
                int maxHp = baseData != null ? baseData.maxHP : Mathf.Max(1, data.currentHP);
                hpBar.maxValue = maxHp;
                hpBar.value = Mathf.Clamp(data.currentHP, 0, maxHp);
            }

            // 스트레스
            if (stressText != null)
                stressText.text = data.currentStress.ToString();

            if (characterImage != null)
                characterImage.color = Color.white;

            if (stressIcon != null)
            {
                stressIcon.enabled = true;
                stressIcon.color = (data.currentStress >= 50) ? Color.red : Color.white;
            }
        }
    }
}