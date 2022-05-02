using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Scene Managers")]
    public MainMenuManager _mainMenuManager = null;      // manager found in MainMenuScene
    public LevelManager _levelManager = null;     // manager found in PlayScene

    [Header("Levels")]
    public LevelPackage[] _levels;                // Array of LevelPackages

    [Header("Themes")]
    public Colorway[] _themesScriptObj;

    [Header("Scene names")]
    public string _mainMenuName = "MainMenu";
    public string _playSceneName = "PlayScene";

    // LEVEL DATA
    private string _package = "Rectangles";       // Sets game style
    private string _lotName = "HourglassPack";        // Sets lot to use
    private LevelLot _levelLot;
    private int _levelNumber = 5;                       // Sets the level to be loaded

    // GAME/SCENE MANAGEMENT
    private PlayerData _player;                   // Player data
    private bool _comingFromPlayScene = false;

    #region Singleton
    /// <summary>
    /// 
    /// Variable that stores the instance of the GameManager, Singleton
    /// 
    /// </summary>
    private static GameManager _instance;

    /// <summary>
    /// 
    /// Start function of GameManager. Checks if another instance of this GameObject exists and 
    /// if not, initializes all required atributes and values of the GameManager, creating a new
    /// one. Has to be Start instead of Awake to guarantee the ad manager has already been initialized
    /// 
    /// If the GameManager already exists, destroy this gameObject. 
    /// 
    /// </summary>
    private void Start()
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

            // Get Player information and store it
            _player = SaveLoadSystem.ReadPlayerData(lotNames);
            DontDestroyOnLoad(_instance);

            _instance.SceneSetup();
        } // if
        else if (_instance != this)
        {
            _instance._levelManager = _levelManager;
            _instance._mainMenuManager = _mainMenuManager;

            _instance.SceneSetup();

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

    private void SceneSetup()
    {
        if (_instance._levelManager != null)
        {
            _levelLot = GetLevelLot();

            _instance._levelManager.PrepareLevel(_instance._levelNumber, _instance._levelLot, _instance.GetLevelPackage()._packageColor,
                _instance._player._numberOfMoves[_instance._levelLot._lotName][_instance._levelNumber], 
                (int)_instance._player._completedLevelsLot[_instance._levelLot._lotName][_instance._levelNumber]);

            _instance._levelManager.PlayLevel();
        }
        else if (_instance._mainMenuManager != null)
        {
            _instance._mainMenuManager.Init(_instance._themesScriptObj.Length, _instance._levels, _instance._player._completedLevelsLot);
        }
    }

    /// <summary>
    /// 
    /// Increases the number of hints in player data
    /// 
    /// </summary>
    public void IncreasePlayerHints()
    {
        _player._hints++;
    }

    /// <summary>
    ///  
    /// Decreases the number of hints in player data
    /// 
    /// </summary>
    public void DecreasePlayerHints()
    {
        _player._hints--;
    }

    public void LoadPlayScene()
    {
        SceneManager.LoadScene(_playSceneName);
    }

    public void LoadMainMenu()
    {
        _comingFromPlayScene = true;
        SceneManager.LoadScene(_mainMenuName);
    }

    public void LoadNextLevel()
    {
        if (_levelNumber < 149)
        {
            _levelNumber += 1;
            SceneManager.LoadScene(_playSceneName);
        }
    }

    public void LoadPreviousLevel()
    {
        if (_levelNumber > 0)
        {
            _levelNumber -= 1;
            LoadPlayScene();
        }
    }

    /// <summary>
    /// 
    /// Function called when the Level is completed. Updates the level and
    /// calls the level manager so it shows the end panel.
    /// 
    /// </summary>

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
        _package = p;
    } // SetPackage

    /// <summary>
    /// Sets the lot selected by the player
    /// </summary>
    /// <param name="l"> (string) Lot selected </param>
    public void SetLot(string l)
    {
        l = l.Replace(" ", String.Empty);   // prevention of spaces in the name
        _lotName = l;
    }

    /// <summary>
    /// 
    /// Sets the Level selected by the player.
    /// 
    /// </summary>
    /// <param name="i"> (int) Level selected. </param>
    public void SetLevel(int i)
    {
        _levelNumber = i;
    } // SetLevel

    /// <summary>
    /// 
    /// Setter for the reload panels bool 
    /// 
    /// </summary>
    /// <param name="val">(bool) wether or not to reload the panels</param>
    public void SetComingFromPlayScene(bool val)
    {
        _comingFromPlayScene = val;
    }

    /// <summary>
    /// 
    /// Sets the theme index to the provided value
    /// 
    /// </summary>
    /// <param name="index">(int) new value of the theme index</param>
    public void SetThemeIndex(int index)
    {
        _player._themeIndex = index;
    }

    public void SetCompletionStatus(int status)
    {
        PlayerData.CompletedStatus s = (PlayerData.CompletedStatus)status;
        _player._completedLevelsLot[_levelLot._lotName][_levelNumber] = s;
    }

    public void SetBestMoves(int nMoves)
    {
        _player._numberOfMoves[_levelLot._lotName][_levelNumber] = nMoves;
    }
    #endregion

    #region getters

    private int GetNumLotsInPackage(LevelPackage levPack)
    {
        return levPack._lotArray.Length;
    }

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
    private LevelPackage GetLevelPackage()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            if (_levels[i].name == _package)
            {
                return _levels[i];
            } // if
        } // for

        return null;
    } // getLevelPackage


    /// <summary>
    /// Given a level package, looks for the lot that is stored in 
    /// the game manager. If it can't find it, returns null
    /// </summary>
    /// <returns>(LevelLot) Selected Lot</returns>
    private LevelLot GetLevelLot()
    {
        LevelPackage pack = GetLevelPackage();
        for (int i = 0; i < pack._lotArray.Length; i++)
        {
            if (pack._lotArray[i].name == _lotName)
            {
                return pack._lotArray[i];
            } // if
        } // for

        return null;
    }

    /// <summary>
    /// 
    /// Gives access to the player data and all the completed level 
    /// and etc.
    /// 
    /// </summary>
    /// <returns> (PlayerData) Actual player data loaded. </returns>
    private PlayerData GetPlayerData()
    {
        return _player;
    } // GetPlayerData

    public int GetHintNumber()
    {
        return _player._hints;
    }

    /// <summary>
    /// 
    /// Used by the menu to know if it should display the initial menu or the level
    /// selection menu
    /// 
    /// </summary>
    /// <returns>(bool) _comingFromPlayScene</returns>
    public bool AreWeComingFromPlayScene()
    {
        return _comingFromPlayScene;
    }

    public bool AdsRemoved()
    {
        return _player._adsRemoved;
    }

    public Colorway GetTheme()
    {
        return _themesScriptObj[_player._themeIndex];
    }

    public Colorway GetThemeByNumber(int i)
    {
        return _themesScriptObj[i];
    }
    #endregion getters

    #region AppLifeManagement
    /// <summary>
    /// 
    /// Function that will manage the close of the app, saving the player's current status. Not
    /// working in mobile.
    /// 
    /// </summary>
    private void OnApplicationQuit()
    {
        // Save player information
        SaveLoadSystem.SavePlayerData(_player);
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
            SaveLoadSystem.SavePlayerData(_player);
        } // if
    } // OnApplicationFocus

    #endregion
}
