using UnityEngine;
using System;

[DisallowMultipleComponent]
// [Obsolete("BaseScreenRouter를 직접 사용하세요. 남아있는 레거시 호출을 위해 임시 유지 중.")]
public class SelectionUIManager : MonoBehaviour
{
    [SerializeField] private BaseScreenRouter router;

    // ✅ 더 이상 직접 패널 참조 안 씀 (필요 없으니 필드 삭제 권장)
    // public GameObject castleDoorPanel; ... 전부 제거해도 됩니다.

    void Reset()
    {
        router = FindAnyObjectByType<BaseScreenRouter>();
    }

    /// <summary>
    /// 레거시 API 호환: 문자열 키로 패널 열기
    /// 내부적으로 BaseScreenRouter.Show(...) 호출
    /// </summary>
    public void OpenPanel(string panelName)
    {
        if (router == null) router = FindAnyObjectByType<BaseScreenRouter>();
        if (router == null)
        {
            Debug.LogWarning("[SelectionUIManager] BaseScreenRouter not found.");
            return;
        }

        var section = Map(panelName);
        router.Show(section);
    }

    /// <summary>
    /// 레거시 API 호환: 모든 패널 닫기 → 선택지 화면으로
    /// </summary>
    public void CloseAllPanels()
    {
        if (router == null) router = FindAnyObjectByType<BaseScreenRouter>();
        if (router != null) router.Show(BaseSection.None);
    }

    private static BaseSection Map(string key)
    {
        // 대소문자 차이, 하이픈/스페이스 잡아주기
        key = (key ?? string.Empty).Trim().Replace(" ", "").Replace("-", "").ToLowerInvariant();

        switch (key)
        {
            case "castledoor": return BaseSection.CastleDoor;
            case "summonportal": return BaseSection.SummonPortal;
            case "goddessstatue": return BaseSection.GoddessStatue;
            case "demonicmerchant": return BaseSection.DemonicMerchant;
            case "storage": return BaseSection.Storage;
            case "castle": return BaseSection.Castle;
            case "trainingroom": return BaseSection.TrainingRoom;
            case "restroom": return BaseSection.RestRoom;
            case "pub": return BaseSection.Pub;
            case "secretgarden": return BaseSection.SecretGarden;
            default:
                Debug.LogWarning($"[SelectionUIManager] Unknown panel key: {key}");
                return BaseSection.None;
        }
    }
}