using UnityEngine;
using UnityEngine.UI;

public class DamagePopupManager : MonoBehaviour
{
    public GameObject damageTextPrefab;   // Text 프리팹
    public Transform worldCanvas;         // 월드 공간에 띄울 캔버스

    public static DamagePopupManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDamage(Vector3 worldPos, int damageAmount)
    {
        GameObject textObj = Instantiate(damageTextPrefab, worldCanvas);
        textObj.transform.position = worldPos;

        Text damageText = textObj.GetComponentInChildren<Text>();
        if (damageText != null)
            damageText.text = damageAmount.ToString();

        Destroy(textObj, 1.0f); // 1초 후 자동 삭제
    }
}
