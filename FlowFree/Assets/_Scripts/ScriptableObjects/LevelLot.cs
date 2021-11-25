using UnityEngine;

/// <summary>
/// 
/// Scriptable object that stores the data of a 
/// package. 
/// 
/// </summary>
[CreateAssetMenu(fileName = "LevelData", menuName = "LevelLot")]
public class LevelLot : ScriptableObject
{
    // whatever is in the level
    public string _name;
    public TextAsset _level;
} // LevelPackage