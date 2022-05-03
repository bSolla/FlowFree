using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 
/// Event class used to send input info to the level manager
/// 
/// </summary>
[System.Serializable]
public class PauseEvent : UnityEvent<bool>
{
}

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

    [Header("Pause event callbacks")]
    public PauseEvent _pauseEvents;

    const string MOVES_MSG = "you completed the level in ";

    // level info
    int _level;
    LevelLot _levelLot;
    Color _packageColor;
    int _nThemes;
    int _bestMoves;
    int _completionStatus;

    // ----------------------------------------------
    // --------------- UNITY METHODS ----------------
    // ----------------------------------------------

    void Awake()
    {
        if (_boardManager == null)
        {
            Debug.LogError("Board Manager reference not set");
        } // if
    } // Awake

    //private void Start()
    //{
    //    PrepareUI();

    //    // set callbacks
    //    _hintButton.onClick.AddListener(AdManager.GetInstance().ShowRewardedVideo);
    //    _endHintButton.onClick.AddListener(AdManager.GetInstance().ShowRewardedVideo);

    //    PlayLevel();
    //} // Start


    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    public void PrepareLevel(int lvl, LevelLot levelLot, Color pkgColor, int nThemes, int nMoves, int completionStatus)
    {
        _level = lvl;
        _levelLot = levelLot;
        _packageColor = pkgColor;
        _nThemes = nThemes;
        _bestMoves = nMoves;
        _completionStatus = completionStatus;

        _boardManager.Init(this, _packageColor);

        PrepareUI();

        // set callbacks
        _hintButton.onClick.AddListener(AdManager.GetInstance().ShowRewardedVideo);
        _endHintButton.onClick.AddListener(AdManager.GetInstance().ShowRewardedVideo);
    }

    /// <summary>
    /// 
    /// Sets all the scene to play the next level. 
    /// 
    /// </summary>
    public void PlayLevel()
    {
        // Set info
        AdManager.GetInstance().ShowInterstitialVideo();

        // Prepare board
        Map map = Map.FromLot(_levelLot, _level);

        _levelText.text = "level " + (_level + 1).ToString() ;
        _levelText.color = _packageColor;

        _boardManager.EmptyBoard();
        _boardManager.SetMap(map);

        //string bestMoves = GameManager.GetInstance().GetPlayerData()._numberOfMoves[GameManager.GetInstance().GetLevelLot()._lotName][level].ToString();
        UpdateInfoUI("0", "0", _bestMoves.ToString(), "0");
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
        _pauseEvents.Invoke(true);

        int levelStatus = 0; // 0 means not completed
        string lotName = _levelLot._lotName;

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
            if (_completionStatus == 0)
                levelStatus = 1; // 1 means completed but not perfect
        }

        if (levelStatus > 0)
        {
            GameManager.GetInstance().SetCompletionStatus(levelStatus);
        }

        // checks for updating the best number of moves
        if (_bestMoves == 0 || _bestMoves > moves)
        {
            GameManager.GetInstance().SetBestMoves(moves);
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
        _endPanelHeaderImg.color = new Color(_packageColor.r, _packageColor.g, _packageColor.b, _endPanelHeaderImg.color.a);
        _endPanelDetailImg.color = new Color(_packageColor.r, _packageColor.g, _packageColor.b, _endPanelDetailImg.color.a);
        _perfectEndHeaderImg.color = _endPanelHeaderImg.color;
        _perfectEndDetailImg.color = _endPanelDetailImg.color;

        _optPanelHeaderImg.color = _endPanelHeaderImg.color;
        _optPanelDetailImg.color = _endPanelDetailImg.color;

        _hintPanelHeaderImg.color = _endPanelHeaderImg.color;
        _hintPanelDetailImg.color = _endPanelDetailImg.color;

        _hintCompleteHeaderImg.color = _endPanelHeaderImg.color;
        _hintCompleteDetailImg.color = _endPanelDetailImg.color;


        if (_level == 0)
            _prevLevelButton.interactable = false;
        else if (_level == 149)
            _nextLevelButton.interactable = false;

        _themeSelectionPanel.CreateThemeSelection(_nThemes);
        _themeSelectionPanel.ChangeTextColors(GameManager.GetInstance().GetTheme());

        // activates the perfect marker if the level has been perfected before
        //_perfectMarker.gameObject.SetActive(GameManager.GetInstance().GetPlayerData().
        //    _completedLevelsLot[GameManager.GetInstance().GetLevelLot()._lotName][GameManager.GetInstance().GetLevel()] == 2);
        _perfectMarker.gameObject.SetActive(_completionStatus == 2);
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
        
        _infoHints.text = "x" + GameManager.GetInstance().GetHintNumber();
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

    /// <summary>
    /// 
    /// Used by play scene objects to increase player hints
    /// 
    /// </summary>
    public void IncreaseHints()
    {
        GameManager.GetInstance().IncreasePlayerHints();
        UpdateInfoUI(null, null, null, null);
    }

    /// <summary>
    /// 
    /// Used by the board manager when the player uses up a hint
    /// 
    /// </summary>
    public void UseUpHint()
    {
        GameManager.GetInstance().DecreasePlayerHints();
        UpdateInfoUI(null, null, null, null);
    }


    /// <summary>
    /// 
    /// Used by buttons on the Claim Hint panel, it makes sure to increase the hints and perform all other necessary tasks
    /// 
    /// </summary>
    /// <param name="claimPanel">(GameObject) reference to the claim hint panel, to deactivate it</param>
    public void ClaimHintCallback(GameObject claimPanel)
    {
        if (!_perfectEndPanel.active) // doesn't unpause if the perfect end panel is active (in case of free hint from that panel)
        {
            _pauseEvents.Invoke(false);
        }

        claimPanel.SetActive(false);

        IncreaseHints();
    }

    public void LevelCompleted(bool isPerfect, int moves)
    {
        ShowEndPanel(isPerfect, moves);
    }
}
