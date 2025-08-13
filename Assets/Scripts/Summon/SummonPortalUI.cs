using UnityEngine;
using UnityEngine.UI;

public class SummonPortalUI : MonoBehaviour
{
    public void OnBasicSummon()
    {
        Debug.Log("기본 영웅 소환 시도!");
        // 소환 연출 및 결과 처리 추가 예정
    }

    public void OnAdvancedSummon()
    {
        Debug.Log("상급 영웅 소환 시도!");
        // 고급 소환 연출 및 결과 처리 추가 예정
    }

    public void OnUpgradePortal()
    {
        Debug.Log("포탈 업그레이드 시도!");
        // 업그레이드 확인 및 비용 차감
    }

    public void OnExit()
    {
        gameObject.SetActive(false);
    }
}
