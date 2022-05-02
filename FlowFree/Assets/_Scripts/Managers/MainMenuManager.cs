using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Resolution Configuration")]
    public Canvas _cnv;                           // Canvas of the scene
    public Camera _cam;                           // Camera of the scene

    [Header("Scene References")]
    public RectTransform _scrollContent = null;
    public VerticalLayoutGroup _packageArea = null;
    public GameObject _packageSelectionPanel = null;
    public LevelSelection _levelSelectionPanel = null;
    public ThemeSelection _themeSelectionPanel = null;

    [Header("Prefabs")]
    public GameObject _packageUIPrefab = null;

    [Header("Limits and Info")]
    public int _topLimit = 5;                   // Top panel
    public int _bottomLimit = 10;               // Bottom panel
    public int _leftLimit = 0;                  // Left limit
    public int _rightLimit = 0;                 // Right limit for buttons
    public int _spaceButtons = 12;              // Space between buttons

    private bool _packagesCreated = false;
    
    // SCALING DATA
    private Vector2 _scalingReferenceResolution;  // Reference resolution for scaling
    private Scaling _scalator;                    // Scaling object


    // gameplay data
    private LevelPackage[] _levelPackages;
    private Dictionary<string, PlayerData.CompletedStatus[]> _completionStatusLots;
    private int _nThemes;

    private void Awake()
    {
        // Store canvas' scaling reference resolution
        _scalingReferenceResolution = _cnv.GetComponent<CanvasScaler>().referenceResolution;

        // Create scalator
        Vector2 res = new Vector2(Screen.width, Screen.height);
        _scalator = new Scaling(res, _scalingReferenceResolution, (int)_cam.orthographicSize);
    }

    /// <summary>
    /// Check for missing references and raise errors if they're missing
    /// </summary>
    private void Start()
    {
        if (_scrollContent == null) Debug.LogError("Scroll content reference not set in main menu manager");
        if (_packageArea == null) Debug.LogError("Package area reference not set in main menu manager");
        if (_packageUIPrefab == null) Debug.LogError("Package UI prefab not set in main menu manager");
        if (_levelSelectionPanel == null) Debug.LogError("level selection panel reference not set in main menu manager");
        if (_packageSelectionPanel == null) Debug.LogError("package selection panel reference not set in main menu manager");
    }

    public void Init(int nThemes, LevelPackage[] levelPackages, Dictionary<string, PlayerData.CompletedStatus[]> completionStatusLots)
    {
        _nThemes = nThemes;
        _levelPackages = levelPackages;
        _completionStatusLots = completionStatusLots;

        _themeSelectionPanel.CreateThemeSelection(_nThemes);
        _themeSelectionPanel.ChangeTextColors(GameManager.GetInstance().GetTheme());

        // determines wther to load the main menu from the start or from the level selection panel
        if (GameManager.GetInstance().AreWeComingFromPlayScene())
        {
            ComingFromPlayScene();
            GameManager.GetInstance().SetComingFromPlayScene(false);
        }
    }

    public void CreatePackageSelectionObjects()
    {
        if (!_packagesCreated)
        {
            // Create and instantiate buttons
            int nButtons = _levelPackages.Length;

            InstantiateButtons(nButtons);
           
            _packagesCreated = true;
        }
    }

    public void ComingFromPlayScene()
    {
        _packageSelectionPanel.SetActive(true);
        CreatePackageSelectionObjects();
    }

    /// <summary>
    /// 
    /// Instantiate all buttons (equal to the number of packages registered).
    /// 
    /// </summary>
    /// <param name="nButtons"> (int) Number of packages. </param>
    void InstantiateButtons(int nButtons)
    {
        GameObject button;

        _packageArea.padding.top = _topLimit;
        _packageArea.padding.bottom = _bottomLimit;
        _packageArea.padding.left = _leftLimit;
        _packageArea.padding.right = _rightLimit;
        _packageArea.spacing = _spaceButtons;

        for (int i = 0; i < nButtons; i++)
        {
            button = Instantiate(_packageUIPrefab, _packageArea.transform);

            SetButton(button, i);
        } // for
    } // InstantiateButtons


    /// <summary>
    /// 
    /// Sets the package buttons
    /// 
    /// </summary>
    /// <param name="b"> (GameObject) Button to set. </param>
    /// <param name="packageNumber"> (int) Current package. </param>
    void SetButton(GameObject b, int packageNumber)
    {
        LevelPackage lp = _levelPackages[packageNumber];
        PackageSelection package = b.GetComponent<PackageSelection>();
        
        package.SetSelectionPanelReference(_levelSelectionPanel);
        package.SetPackageName(lp._packageName);
        package.SetPackageColor(lp._packageColor);
        
        package.SetLots(lp, _packageArea, _completionStatusLots);
    } // SetButton

    
}
