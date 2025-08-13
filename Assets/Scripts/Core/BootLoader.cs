using UnityEngine;

namespace Game
{
    public class BootLoader : MonoBehaviour
    {
        // 부팅 시 전역 싱글톤만 보장 생성
        private void Start()
        {
            if (SceneFlow.Instance == null)
                new GameObject("SceneFlow").AddComponent<SceneFlow>();
            if (GameContext.Instance == null)
                new GameObject("GameContext").AddComponent<GameContext>();

            // 필요하면 여기서 InventoryManager 등 전역 매니저도 생성 가능

            SceneFlow.Instance.LoadTitle();
        }
    }
}
