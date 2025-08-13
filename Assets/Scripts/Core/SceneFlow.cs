using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class SceneFlow : MonoBehaviour
    {
        public static SceneFlow Instance { get; private set; }

        [Header("Optional Fade")]
        [SerializeField] private CanvasGroup fadeCanvas; // �� ������
        [SerializeField] private float fadeTime = 0.25f;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Load(string sceneName) => StartCoroutine(LoadRoutine(sceneName));

        public void LoadTitle() => Load(SceneNames.Title);
        public void LoadBase() => Load(SceneNames.Base);
        public void LoadAdventure() => Load(SceneNames.Adventure);
        public void LoadBattle() => Load(SceneNames.Battle);
        public void LoadResult() => Load(SceneNames.Result);

        private IEnumerator LoadRoutine(string sceneName)
        {
            // ���̵� �ƿ� (������ ��ŵ)
            if (fadeCanvas != null) yield return Fade(1f);

            var op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone) yield return null;

            // ���̵� ��
            if (fadeCanvas != null) yield return Fade(0f);
        }

        private IEnumerator Fade(float target)
        {
            float t = 0f;
            float start = fadeCanvas.alpha;
            while (t < fadeTime)
            {
                t += Time.unscaledDeltaTime;
                fadeCanvas.alpha = Mathf.Lerp(start, target, t / fadeTime);
                yield return null;
            }
            fadeCanvas.alpha = target;
        }
    }
}
