using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [Header("Prefabs")]
    public LevelGroup _levelGroupPrefab = null;

    [Header("Game Object References")]
    public HorizontalLayoutGroup _levelLayout = null;
    public Text _lotName = null;

    // for easy deleting of objects without messing with scene objects
    private LevelGroup[] _levels;

    private Color _lotColor;
    private int _nLevels = 150;
    private const int LEVELS_PER_GROUP = 30;
    private string _packageName;
    private PlayerData.CompletedStatus[] _completedLevelsMarkers;

    void Start()
    {
        if (_levelGroupPrefab == null) Debug.LogError("Level group prefab not set in Level Selection object");
        if (_levelLayout == null) Debug.LogError("Layout reference not set in level selection object");
        if (_lotName == null) Debug.LogError("lot name object reference not set in level selection object");
    }

    public void SetLotData(Color c, string lotName, string packageName, PlayerData.CompletedStatus[] completedLevelsMarkers)
    {
        _lotColor = c;

        _lotName.text = lotName;
        _lotName.color = c;

        _packageName = packageName;

        _completedLevelsMarkers = completedLevelsMarkers;
    }

    public void CreateLevelButtons()
    {
        LevelGroup group;
        int nGroups = _nLevels / LEVELS_PER_GROUP;
        _levels = new LevelGroup[nGroups];

        for (int i = 0; i < nGroups; ++i)
        {
            group = Instantiate(_levelGroupPrefab, _levelLayout.transform);
            group.SetLevelSelection(this);
            group.SetButtonColor(_lotColor);
            group.SetButtonNumbers(1 + LEVELS_PER_GROUP * i, _completedLevelsMarkers);

            _levels[i] = group;
        }
    }

    public void DeleteLevelButtons()
    {
        int nGroups = _nLevels / LEVELS_PER_GROUP;

        for (int i = 0; i < nGroups; ++i)
        {
            if (_levels[i].gameObject != null)
                Destroy(_levels[i].gameObject);
        }

        _levels = null;
    }

    public void LoadSelectedLevel(int level) {
        // send info to game manager

        GameManager.GetInstance().SetLevel(level - 1);
        GameManager.GetInstance().SetPackage(_packageName);
        GameManager.GetInstance().SetLot(_lotName.text);

        GameManager.GetInstance().LoadPlayScene();
    }
}
