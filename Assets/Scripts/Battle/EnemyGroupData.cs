// Scripts/Battle/EnemyGroupData.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Enemy Group")]
public class EnemyGroupData : ScriptableObject
{
    public List<CharacterData2> enemies;   // �� ���ø�(������ CharacterData2 ����)
    // �ʿ� �� AI/��ġ/���� ������ �ʵ� �߰�
}
