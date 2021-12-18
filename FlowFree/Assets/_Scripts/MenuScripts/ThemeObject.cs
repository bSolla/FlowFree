using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeObject : MonoBehaviour
{
    public Text _themeName;
    public Image[] _dotImages;

    private int _themeIndex;
    private string _shortName;

    public void Init(Colorway theme, int idx)
    {
        _themeName.text = theme._name;
        _themeName.color = theme._nameColor;
        _themeIndex = idx;

        for (int i = 0; i < _dotImages.Length; ++i)
        {
            _dotImages[i].color = theme._arrayColors[i];
        }
    }

    public void SetTheme()
    {
        GameManager.GetInstance().SetThemeIndex(_themeIndex);
    }
}
