using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "NewAdventureMap", menuName = "ScriptableObjects/AdventureMap")]
public class AdventureMapData : ScriptableObject
{
    public string mapName;
    [TextArea]
    public string description;
    public Sprite thumbnail;
    public Sprite backgroundImage;
    public int difficulty;
    public int maxStage;
    // ���߿� �������� ��� �� �߰� ����

  
    public List<StageEventData> stages;

}
