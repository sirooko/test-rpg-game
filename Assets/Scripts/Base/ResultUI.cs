using UnityEngine;
using UnityEngine.UI;
using Game;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private Button okButton;
    void Awake()
    {
        okButton.onClick.AddListener(() => SceneFlow.Instance.LoadBase());
    }
}
