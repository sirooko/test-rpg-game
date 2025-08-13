using UnityEngine;
using UnityEngine.UI;
using Game;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button startButton;

    void Awake()
    {
        startButton.onClick.AddListener(() => SceneFlow.Instance.LoadBase());
    }
}
