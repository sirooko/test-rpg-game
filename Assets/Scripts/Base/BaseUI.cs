using UnityEngine;
using UnityEngine.UI;
using Game;

public class BaseUI : MonoBehaviour
{
    [SerializeField] private Button toAdventureButton;

    void Awake()
    {
        toAdventureButton.onClick.AddListener(() => SceneFlow.Instance.LoadAdventure());
    }
}
