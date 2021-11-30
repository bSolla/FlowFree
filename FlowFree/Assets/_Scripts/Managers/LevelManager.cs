using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Tooltip("Board Manager object")]
    public BoardManager _boardManager;            // BoardManager instance
    public Canvas _canvas;                        // Canvas of the scene
    public Camera _camera;                        // Camera 
    public RectTransform _topPanel;               // Top panel of canvas
    public RectTransform _botPanel;               // Bottom panel of canvas


    // ----------------------------------------------
    // --------------- UNITY METHODS ----------------
    // ----------------------------------------------

    void Awake()
    {
        if (_boardManager == null)
        {
            Debug.LogError("Board Manager reference not set");
        } // if
        //if (_endPanel == null)
        //{
        //    Debug.LogError("End panel reference not set");
        //} // if
        else
        {
            _boardManager.Init(this);
        } // else
    } // Awake

    private void Start()
    {
        GameManager.GetInstance().SetCamera(_camera);
        GameManager.GetInstance().SetCanvas(_canvas);

        // set callbacks
        //_homePauseButton.onClick.AddListener(GameManager.GetInstance().ReturnToMenu);
        //_homeEndedButton.onClick.AddListener(GameManager.GetInstance().ReturnToMenu);
        //_finalHomeButton.onClick.AddListener(GameManager.GetInstance().ReturnToMenu);

        PlayLevel();
    } // Start


    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    /// <summary>
    /// 
    /// Sets all the scene to play the next level. 
    /// 
    /// </summary>
    public void PlayLevel()
    {
        // Set info
        // ... ad manager, pause, etc

        // Prepare board
        //LevelPackage lp = GameManager.GetInstance().GetLevelPackage();
        //int level = GameManager.GetInstance().GetLevel();

        //Map map = Map.FromLot(""/*lp.levels[level].ToString()*/);

        //_boardManager.EmptyBoard();
        _boardManager.SetMap(null/*created map*/);
    }
}
