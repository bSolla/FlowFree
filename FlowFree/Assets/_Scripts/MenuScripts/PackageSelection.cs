using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackageSelection : MonoBehaviour
{
    [Header("Game object references")]
    public Text _packageName = null;
    public Image _lineDetail = null;
    public Image _background = null;

    [Header("Prefabs")]
    public LotSelection _lotSelectionPrefab = null;

    private LevelSelection _levelSelectionPanel;
    

    // Start is called before the first frame update
    void Start()
    {
        if (_packageName == null) Debug.LogError("Text reference not set in Package selection object " + gameObject.name);
        if (_lineDetail == null) Debug.LogError("Line detail image reference not set in Package selection object " + gameObject.name);
        if (_background == null) Debug.LogError("Background image reference not set in package selection object " + gameObject.name);
        if (_lotSelectionPrefab == null) Debug.LogError("Lot prefab not set in package selection object " + gameObject.name);
    }

    public void SetSelectionPanelReference(LevelSelection ls)
    {
        _levelSelectionPanel = ls;
    }

    /// <summary>
    /// 
    /// Sets the GameMode that this button will open. 
    /// 
    /// </summary>
    /// <param name="name"> (string) Package name. </param>
    public void SetPackageName(string name)
    {
        _packageName.text = name;
    } // SetPackageName

    /// <summary>
    /// Sets the colors of the sprites to the one provided
    /// </summary>
    /// <param name="color">(Color) color of the package</param>
    public void SetPackageColor(Color color)
    {
        Color bgColor = new Color(color.r, color.g, color.b, _background.color.a);
        _background.color = bgColor;
        _lineDetail.color = color;
    }

    public void SetLots(LevelPackage package, VerticalLayoutGroup verticalLayout)
    {
        LotSelection lot;

        for (int i = 0; i < package._lotArray.Length; i++)
        {
            lot = Instantiate(_lotSelectionPrefab, verticalLayout.transform);
            // TODO: modify the value of the completed lots according to player data
            lot.SetLotData(package._packageName, package._lotArray[i]._lotName, package._packageColor);
            lot.SetButtonCall(_levelSelectionPanel);
        }
    }

}
