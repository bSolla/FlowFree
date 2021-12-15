using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotSelection : MonoBehaviour
{
    public Text _lotName = null;
    public Text _completedLevelsText = null;
    public Button _buttonComponent = null;

    private LevelSelection _levelSelectionPanel;
    private string _packageName;
    private int[] _completedLevelsMarkers;

    private void Start()
    {
        if (_lotName == null) Debug.LogError("Lot name text reference not set in lot object " + gameObject.name);
        if (_completedLevelsText == null) Debug.LogError("Completed lots text reference not set in lot object " + gameObject.name);
        if (_buttonComponent == null) Debug.LogError("button component reference not set in lot object " + gameObject.name);
    }

    public void SetButtonCall(LevelSelection ls)
    {
        _levelSelectionPanel = ls;
        _buttonComponent.onClick.AddListener(ButtonTask);
    }

    public void SetLotData(string packageName, string lotName, Color lotColor) // TODO: add completed levels eventually
    {
        _packageName = packageName;
        _lotName.text = lotName;
        _lotName.color = lotColor;
        
        _completedLevelsMarkers = GameManager.GetInstance().GetPlayerData()._completedLevelsLot[lotName];
        int nCompleted = 0;
        for (int i = 0; i < _completedLevelsMarkers.Length; ++i)
        {
            if (_completedLevelsMarkers[i] != 0)
                nCompleted++;
        }
        _completedLevelsText.text = nCompleted.ToString() + "/" + _completedLevelsMarkers.Length.ToString();
    }

    void ButtonTask()
    {
        _levelSelectionPanel.gameObject.SetActive(true);
        _levelSelectionPanel.SetLotData(_lotName.color, _lotName.text, _packageName, _completedLevelsMarkers);
        _levelSelectionPanel.CreateLevelButtons();
    }
}
