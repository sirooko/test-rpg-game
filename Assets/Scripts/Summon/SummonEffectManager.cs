using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SummonEffectManager : MonoBehaviour
{
    public GameObject summonEffectUI;              // 전체 패널
    public GameObject summonUI; // 기존 버튼들을 감싸는 부모 오브젝트
    public SummonManager summonManager; // 연결 필요



    public Image backgroundDim;
    public Image magicCircle;
    public Image lightBurst;
    public Image characterImage;
    public Text resultText;
    public Button closeButton;


    void Start()
    {
        closeButton.onClick.AddListener(CloseEffect);
        summonEffectUI.SetActive(false);
    }

    public void StartSummonEffect()
    {
        summonUI.SetActive(false);

        StartCoroutine(PlaySummonEffect());
    }

    IEnumerator PlaySummonEffect()
    {
        summonEffectUI.SetActive(true);

        // 초기화
        lightBurst.gameObject.SetActive(false);
        characterImage.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        // 빛 번쩍
        lightBurst.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);


        yield return new WaitForSeconds(0.5f);

        // 캐릭터 등장
        ShowSummonResrult();

        yield return new WaitForSeconds(1f);

        // 닫기 버튼 등장
        closeButton.gameObject.SetActive(true);
    }

    void CloseEffect()
    {
        summonManager.CloseResultUI();
        summonEffectUI.SetActive(false);
        summonUI.SetActive(true);
    }

    void ShowSummonResrult()
    {
        summonManager.SummonCharacter();
    }
}
