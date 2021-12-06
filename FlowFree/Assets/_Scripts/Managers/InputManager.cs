using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Class to process the Input and hand it over to other 
/// obejcts to handle it. 
/// 
/// </summary>
public class InputManager : MonoBehaviour
{
    private Vector2 _touchPos;                 //Last touch position

    public enum InputType { NONE, MOVEMENT };

    void Update()
    {
        // if we're in editor, use PC input
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            _touchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y); // save touch 
            GameManager.GetInstance().ReceiveInput(InputType.MOVEMENT, _touchPos);
        } // if
        if (Input.GetMouseButtonUp(0))
        {
            GameManager.GetInstance().ReceiveInput(InputType.NONE, _touchPos);
        }
#else
        // TODO: MULTI TOUCH SUPPORT
        if (Input.touchCount == 1) // user is touching the screen 
        {
            Touch touch = Input.GetTouch(0); // get the touch
            
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) //check for the first touch
            {
                _touchPos = touch.position;
                GameManager.GetInstance().ReceiveInput(InputType.MOVEMENT, _touchPos);
            } // if
            else (touch.phase == TouchPhase.Ended) {
                GameManager.GetInstance().ReceiveInput(InputType.NONE, _touchPos);
            }
        } // if
#endif
    } // Update
} // InputManager
