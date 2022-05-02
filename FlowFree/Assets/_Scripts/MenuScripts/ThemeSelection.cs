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
    public InputManager _inputManager = null;

    [Header("Color changing texts")]
    public Text _textFlow = null;
    public Text _textLevels = null;
    public Text _textSettings = null;
    public Text _textThemeName = null;
    public Text _textThemeHeader = null;

    private bool _alreadyCreated = false;

    public void CreateThemeSelection(int nThemes)
    {
        if (!_alreadyCreated)
        {
            ThemeObject theme;

            for (int i = 0; i < nThemes; ++i)
            {
                theme = Instantiate(_themeObjectPrefab, _themeArea.transform);
                theme.Init(GameManager.GetInstance().GetThemeByNumber(i), i, _settingsPanel, this, _optionsPanel, _inputManager);
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
