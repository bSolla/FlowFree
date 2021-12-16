using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Game Object references")]
    public BoardManager _boardManager;            // BoardManager instance
    public Canvas _canvas;                        // Canvas of the scene
    public Camera _camera;                        // Camera 
    public RectTransform _topPanel;               // Top panel of canvas
    public RectTransform _botPanel;               // Bottom panel of canvas
    public Text _levelText;                       // Text with the level
    
    [Header("End panel UI objects")]
    public GameObject _endPanel;
    public Text _endPanelHeaderText;
    public Image _endPanelHeaderImg;
    public Image _endPanelDetailImg;
    public Text _endPanelMovesText;

    [Header("Settings panel UI objects")]
    public Image _optPanelHeaderImg;
    public Image _optPanelDetailImg;


    private bool _paused = false;                 // Pause flag for Input control
    const string MOVES_MSG = "you completed the level in ";
    // ----------------------------------------------
    // --------------- UNITY METHODS ----------------
    // ----------------------------------------------

    void Awake()
    {
        if (_boardManager == null)
        {
            Debug.LogError("Board Manager reference not set");
        } // if
        else
        {
            _boardManager.Init(this);
        } // else
    } // Awake

    private void Start()
    {
        SetUIColors();

        // set callbacks
        //_homePauseButton.onClick.AddListener(GameManager.GetInstance().ReturnToMenu);
        //_homeEndedButton.onClick.AddListener(GameManager.GetInstance().ReturnToMenu);
        //_finalHomeButton.onClick.AddListener(GameManager.GetInstance().ReturnToMenu);

        PlayLevel();
    } // Start


    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    /// <summary>
    /// 
    /// Sets all the scene to play the next level. 
    /// 
    /// </summary>
    public void PlayLevel()
    {
        // Set info
        // ... ad manager, pause, etc

        // Prepare board
        int level = GameManager.GetInstance().GetLevel();
        Map map = Map.FromLot(GameManager.GetInstance().GetLevelLot(), level);

        _levelText.text = "level " + level.ToString();
        _levelText.color = GameManager.GetInstance().GetPackageColor();

        _boardManager.EmptyBoard();
        _boardManager.SetMap(map);
    }

    /// <summary>
    /// 
    /// Receive new Input and process it. 
    /// 
    /// </summary>
    /// <param name="it"> (InputType) Type of new input. </param>
    public void ReceiveInput(InputManager.InputType it, Vector2 pos)
    {
        if (!_paused)
        {
            _boardManager.ReceiveInput(it, pos);
        } // if
    } // ReceiveInput


    public void SetPause(bool isPaused)
    {
        _paused = isPaused;
    }

    public void ShowEndPanel(bool isPerfect, int moves)
    {
        _paused = true;
        _endPanel.SetActive(true);
        
        if (isPerfect)
            _endPanelHeaderText.text = "perfect!";
        else
            _endPanelHeaderText.text = "level complete!";

        _endPanelMovesText.text = MOVES_MSG + moves.ToString() + " moves";
    }

    public void HideEndPanel()
    {
        _paused = false;
        _endPanel.SetActive(false);
    }

    private void SetUIColors()
    {
        Color packageColor = GameManager.GetInstance().GetPackageColor();
        
        _endPanelHeaderImg.color = new Color(packageColor.r, packageColor.g, packageColor.b, _endPanelHeaderImg.color.a);
        _endPanelDetailImg.color = new Color(packageColor.r, packageColor.g, packageColor.b, _endPanelDetailImg.color.a);

        _optPanelHeaderImg.color = _endPanelHeaderImg.color;
        _optPanelDetailImg.color = _endPanelDetailImg.color;
    }
}
