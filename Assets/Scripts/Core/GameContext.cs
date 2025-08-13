using UnityEngine;

namespace Game
{
    public class GameContext : MonoBehaviour
    {
        public static GameContext Instance { get; private set; }

        // 씬 간 전달하고 싶은 최소 데이터 (점차 확장)
        public StageData SelectedStage;   // Adventure→Battle 전달

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
