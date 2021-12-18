using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeSelection : MonoBehaviour
{
    [Header("Prefabs")]
    public ThemeObject _themeObjectPrefab = null;

    [Header("Game object references")]
    public VerticalLayoutGroup _themeArea = null;
    public GameObject _settingsPanel = null;
    public GameObject _optionsPanel = null;
    public LevelManager _levelManager = null;

    private bool _alreadyCreated = false;

    private void Start()
    {
        if (_themeObjectPrefab == null) Debug.LogError("Missing prefab in Theme Selection panel");
        if (_themeArea == null) Debug.LogError("Missing vertical scroll area refernece in theme selection panel");
        if (_settingsPanel == null) Debug.LogError("missing settings panel reference in theme selection panel");
        if (_optionsPanel == null) Debug.LogWarning("missing options panel reference in theme selection panel");
        if (_levelManager == null) Debug.LogWarning("missing level manager reference in theme selection panel");

        CreateThemeSelection();
    }

    public void CreateThemeSelection()
    {
        if (!_alreadyCreated)
        {
            ThemeObject theme;
            int nThemes = GameManager.GetInstance().GetNumberThemes();

            for (int i = 0; i < nThemes; ++i)
            {
                theme = Instantiate(_themeObjectPrefab, _themeArea.transform);
                theme.Init(GameManager.GetInstance().GetThemeByNumber(i), i, _settingsPanel, gameObject, _optionsPanel, _levelManager);
                theme._buttonComponent.onClick.AddListener(theme.DisablePanels);
            }

            _alreadyCreated = true;
        }
    }
}
