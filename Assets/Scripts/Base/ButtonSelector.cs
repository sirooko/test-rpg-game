using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 선택지 버튼 → BaseScreenRouter.Show(...) 호출 전용
/// enum 순서와 무관하게 '명시 매핑'으로 안전하게 연결합니다.
/// </summary>
public class ButtonSelector : MonoBehaviour
{
    public enum SelectionType
    {
        castleDoor,
        summonPortal,
        goddessStatue,
        demonicMerchant,
        storage,
        castle,
        trainingRoom,
        restRoom,
        pub,
        secretGarden
    }

    [SerializeField] private SelectionType type;
    [SerializeField] private BaseScreenRouter router;   // 인스펙터에 라우터 할당

    void Reset()
    {
        // 씬에서 자동으로 라우터 찾아 연결 (수동 할당 없어도 안전)
        router = FindAnyObjectByType<BaseScreenRouter>();
    }

    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
        {
            // 다른 연출(OnClick)이 있으면 RemoveAllListeners()는 넣지 마세요.
            // btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }
    }

    public void OnClick()
    {
        if (router == null) router = FindAnyObjectByType<BaseScreenRouter>();
        if (router == null)
        {
            Debug.LogWarning("[ButtonSelector] BaseScreenRouter not found.");
            return;
        }
        router.Show(Map(type));
    }

    // 🔒 명시 매핑: enum 순서가 바뀌어도 안전
    private static BaseSection Map(SelectionType t)
    {
        switch (t)
        {
            case SelectionType.castleDoor: return BaseSection.CastleDoor;
            case SelectionType.summonPortal: return BaseSection.SummonPortal;
            case SelectionType.goddessStatue: return BaseSection.GoddessStatue;
            case SelectionType.demonicMerchant: return BaseSection.DemonicMerchant;
            case SelectionType.storage: return BaseSection.Storage;
            case SelectionType.castle: return BaseSection.Castle;
            case SelectionType.trainingRoom: return BaseSection.TrainingRoom;
            case SelectionType.restRoom: return BaseSection.RestRoom;
            case SelectionType.pub: return BaseSection.Pub;
            case SelectionType.secretGarden: return BaseSection.SecretGarden;
            default: return BaseSection.None;
        }
    }
}
