using UnityEngine;
using UnityEngine.UI;

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

    public SelectionType type;
    public SelectionUIManager selectionUIManager;

    void Start()
    {
        Button btn = GetComponent<Button>();


        if (btn != null && selectionUIManager != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => selectionUIManager.OpenPanel(type.ToString()));
        }
    }
}
