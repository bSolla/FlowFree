using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGroup : MonoBehaviour
{
    public LevelSelection _levelSelection;
    public Text _levelHeader;
    public Image[] _buttons = new Image[30];
    public Text[] _buttonText = new Text[30];

    public void SetLevelSelection(LevelSelection ls)
    {
        _levelSelection = ls;
    }

    public void SetButtonColor(Color color)
    {
        foreach (Image button in _buttons)
        {
            button.color = color;
        }
    }

    public void SetButtonNumbers(int from)
    {
        _levelHeader.text = from.ToString() + " - " + (from + 29).ToString();

        for (int i = 0; i < 30; ++i)
        {
            _buttonText[i].text = (from + i).ToString();
        }
    }

    public void PassButtonInfo(Text buttonText)
    {
        int level = int.Parse(buttonText.text);
        _levelSelection.LoadSelectedLevel(level);
    }
}
