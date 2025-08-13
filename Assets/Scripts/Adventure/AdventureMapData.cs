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
    // 나중에 스테이지 목록 등 추가 가능

  
    public List<StageEventData> stages;

}
