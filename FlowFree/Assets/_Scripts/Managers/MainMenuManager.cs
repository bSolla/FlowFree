using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene References")]
    public RectTransform _scrollContent = null;
    public VerticalLayoutGroup _packageArea = null;
    public GameObject _packageSelectionPanel = null;
    public LevelSelection _levelSelectionPanel = null;

    [Header("Prefabs")]
    public GameObject _packageUIPrefab = null;

    [Header("Limits and Info")]
    public int _topLimit = 5;                   // Top panel
    public int _bottomLimit = 10;               // Bottom panel
    public int _leftLimit = 0;                  // Left limit
    public int _rightLimit = 0;                 // Right limit for buttons
    public int _spaceButtons = 12;              // Space between buttons

    private bool _packagesCreated = false;

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

        GameManager.GetInstance().SetMainMenuManager(this);
        if(GameManager.GetInstance().GetReloadPanels())
        {
            ComingFromPlayScene();
            GameManager.GetInstance().SetReloadPanels(false);
        }
    }

    public void CreatePackageSelectionObjects()
    {
        if (!_packagesCreated)
        {
            // Create and instantiate buttons
            int nButtons = GameManager.GetInstance().GetNumPackages();

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
        LevelPackage lp = GameManager.GetInstance().GetLevelPackage(packageNumber);
        PackageSelection package = b.GetComponent<PackageSelection>();
        
        package.SetSelectionPanelReference(_levelSelectionPanel);
        package.SetPackageName(lp.name);
        package.SetPackageColor(lp._packageColor);
        
        package.SetLots(lp, _packageArea);
    } // SetButton

    /// <summary>
    /// 
    /// Called when a rewarded ad is successful. Shows a panel.
    /// 
    /// </summary>
    public void AdCompleted()
    {
        Debug.Log("Panel should show here");
        //_hintsMessagePanel.SetActive(true);
        //_hintsMessage.Play("hints_window");
    } // AdCompleted
}
