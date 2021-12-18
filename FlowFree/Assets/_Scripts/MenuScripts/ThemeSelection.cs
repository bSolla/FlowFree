using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    [Header("Color changing texts")]
    public Text _textFlow = null;
    public Text _textLevels = null;
    public Text _textSettings = null;
    public Text _textThemeName = null;
    public Text _textThemeHeader = null;

    private bool _alreadyCreated = false;

    private void Start()
    {
        if (_themeObjectPrefab == null) Debug.LogError("Missing prefab in Theme Selection panel");
        if (_themeArea == null) Debug.LogError("Missing vertical scroll area refernece in theme selection panel");
        if (_settingsPanel == null) Debug.LogError("missing settings panel reference in theme selection panel");
        if (_optionsPanel == null) Debug.LogWarning("missing options panel reference in theme selection panel");
        if (_levelManager == null) Debug.LogWarning("missing level manager reference in theme selection panel");
        if (_textFlow == null) Debug.LogWarning("missing reference to the Flow text in theme selection panel");
        if (_textLevels == null) Debug.LogWarning("missing reference to the Level selector text in theme selection panel");
        if (_textSettings == null) Debug.LogError("missing reference to the Settings text object in the theme selection panel");
        if (_textThemeName == null) Debug.LogError("missing reference to the Theme name text object in the theme selection panel");
        if (_textThemeHeader == null) Debug.LogError("missing reference to the theme panel header object");

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
                theme.Init(GameManager.GetInstance().GetThemeByNumber(i), i, _settingsPanel, this, _optionsPanel, _levelManager);
                theme._buttonComponent.onClick.AddListener(theme.ThemeChange);
            }

            _alreadyCreated = true;
        }
    }

    public void ChangeTextColors(Colorway theme)
    {
        if (_textFlow != null)
        {
            string flow = CreateColoredText("flow", theme);
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write("<size=40><color=#");
            streamWriter.Write(ColorUtility.ToHtmlStringRGBA(theme._arrayColors[4]));
            streamWriter.Write(">®</color></size>");
            streamWriter.Flush();
            string result = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            _textFlow.text = flow + result;
        }

        if (_textLevels != null)
            _textLevels.text = CreateColoredText("levels", theme);

        _textSettings.text = CreateColoredText("settings", theme);

        _textThemeName.text = "theme: " + CreateColoredText(theme._shortName, theme);

        _textThemeHeader.text = CreateColoredText("themes", theme);
    }

    string CreateColoredText(string source, Colorway theme)
    {
        MemoryStream memoryStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(memoryStream);

        for (int i = 0; i < source.Length; ++i)
        {
            streamWriter.Write("<color=#");
            streamWriter.Write(ColorUtility.ToHtmlStringRGBA(theme._arrayColors[i]));
            streamWriter.Write(">");
            streamWriter.Write(source[i]);
            streamWriter.Write("</color>");
        }
        streamWriter.Flush();
        return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int)memoryStream.Length);
    }
}
