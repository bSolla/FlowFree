using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Resolution Configuration")]
    public Canvas _cnv;                           // Canvas of the scene
    public Camera _cam;                           // Camera of the scene

    [Header("Levels")]
    public LevelManager _levelManager = null;     // LevelManager for level
    public LevelPackage[] _levels;                // Array of LevelPackages

    [Header("Debugging")]
    public bool _debugging = false;               // Sets if debug mode is on, for avoiding some changes

    private string _package = "Classic";          // Sets game style
    private int _level = 10;                      // Sets the level to be loaded

    // SCALING DATA
    private Vector2 _scalingReferenceResolution;  // Reference resolution for scaling
    private Scaling _scalator;                    // Scaling object

    // GAME/SCENE MANAGEMENT
    private PlayerData _player;                   // Player data
    private RectTransform _topPanel;              // Top panel of the canvas
    private RectTransform _bottomPanel;           // Bottom panel of the canvas
    private MainMenuManager _mainMenu;            // MainMenuManager to change things and update data
    private int _lastScene;                       // Last scene to return to it if necessary

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

            string[] packagesNames = new string[_levels.Length];

            for (int i = 0; i < _levels.Length; i++)
            {
                packagesNames[i] = _levels[i].name;
            } // for

            // Store canvas' scaling reference resolution
            _scalingReferenceResolution = _cnv.GetComponent<CanvasScaler>().referenceResolution;

            // Create scalator
            Vector2 res = new Vector2(Screen.width, Screen.height);
            _scalator = new Scaling(res, _scalingReferenceResolution, (int)_cam.orthographicSize);

            // Get Player information and store it
            //_player = FileLoader.ReadPlayerData(packagesNames);

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
    /// Load all panels in scene for dimensions info.
    /// 
    /// </summary>
    public void ReloadPanels()
    {
        // Search in the canvas and check names
        foreach (Transform child in GetInstance()._cnv.transform)
        {
            if (child.name == "Top")
            {
                GetInstance()._topPanel = child.GetComponent<RectTransform>();
            } // if
            else if (child.name == "Bottom")
            {
                GetInstance()._bottomPanel = child.GetComponent<RectTransform>();
            } // if
        } // foreach
    } // ReloadPanels


    #region Setters

    /// <summary>
    /// 
    /// Set the camera of the scene as the instance's camera.
    /// 
    /// </summary>
    /// <param name="c"> (Camera) Camera to be set. </param>
    public void SetCamera(Camera c)
    {
        // Set the camera
        GetInstance()._cam = c;
    } // SetCamera

    /// <summary>
    /// 
    /// Set the current canvas. 
    /// 
    /// </summary>
    /// <param name="c"> (Canvas) Canvas of the scene. </param>
    public void SetCanvas(Canvas c)
    {
        GetInstance()._cnv = c;
        ReloadPanels();
    } // SetCanvas

    /// <summary>
    /// 
    /// Sets the package selected by the player.
    /// 
    /// </summary>
    /// <param name="p"> (string) Package selected. </param>
    public void SetPackage(string p)
    {
        GetInstance()._package = p;
    } // SetPackage

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
    /// Gives access to the actual level selected.
    /// 
    /// </summary>
    /// <returns> (int) Current level. </returns>
    public int GetLevel()
    {
        return GetInstance()._level;
    } // GetLevel

    #endregion getters
}
