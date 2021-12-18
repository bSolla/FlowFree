using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeObject : MonoBehaviour
{
    public Text _themeName;
    public Image[] _dotImages;
    public Button _buttonComponent;

    private int _themeIndex;
    private string _shortName;
    private GameObject _settingsPanelReference;
    private ThemeSelection _themesPanelReference;
    private GameObject _optionsPanelReference;
    private LevelManager _levelManager;

    public void Init(Colorway theme, int idx, GameObject setPanel, ThemeSelection themPanel, GameObject optPanel, LevelManager lm)
    {
        _themeName.text = theme._name;
        _themeName.color = theme._nameColor;
        
        _themeIndex = idx;

        _shortName = theme._shortName;

        _settingsPanelReference = setPanel;
        _themesPanelReference = themPanel;
        _optionsPanelReference = optPanel;

        _levelManager = lm;

        for (int i = 0; i < _dotImages.Length; ++i)
        {
            _dotImages[i].color = theme._arrayColors[i];
        }
    }

    public void SetTheme()
    {
        GameManager.GetInstance().SetThemeIndex(_themeIndex);
    }

    public void ThemeChange()
    {
        _settingsPanelReference.SetActive(false);
        _themesPanelReference.gameObject.SetActive(false);
        if (_optionsPanelReference != null)
            _optionsPanelReference.SetActive(false);
        if (_levelManager != null)
            _levelManager.SetPause(false);

        _themesPanelReference.ChangeTextColors(GameManager.GetInstance().GetThemeByNumber(_themeIndex));
    }
}
