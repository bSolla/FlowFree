using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotSelection : MonoBehaviour
{
    public Text _lotName = null;
    public Text _completedLots = null;
    public Button _buttonComponent = null;

    private LevelSelection _levelSelectionPanel;
    private string _packageName;

    private void Start()
    {
        if (_lotName == null) Debug.LogError("Lot name text reference not set in lot object " + gameObject.name);
        if (_completedLots == null) Debug.LogError("Completed lots text reference not set in lot object " + gameObject.name);
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
    }

    void ButtonTask()
    {
        _levelSelectionPanel.gameObject.SetActive(true);
        _levelSelectionPanel.SetLotData(_lotName.color, _lotName.text, _packageName);
        _levelSelectionPanel.CreateLevelButtons();
    }
}
