using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Resolution Configuration")]
    public Canvas _cnv;                           // Canvas of the scene
    public Camera _cam;                           // Camera of the scene

    [Header("Canvas objects")]
    public RectTransform _topPanel;              // Top panel of the canvas
    public RectTransform _bottomPanel;           // Bottom panel of the canvas

    [Header("Levels")]
    public LevelManager _levelManager = null;     // LevelManager for level
    public LevelPackage[] _levels;                // Array of LevelPackages

    [Header("Themes")]
    public Themes _themesScriptObj;               // scriptable object that contains theme info

    [Header("Debugging")]
    public bool _debugging = false;               // Sets if debug mode is on, for avoiding some changes

    // THEME DATA
    private int _themeIndex = 0;                  // current theme used

    // LEVEL DATA
    private string _package = "Rectangles";       // Sets game style
    private string _lot = "HourglassPack";        // Sets lot to use
    private int _level = 5;                       // Sets the level to be loaded

    // SCALING DATA
    private Vector2 _scalingReferenceResolution;  // Reference resolution for scaling
    private Scaling _scalator;                    // Scaling object

    // GAME/SCENE MANAGEMENT
    private PlayerData _player;                   // Player data
    private MainMenuManager _mainMenu;            // MainMenuManager to change things and update data
    private int _lastScene;                       // Last scene to return to it if necessary
    private bool _reloadPanels = false;

    #region Singleton
    /// <summary>
    /// 
    /// Variable that stores the instance of the GameManager, Singleton
    /// 
    /// </summary>
    private static GameManager _instance;

    /// <summary>
    /// 
    /// Awake function of GameManager. Checks if another instance of this GameObject exists and 
    /// if not, initializes all required atributes and values of the GameManager, creating a new
    /// one. 
    /// 
    /// If the GameManager already exists, destroy this gameObject. 
    /// 
    /// </summary>
    private void Awake()
    {
        // If GameManager is not created and initialized...
        if (_instance == null)
        {
            // Set this GameManager as instance
            _instance = this;

            List<string> lotNames = new List<string>();
            for (int i = 0; i < _levels.Length; ++i)
            {
                foreach (LevelLot lot in _levels[i]._lotArray)
                {
                    lotNames.Add(lot._lotName);
                }
            }

            // Store canvas' scaling reference resolution
            _scalingReferenceResolution = _cnv.GetComponent<CanvasScaler>().referenceResolution;

            // Create scalator
            Vector2 res = new Vector2(Screen.width, Screen.height);
            _scalator = new Scaling(res, _scalingReferenceResolution, (int)_cam.orthographicSize);

            // Get Player information and store it
            _player = SaveLoadSystem.ReadPlayerData(lotNames);
            _themeIndex = _player._themeIndex;
            DontDestroyOnLoad(_instance);
        } // if
        else if (_instance != this)
        {
            _instance._levelManager = _levelManager;
            _instance._topPanel = _topPanel;
            _instance._bottomPanel = _bottomPanel;
            _instance._cam = _cam;
            _instance._cnv = _cnv;

            Destroy(gameObject);
        } // else if
    } // Awake


    /// <summary>
    /// 
    /// Gives access to the GameManager instance for the rest of scripts and objects, 
    /// also is used for changing some values in the gameManager only by the GameManager.
    /// 
    /// </summary>
    /// <returns> (GameManager) Actual instance of the GameManager. </returns>
    public static GameManager GetInstance()
    {
        return _instance;
    } // GetInstance

    #endregion

    /// <summary>
    /// 
    /// Function called when a rewarded ad ended successfully.
    /// 
    /// </summary>
    public void AdEnded()
    {
        // MainMenu
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            _levelManager.AdCompleted();
        } // if
    }

    public void IncreaseHints()
    {
        GetInstance()._player._hints++;
    }

    public void ReceiveInput(InputManager.InputType it, Vector2 pos)
    {
        _levelManager.ReceiveInput(it, pos);
    }

    public void LoadPlayScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadMainMenu()
    {
        _reloadPanels = true;
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        if (GetInstance()._level < 149)
        {
            GetInstance()._level += 1;
            LoadPlayScene();
        }
    }

    public void LoadPreviousLevel()
    {
        if (GetInstance()._level > 0)
        {
            GetInstance()._level -= 1;
            LoadPlayScene();
        }
    }

    /// <summary>
    /// 
    /// Function called when the Level is completed. Updates the level and
    /// calls the level manager so it shows the end panel.
    /// 
    /// </summary>
    public void LevelCompleted()
    {
        //_level++;
        //if (_level < GameManager.GetInstance().GetLevelPackage().levels.Length)
        //{
        //    if (GetInstance()._player._completedLevelsPackage[_package] <= _level)
        //        GetInstance()._player._completedLevelsPackage[_package]++;
        //    _levelManager.ShowEndMenu();
        //}
        //else
        //{
        //    _levelManager.ShowFinalMenu();
        //}

    } // LevelCompleted

    #region Setters

    /// <summary>
    /// 
    /// Sets the package selected by the player.
    /// 
    /// </summary>
    /// <param name="p"> (string) Package selected. </param>
    public void SetPackage(string p)
    {
        p = p.Replace(" ", String.Empty);   // prevention of spaces in the name
        GetInstance()._package = p;
    } // SetPackage

    /// <summary>
    /// Sets the lot selected by the player
    /// </summary>
    /// <param name="l"> (string) Lot selected </param>
    public void SetLot(string l)
    {
        l = l.Replace(" ", String.Empty);   // prevention of spaces in the name
        GetInstance()._lot = l;
    }

    /// <summary>
    /// 
    /// Sets the Level selected by the player.
    /// 
    /// </summary>
    /// <param name="i"> (int) Level selected. </param>
    public void SetLevel(int i)
    {
        GetInstance()._level = i;
    } // SetLevel

    /// <summary>
    /// 
    /// Sets the MainMenuManager.
    /// 
    /// </summary>
    /// <param name="mg"> (MainMenuManager) Current Main menu. </param>
    public void SetMainMenuManager(MainMenuManager mg)
    {
        GetInstance()._mainMenu = mg;
    } // SetMainMenuManager

    /// <summary>
    /// 
    /// Setter for the reload panels bool 
    /// 
    /// </summary>
    /// <param name="val">(bool) wether or not to reload the panels</param>
    public void SetReloadPanels(bool val)
    {
        _reloadPanels = val;
    }

    /// <summary>
    /// 
    /// Sets the theme index to the provided value
    /// 
    /// </summary>
    /// <param name="index">(int) new value of the theme index</param>
    public void SetThemeIndex(int index)
    {
        _themeIndex = index;
    }
    #endregion

    #region getters
    /// <summary>
    /// 
    /// Gives access to the scalator instance.
    /// 
    /// </summary>
    /// <returns> (Scaling) Scaling instance stored in GM instance. </returns>
    public Scaling GetScaling()
    {
        return GetInstance()._scalator;
    } // GetScaling

    /// <summary>
    /// 
    /// Gives access to the canvas stored in instance.
    /// 
    /// </summary>
    /// <returns> (Canvas) Canvas access. </returns>
    public Canvas GetCanvas()
    {
        return GetInstance()._cnv;
    } // GetCanvas

    /// <summary>
    /// 
    /// Returns the height of the top panel.
    /// 
    /// </summary>
    /// <returns> (float) Panel height. </returns>
    public float GetTopPanelHeight()
    {
        return GetInstance()._topPanel.rect.height;
    } // GetTopPanelHeight

    /// <summary>
    /// 
    /// Returns the height of the bottom panel in pixels.
    /// 
    /// </summary>
    /// <returns> (float) Height of panel </returns>
    public float GetBottomPanelHeight()
    {
        return GetInstance()._bottomPanel.rect.height;
    } // GetTopPanelHeight

    /// <summary>
    /// 
    /// Gives the reference resolution used when scaling things for later use. 
    /// 
    /// </summary>
    /// <returns> (Vector2) Reference resolution. </returns>
    public Vector2 GetReferenceResolution()
    {
        return GetInstance()._scalingReferenceResolution;
    } // GetReferenceResolution


    /// <summary>
    /// 
    /// Gives the number of packages registered in the game.
    /// 
    /// </summary>
    /// <returns> (int) Number of packages. </returns>
    public int GetNumPackages()
    {
        return GetInstance()._levels.Length;
    } // GetNumPackages

    public int GetNumLotsInPackage(LevelPackage levPack)
    {
        return levPack._lotArray.Length;
    }

    /// <summary>
    /// 
    /// Get the name of the current package selected.
    /// 
    /// </summary>
    /// <returns> (string) Package name.</returns>
    public string GetPackageName()
    {
        return GetInstance()._package;
    } // GetPackageName

    /// <summary>
    /// 
    /// Gives access to a level package selected
    /// by number, necessary for button instantiation.
    /// 
    /// </summary>
    /// <param name="i"> (int) Package to access. </param>
    /// <returns> (LevelPackage) Package with data. </returns>
    public LevelPackage GetLevelPackage(int i)
    {
        return GetInstance()._levels[i];
    } // GetPackage


    /// <summary>
    /// 
    /// Returns the package of the level selected. Searches in
    /// the LevelPackage's list and compares the names of the 
    /// objects. When object is found, returns it. 
    /// 
    /// If LevelPackage is not found, return null. 
    /// 
    /// </summary>
    /// <returns> (LevelPackage) Selected LevelPackage. </returns>
    public LevelPackage GetLevelPackage()
    {
        for (int i = 0; i < GetInstance()._levels.Length; i++)
        {
            if (GetInstance()._levels[i].name == _package)
            {
                return GetInstance()._levels[i];
            } // if
        } // for

        return null;
    } // getLevelPackage


    /// <summary>
    /// Given a level package, looks for the lot that is stored in 
    /// the game manager. If it can't find it, returns null
    /// </summary>
    /// <returns>(LevelLot) Selected Lot</returns>
    public LevelLot GetLevelLot()
    {
        LevelPackage pack = GetInstance().GetLevelPackage();
        for (int i = 0; i < pack._lotArray.Length; i++)
        {
            if (pack._lotArray[i].name == _lot)
            {
                return pack._lotArray[i];
            } // if
        } // for

        return null;
    }
    /// <summary>
    /// 
    /// Gives access to the actual level selected.
    /// 
    /// </summary>
    /// <returns> (int) Current level. </returns>
    public int GetLevel()
    {
        return GetInstance()._level;
    } // GetLevel

    /// <summary>
    /// Gets the color of the current package
    /// </summary>
    /// <returns>(Color) color of the current package</returns>
    public Color GetPackageColor()
    {
        return GetLevelPackage()._packageColor;
    }


    /// <summary>
    /// 
    /// Gives access to the player data and all the completed level 
    /// and etc.
    /// 
    /// </summary>
    /// <returns> (PlayerData) Actual player data loaded. </returns>
    public PlayerData GetPlayerData()
    {
        return GetInstance()._player;
    } // GetPlayerData


    public bool GetReloadPanels()
    {
        return _reloadPanels;
    }

    public Colorway GetTheme()
    {
        return _themesScriptObj._themeArray[2];
    }
    #endregion getters

    #region AppLifeManagement
    /// <summary>
    /// 
    /// Eventhough it's not an applifemanagement method, here are all
    /// the methods that interact with external data. This method opens
    /// a browser window with the provided link.
    /// 
    /// </summary>
    /// <param name="link"> (string) Link. </param>
    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    } // OpenLink

    /// <summary>
    /// 
    /// Function that will manage the close of the app, saving the player's current status. Not
    /// working in mobile.
    /// 
    /// </summary>
    private void OnApplicationQuit()
    {
        // Save player information
        SaveLoadSystem.SavePlayerData(GetInstance()._player);
    } // OnApplicationQuit

    /// <summary>
    /// 
    /// Save data also when application loses focus, to avoid
    /// losing data and etc.
    /// 
    /// </summary>
    /// <param name="focus"> (bool) Focus status. </param>
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            // Save player information
            SaveLoadSystem.SavePlayerData(GetInstance()._player);
        } // if
    } // OnApplicationFocus

    #endregion
}
