using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SummonEffectManager : MonoBehaviour
{
    public GameObject summonEffectUI;              // ��ü �г�
    public GameObject summonUI; // ���� ��ư���� ���δ� �θ� ������Ʈ
    public SummonManager summonManager; // ���� �ʿ�



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

        // �ʱ�ȭ
        lightBurst.gameObject.SetActive(false);
        characterImage.gameObject.SetActive(false);
        resultText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        // �� ��½
        lightBurst.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);


        yield return new WaitForSeconds(0.5f);

        // ĳ���� ����
        ShowSummonResrult();

        yield return new WaitForSeconds(1f);

        // �ݱ� ��ư ����
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
