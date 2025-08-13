using UnityEngine;

namespace Game
{
    public class BootLoader : MonoBehaviour
    {
        // ���� �� ���� �̱��游 ���� ����
        private void Start()
        {
            if (SceneFlow.Instance == null)
                new GameObject("SceneFlow").AddComponent<SceneFlow>();
            if (GameContext.Instance == null)
                new GameObject("GameContext").AddComponent<GameContext>();

            // �ʿ��ϸ� ���⼭ InventoryManager �� ���� �Ŵ����� ���� ����

            SceneFlow.Instance.LoadTitle();
        }
    }
}
