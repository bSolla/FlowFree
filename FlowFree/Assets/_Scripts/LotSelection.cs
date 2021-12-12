using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotSelection : MonoBehaviour
{
    public Text _lotName = null;
    public Text _completedLots = null;

    private void Start()
    {
        if (_lotName == null) Debug.LogError("Lot name text reference not set in lot object " + gameObject.name);
        if (_completedLots == null) Debug.LogError("Completed lots text reference not set in lot object " + gameObject.name);
    }
}
