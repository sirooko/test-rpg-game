// Scripts/Adventure/StageData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    public string stageId;
    public Sprite background;                     // (����) ���
    public List<CharacterData2> enemyTemplates;   // �⺻ �� ����
    // �ʿ� ��: ���̺�/��ġ/AI �� Ȯ�� �ʵ� �߰�
}
