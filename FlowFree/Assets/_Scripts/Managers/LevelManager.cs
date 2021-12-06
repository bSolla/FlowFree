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

    private bool _paused = false;                 // Pause flag for Input control

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
        LevelPackage lp = GameManager.GetInstance().GetLevelPackage(2);
        int level = GameManager.GetInstance().GetLevel();
        Map map = Map.FromLot(lp._levels[2], level);

        //_boardManager.EmptyBoard();
        _boardManager.SetMap(map);
    }

    /// <summary>
    /// 
    /// Receive new Input and process it. 
    /// 
    /// </summary>
    /// <param name="it"> (InputType) Type of new input. </param>
    public void ReceiveInput(InputManager.InputType it, Vector2 pos)
    {
        if (!_paused)
        {
            _boardManager.ReceiveInput(it, pos);
        } // if
    } // ReceiveInput
}
