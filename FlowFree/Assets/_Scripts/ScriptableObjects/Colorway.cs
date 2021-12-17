using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorThemes", menuName = "Colorway")]
public class Colorway : ScriptableObject
{
    public string _name;
    public string _shortName;
    public Color[] _arrayColors;
}
