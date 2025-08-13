using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatCharacterUI : MonoBehaviour
{
    [Header("Refs (Slot = 이 스크립트가 붙은 오브젝트)")]
    public Image characterImage;
    public Text nameText;
    public Slider hpSlider;
    public Slider mpSlider;
    public Button targetButton;

    [Header("Shake 대상(컨텐츠) - 반드시 Slot의 자식 RectTransform 할당")]
    [SerializeField] private RectTransform content;   // ✅ 여기만 흔듦
    [SerializeField] private CanvasGroup canvasGroup; // 선택: 죽음 표시용

    private CombatCharacter linkedCharacter;

    // 원상 복구용
    private Vector2 baseAnchoredPos;    // content 기준
    private Color originalColor;
    private Coroutine hitEffectRoutine;

    private void Awake()
    {
        // content 미지정 시, 자식에서 자동 탐색 (실수 방지)
        if (content == null)
        {
            var t = transform.childCount > 0 ? transform.GetChild(0) as RectTransform : null;
            content = t ?? GetComponent<RectTransform>(); // 마지막 안전장치
        }

        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (characterImage != null) originalColor = characterImage.color;
    }

    /// <summary>
    /// 전투 캐릭터와 UI를 연결
    /// </summary>
    public void Setup(CombatCharacter character, Sprite sprite, bool isEnemy)
    {
        linkedCharacter = character;

        if (nameText != null) nameText.text = character.characterData.characterName;
        if (characterImage != null)
        {
            characterImage.sprite = sprite;
            characterImage.rectTransform.localScale = isEnemy ? new Vector3(-1, 1, 1) : Vector3.one;
        }

        if (hpSlider != null)
        {
            hpSlider.maxValue = character.MaxHP;
            hpSlider.value = character.currentHP;
        }
        if (mpSlider != null)
        {
            mpSlider.maxValue = character.MaxMP;
            mpSlider.value = character.currentMP;
        }

        // 1프레임 뒤에 레이아웃이 자리잡은 후 기준 위치 저장
        StartCoroutine(CaptureBaseAfterLayout());
    }

    private IEnumerator CaptureBaseAfterLayout()
    {
        yield return null; // 레이아웃 적용 후
        if (content != null) baseAnchoredPos = content.anchoredPosition;
        if (characterImage != null) originalColor = characterImage.color;
    }

    /// <summary>
    /// HP/MP 슬라이더 갱신
    /// </summary>
    public void UpdateStats()
    {
        if (linkedCharacter == null) return;

        if (hpSlider != null)
        {
            // MaxHP가 변할 수 있다면 항상 동기화
            if (hpSlider.maxValue != linkedCharacter.MaxHP)
                hpSlider.maxValue = linkedCharacter.MaxHP;
            hpSlider.value = Mathf.Clamp(linkedCharacter.currentHP, 0, linkedCharacter.MaxHP);
        }

        if (mpSlider != null)
        {
            if (mpSlider.maxValue != linkedCharacter.MaxMP)
                mpSlider.maxValue = linkedCharacter.MaxMP;
            mpSlider.value = Mathf.Clamp(linkedCharacter.currentMP, 0, linkedCharacter.MaxMP);
        }
    }

    public void SetTargetButton(bool active, UnityEngine.Events.UnityAction onClickAction)
    {
        if (targetButton == null) return;

        targetButton.onClick.RemoveAllListeners();
        targetButton.gameObject.SetActive(active);
        if (active && onClickAction != null)
            targetButton.onClick.AddListener(onClickAction);
    }

    /// <summary>
    /// 안전한 피격 연출 (Slot은 고정, Content만 흔들기)
    /// </summary>
    public void PlayHitEffect(int damage = 0)
    {
        if (hitEffectRoutine != null)
        {
            StopCoroutine(hitEffectRoutine);
            hitEffectRoutine = null;
            // 중복 호출 시 누적 이동 방지 위해 즉시 원복
            if (content != null) content.anchoredPosition = baseAnchoredPos;
            if (characterImage != null) characterImage.color = originalColor;
        }

        hitEffectRoutine = StartCoroutine(HitEffectCoroutine());

        // 데미지 팝업
        if (damage > 0)
        {
            // 화면 좌표 보정이 필요하면 RectTransformUtility 사용 고려
            Vector3 popupPos = (content != null ? content.position : transform.position) + Vector3.up * 100f;
            DamagePopupManager.Instance?.ShowDamage(popupPos, damage);
        }
    }

    private IEnumerator HitEffectCoroutine()
    {
        if (content == null)
            yield break;

        // 색상 플래시
        if (characterImage != null)
            characterImage.color = Color.red;

        // 흔들기
        float duration = 0.2f;
        float t = 0f;
        float amplitude = 8f;

        // 기준은 "현재" baseAnchoredPos (사망 등으로 레이아웃 변해도 안전)
        Vector2 basePos = baseAnchoredPos;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // UI는 unscaled로
            float off = Mathf.Sin(t * 40f) * amplitude;
            content.anchoredPosition = basePos + new Vector2(off, 0f);
            yield return null;
        }

        // 원복
        content.anchoredPosition = basePos;
        if (characterImage != null)
            characterImage.color = originalColor;

        hitEffectRoutine = null;
    }

    /// <summary>
    /// 죽음 표시(삭제 대신 슬롯 유지하고 비활성/회색 처리)
    /// </summary>
    public void SetDead(bool dead)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = dead ? 0.5f : 1f;
            canvasGroup.interactable = !dead;
            canvasGroup.blocksRaycasts = !dead;
        }

        // HP/MP 0 표시
        if (dead)
        {
            if (hpSlider != null) hpSlider.value = 0;
            if (mpSlider != null) mpSlider.value = Mathf.Min(mpSlider.value, 0);
        }

        // 타겟 버튼 비활성
        SetTargetButton(false, null);

        // 흔들기 중이었으면 원복
        if (hitEffectRoutine != null)
        {
            StopCoroutine(hitEffectRoutine);
            hitEffectRoutine = null;
        }
        if (content != null) content.anchoredPosition = baseAnchoredPos;
        if (characterImage != null) characterImage.color = originalColor;
    }
}
