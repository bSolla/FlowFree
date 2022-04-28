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
    public GameObject _hintsMessagePanel;         // Panel that gives hint feedback
    public ThemeSelection _themeSelectionPanel;   // Used for changing color of some texts
    public Image _perfectMarker;                  // Image that shows up when a level has been perfected

    [Header("Info UI")]
    public Text _infoFlows;
    public Text _infoMoves;
    public Text _infoBest;
    public Text _infoPipe;
    public Text _infoHints;

    [Header("Buttons")]
    public Button _backToMenuButton;
    public Button _prevLevelButton;
    public Button _reloadButton;
    public Button _nextLevelButton;
    public Button _endNextButton;
    public Button _perfectEndNextButton;
    public Button _hintButton;
    public Button _endHintButton;
    public Button _hintEarnedButton;
    
    [Header("End panel UI objects")]
    public GameObject _endPanel;
    public Image _endPanelHeaderImg;
    public Image _endPanelDetailImg;
    public Text _endPanelMovesText;
    public GameObject _perfectEndPanel;
    public Image _perfectEndHeaderImg;
    public Image _perfectEndDetailImg;
    public Text _perfectEndPanelMovesText;

    [Header("Settings panel UI objects")]
    public Image _optPanelHeaderImg;
    public Image _optPanelDetailImg;

    [Header("Hints panel UI objects")]
    public Image _hintPanelHeaderImg;
    public Image _hintPanelDetailImg;

    [Header("Hints completed UI objects")]
    public Image _hintCompleteHeaderImg;
    public Image _hintCompleteDetailImg;

    public bool _paused = false;                 // Pause flag for Input control
    const string MOVES_MSG = "you completed the level in ";
    // ----------------------------------------------
    // --------------- UNITY METHODS ----------------
    // ----------------------------------------------

    void Awake()
    {
        _paused = true;

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
        PrepareUI();

        // set callbacks
        _hintButton.onClick.AddListener(AdManager.GetInstance().ShowRewardedVideo);
        _endHintButton.onClick.AddListener(AdManager.GetInstance().ShowRewardedVideo);
        _hintEarnedButton.onClick.AddListener(GameManager.GetInstance().IncreaseHints);

        PlayLevel();

        _paused = false;
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
        AdManager.GetInstance().ShowVideo();

        // Prepare board
        int level = GameManager.GetInstance().GetLevel();
        Map map = Map.FromLot(GameManager.GetInstance().GetLevelLot(), level);

        _levelText.text = "level " + (level + 1).ToString() ;
        _levelText.color = GameManager.GetInstance().GetPackageColor();

        _boardManager.EmptyBoard();
        _boardManager.SetMap(map);

        string bestMoves = GameManager.GetInstance().GetPlayerData()._numberOfMoves[GameManager.GetInstance().GetLevelLot()._lotName][level].ToString();
        UpdateInfoUI("0", "0", bestMoves, "0");
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

    /// <summary>
    /// 
    /// Shows the right end panel depending of how the level was completed
    /// (perfect or not), and uses the game manager to save that info into
    /// the player data
    /// 
    /// </summary>
    /// <param name="isPerfect">(bool) if the level was completed with the minimum
    /// number of moves or not</param>
    /// <param name="moves">(int) number of moves</param>
    public void ShowEndPanel(bool isPerfect, int moves)
    {
        _paused = true; // make sure no board input is processed
        int levelStatus = 0; // 0 means not completed
        string lotName = GameManager.GetInstance().GetLevelLot()._lotName;
        int levelNumber = GameManager.GetInstance().GetLevel();

        // checks for perfects and saves completed levels if necessary
        if (isPerfect)
        {
            _perfectEndPanel.SetActive(true);
            _perfectEndPanelMovesText.text = MOVES_MSG + moves + " moves";
            levelStatus = 2; // 2 means completed AND perfect
            
        }
        else
        {
            _endPanel.SetActive(true);
            _endPanelMovesText.text = MOVES_MSG + moves + " moves";

            // only change save data if it comes from not being complete to avoid erasing perfect scores
            if (GameManager.GetInstance().GetPlayerData().
            _completedLevelsLot[lotName][levelNumber] == 0)
                levelStatus = 1; // 1 means completed but not perfect
        }

        if (levelStatus > 0)
        {
            GameManager.GetInstance().GetPlayerData()._completedLevelsLot[lotName][levelNumber] = levelStatus;
        }

        // checks for updating the best number of moves
        int minMoves = GameManager.GetInstance().GetPlayerData()._numberOfMoves[lotName][levelNumber];
        if (minMoves == 0 || minMoves > moves)
        {
            GameManager.GetInstance().GetPlayerData()._numberOfMoves[lotName][levelNumber] = moves;
            UpdateInfoUI(null, null, moves.ToString(), null);
        }
    }

    public void HideEndPanel()
    {
        _endPanel.SetActive(false);
        _perfectEndPanel.SetActive(false);
    }

    private void PrepareUI()
    {
        Color packageColor = GameManager.GetInstance().GetPackageColor();
        
        _endPanelHeaderImg.color = new Color(packageColor.r, packageColor.g, packageColor.b, _endPanelHeaderImg.color.a);
        _endPanelDetailImg.color = new Color(packageColor.r, packageColor.g, packageColor.b, _endPanelDetailImg.color.a);
        _perfectEndHeaderImg.color = _endPanelHeaderImg.color;
        _perfectEndDetailImg.color = _endPanelDetailImg.color;

        _optPanelHeaderImg.color = _endPanelHeaderImg.color;
        _optPanelDetailImg.color = _endPanelDetailImg.color;

        _hintPanelHeaderImg.color = _endPanelHeaderImg.color;
        _hintPanelDetailImg.color = _endPanelDetailImg.color;

        _hintCompleteHeaderImg.color = _endPanelHeaderImg.color;
        _hintCompleteDetailImg.color = _endPanelDetailImg.color;


        if (GameManager.GetInstance().GetLevel() == 0)
            _prevLevelButton.interactable = false;
        else if (GameManager.GetInstance().GetLevel() == 149)
            _nextLevelButton.interactable = false;

        _themeSelectionPanel.ChangeTextColors(GameManager.GetInstance().GetTheme());

        // activates the perfect marker if the level has been perfected before
        _perfectMarker.gameObject.SetActive(GameManager.GetInstance().GetPlayerData().
            _completedLevelsLot[GameManager.GetInstance().GetLevelLot()._lotName][GameManager.GetInstance().GetLevel()] == 2);
    }


    /// <summary>
    /// 
    /// Update the text inside some or all the info UI objects. If a parameter
    /// is null, that object won't be updated
    /// 
    /// </summary>
    /// <param name="flow">(string) new value for flow UI</param>
    /// <param name="moves">(string) new value for moves UI</param>
    /// <param name="best">(string) new value for best UI</param>
    /// <param name="pipe">(string) new value for pipe UI</param>
    public void UpdateInfoUI(string flow, string moves, string best, string pipe)
    {
        
        if (flow != null)
            _infoFlows.text = "flow: " + flow + "/" + _boardManager.GetTotalFlows();
        if (moves != null)
            _infoMoves.text = "moves: " + moves;
        if (best != null)
        {
            if (best == "0") best = "-";
            _infoBest.text = "best: " + best;
        }
        if (pipe != null)
            _infoPipe.text = "pipe: " + pipe + "%";
        
        _infoHints.text = "x" + GameManager.GetInstance().GetPlayerData()._hints;
    }


    /// <summary>
    /// 
    /// Called when a rewarded ad is successful. Shows a panel.
    /// 
    /// </summary>
    public void AdCompleted()
    {
        _hintsMessagePanel.SetActive(true);
    } // AdCompleted

    /// <summary>
    /// 
    /// Used by UI buttons to increase level
    /// 
    /// </summary>
    public void NextLevelCallback()
    {
        GameManager.GetInstance().LoadNextLevel();
    }

    /// <summary>
    /// 
    /// Used by UI buttons to reload the scene
    /// 
    /// </summary>
    public void ReloadLevelCallback()
    {
        GameManager.GetInstance().LoadPlayScene();
    }

    /// <summary>
    /// 
    /// Used by UI buttons to load the previous level
    /// 
    /// </summary>
    public void PreviousLevelCallback()
    {
        GameManager.GetInstance().LoadPreviousLevel();
    }

    /// <summary>
    /// 
    /// Used by UI buttons to load the main menu
    /// 
    /// </summary>
    public void LoadMainMenuCallback()
    {
        GameManager.GetInstance().LoadMainMenu();
    }
}
