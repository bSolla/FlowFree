using UnityEngine;

/// <summary>
/// 
/// Scriptable object that stores the data of a 
/// package. 
/// 
/// </summary>
[CreateAssetMenu(fileName = "LevelData", menuName = "LevelPackage")]
public class LevelPackage : ScriptableObject
{
    // whatever is in the level
    public string _name;
    public TextAsset[] _levels;
} // LevelPackage