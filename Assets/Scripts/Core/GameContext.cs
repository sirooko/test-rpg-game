using UnityEngine;

namespace Game
{
    public class GameContext : MonoBehaviour
    {
        public static GameContext Instance { get; private set; }

        // �� �� �����ϰ� ���� �ּ� ������ (���� Ȯ��)
        public StageData SelectedStage;   // Adventure��Battle ����

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
