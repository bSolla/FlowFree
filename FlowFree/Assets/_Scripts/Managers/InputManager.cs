using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 
/// Event class used to send input info to the level manager
/// 
/// </summary>
[System.Serializable]
public class InputEvent : UnityEvent<InputManager.InputType, Vector2>
{
}

/// <summary>
/// 
/// Class to process the Input and hand it over to other 
/// obejcts to handle it. 
/// 
/// </summary>
public class InputManager : MonoBehaviour
{
    [System.Serializable]
    public enum InputType { NONE, MOVEMENT };

    [SerializeField]
    public InputEvent _inputReceived = null;

    private Vector2 _touchPos;                 //Last touch position
    private bool _pressing = false;
    private bool _paused = false;

    void Update()
    {
        if (!_paused)
        {
            // if we're in editor, use PC input
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetMouseButtonDown(0))
            {
                _pressing = true;
            } // if
            if (Input.GetMouseButtonUp(0))
            {
                _inputReceived.Invoke(InputType.NONE, _touchPos);
                _pressing = false;
            }
            if (_pressing)
            {
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _touchPos = new Vector2(worldPosition.x, worldPosition.y); // save touch 
                _inputReceived.Invoke(InputType.MOVEMENT, _touchPos);
            }
#elif UNITY_ANDROID
            if (Input.touchCount == 1) // user is touching the screen 
            {
                Touch touch = Input.GetTouch(0); // get the touch

                _touchPos = touch.position;
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) //check for the first touch
                {
                    _pressing = true;
                } // if
                else if (touch.phase == TouchPhase.Ended)
                {
                    _inputReceived.Invoke(InputType.NONE, _touchPos);
                    _pressing = false;
                }
                if (_pressing)
                {
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(_touchPos);
                    _touchPos = new Vector2(worldPosition.x, worldPosition.y); // save touch 
                    _inputReceived.Invoke(InputType.MOVEMENT, _touchPos);
                }
            } // if
#endif
        }
    } // Update

    public void SetPause(bool isPaused)
    {
        _paused = isPaused;
    }
} // InputManager

