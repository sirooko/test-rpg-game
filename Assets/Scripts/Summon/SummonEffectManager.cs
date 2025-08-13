using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SummonEffectManager : MonoBehaviour
{
    [Header("Refs")]
    public GameObject summonEffectUI;   // 전체 패널
    public GameObject summonUI;         // 기존 버튼들을 감싸는 부모 오브젝트
    public SummonManager summonManager; // 연결 필요

    public Image backgroundDim;
    public Image magicCircle;
    public Image lightBurst;
    public Image characterImage;
    public Text resultText;
    public Button closeButton;

    void Start()
    {
        if (closeButton != null) closeButton.onClick.AddListener(CloseEffect);
        if (summonEffectUI != null) summonEffectUI.SetActive(false);
    }

    public void StartSummonEffect()
    {
        if (summonUI != null) summonUI.SetActive(false);
        StartCoroutine(PlaySummonEffect());
    }

    IEnumerator PlaySummonEffect()
    {
        if (summonEffectUI != null) summonEffectUI.SetActive(true);

        // 초기화
        if (lightBurst) lightBurst.gameObject.SetActive(false);
        if (characterImage) characterImage.gameObject.SetActive(false);
        if (resultText) resultText.gameObject.SetActive(false);
        if (closeButton) closeButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        // 빛 번쩍
        if (lightBurst) lightBurst.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // (연출 여백)
        yield return new WaitForSeconds(0.5f);

        // ✅ 캐릭터 등장: 비용은 이미 컨트롤러에서 차감되었으므로 'NoCost' 호출
        ShowSummonResult();

        yield return new WaitForSeconds(1f);

        // 닫기 버튼 등장
        if (closeButton) closeButton.gameObject.SetActive(true);
    }

    void ShowSummonResult()
    {
        if (summonManager == null)
        {
            Debug.LogWarning("[SummonEffectManager] SummonManager not set.");
            return;
        }
        // ✅ 비용 없는 소환을 호출 (내부에서 SpendCurrency 절대 호출 X)
        summonManager.SummonCharacterNoCost();
    }

    void CloseEffect()
    {
        if (summonManager != null) summonManager.CloseResultUI();
        if (summonEffectUI != null) summonEffectUI.SetActive(false);
        if (summonUI != null) summonUI.SetActive(true);
    }
}