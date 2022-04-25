using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Levels")]
    public LevelManager _levelManager = null;     // LevelManager for level
    public LevelPackage[] _levels;                // Array of LevelPackages

    [Header("Themes")]
    public Colorway[] _themesScriptObj;

    [Header("Scene names")]
    public string _mainMenuName = "MainMenu";
    public string _playSceneName = "PlayScene";

    // LEVEL DATA
    private string _package = "Rectangles";       // Sets game style
    private string _lot = "HourglassPack";        // Sets lot to use
    private int _level = 5;                       // Sets the level to be loaded

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

            // Get Player information and store it
            _player = SaveLoadSystem.ReadPlayerData(lotNames);
            DontDestroyOnLoad(_instance);
        } // if
        else if (_instance != this)
        {
            _instance._levelManager = _levelManager;

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
        if (SceneManager.GetActiveScene().name == _mainMenuName)
        {
            _levelManager.AdCompleted();
        } // if
    }

    public void IncreaseHints()
    {
        _player._hints++;
        _levelManager.UpdateInfoUI(null, null, null, null);
    }

    public void DecreaseHints()
    {
        _player._hints--;
        _levelManager.UpdateInfoUI(null, null, null, null);
    }

    public void ReceiveInput(InputManager.InputType it, Vector2 pos)
    {
        _levelManager.ReceiveInput(it, pos);
    }

    public void LoadPlayScene()
    {
        SceneManager.LoadScene(_playSceneName);
    }

    public void LoadMainMenu()
    {
        _reloadPanels = true;
        SceneManager.LoadScene(_mainMenuName);
    }

    public void LoadNextLevel()
    {
        if (_level < 149)
        {
            _level += 1;
            LoadPlayScene();
        }
    }

    public void LoadPreviousLevel()
    {
        if (_level > 0)
        {
            _level -= 1;
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
        _package = p;
    } // SetPackage

    /// <summary>
    /// Sets the lot selected by the player
    /// </summary>
    /// <param name="l"> (string) Lot selected </param>
    public void SetLot(string l)
    {
        l = l.Replace(" ", String.Empty);   // prevention of spaces in the name
        _lot = l;
    }

    /// <summary>
    /// 
    /// Sets the Level selected by the player.
    /// 
    /// </summary>
    /// <param name="i"> (int) Level selected. </param>
    public void SetLevel(int i)
    {
        _level = i;
    } // SetLevel

    /// <summary>
    /// 
    /// Sets the MainMenuManager.
    /// 
    /// </summary>
    /// <param name="mg"> (MainMenuManager) Current Main menu. </param>
    public void SetMainMenuManager(MainMenuManager mg)
    {
        _mainMenu = mg;
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
        _player._themeIndex = index;
    }
    #endregion

    #region getters
    /// <summary>
    /// 
    /// Gives the number of packages registered in the game.
    /// 
    /// </summary>
    /// <returns> (int) Number of packages. </returns>
    public int GetNumPackages()
    {
        return _levels.Length;
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
        return _package;
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
        return _levels[i];
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
    public LevelLot GetLevelLot()
    {
        LevelPackage pack = GetLevelPackage();
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
        return _level;
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
        return _player;
    } // GetPlayerData


    public bool GetReloadPanels()
    {
        return _reloadPanels;
    }

    public Colorway GetTheme()
    {
        return _themesScriptObj[_player._themeIndex];
    }

    public Colorway GetThemeByNumber(int i)
    {
        return _themesScriptObj[i];
    }

    public int GetNumberThemes()
    {
        return _themesScriptObj.Length;
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
