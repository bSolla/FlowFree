using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGroup : MonoBehaviour
{
    LevelSelection _levelSelection;
    
    [Header("Game Object references")]
    public Text _levelHeader;
    public Image[] _buttons = new Image[30];
    public Text[] _buttonText = new Text[30];

    [Header("Resources")]
    public Sprite _completedSprite = null;
    public Sprite _perfectSprite = null;

    private void Start()
    {
        if (_completedSprite == null) Debug.LogError("missing completed sprite in level group");
        if (_perfectSprite == null) Debug.LogError("missing perfect sprite in level group");
    }

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

    public void SetButtonNumbers(int from, int[] _completedLevelsMarkers)
    {
        _levelHeader.text = from.ToString() + " - " + (from + 29).ToString();

        GameObject completedImage;
        for (int i = 0; i < 30; ++i) // each level group is a collection of 30 buttons
        {
            _buttonText[i].text = (from + i).ToString();
            if (_completedLevelsMarkers[from + i - 1] == 1) // completed
            {
                completedImage= Instantiate(new GameObject(), _buttons[i].transform);
                completedImage.AddComponent<Image>();
                completedImage.GetComponent<Image>().sprite = _completedSprite;
            }
            else if (_completedLevelsMarkers[from + i - 1] == 2) // perfect 
            {
                completedImage = Instantiate(new GameObject(), _buttons[i].transform);
                completedImage.AddComponent<Image>();
                completedImage.GetComponent<Image>().sprite = _perfectSprite;
            }
        }
    }

    public void PassButtonInfo(Text buttonText)
    {
        int level = int.Parse(buttonText.text);
        _levelSelection.LoadSelectedLevel(level);
    }
}
