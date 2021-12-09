using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
