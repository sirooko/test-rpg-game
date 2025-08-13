using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    public string stageId;
    public string displayName;
    public Sprite preview;
    // ����: �� ���̺�, ��� ���̺� �� Ȯ��
}
