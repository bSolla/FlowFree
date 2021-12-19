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

    private bool pressing = false;

    void Update()
    {
        // if we're in editor, use PC input
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            pressing = true;
        } // if
        if (Input.GetMouseButtonUp(0))
        {
            GameManager.GetInstance().ReceiveInput(InputType.NONE, _touchPos);
            pressing = false;
        }
        if (pressing)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _touchPos = new Vector2(worldPosition.x, worldPosition.y); // save touch 
            GameManager.GetInstance().ReceiveInput(InputType.MOVEMENT, _touchPos);
        }
#else

        if (Input.touchCount == 1) // user is touching the screen 
        {
            Touch touch = Input.GetTouch(0); // get the touch
            
            _touchPos = touch.position;
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved) //check for the first touch
            {
                pressing = true;
            } // if
            else if (touch.phase == TouchPhase.Ended) {
                GameManager.GetInstance().ReceiveInput(InputType.NONE, _touchPos);
                pressing = false;
            }
            if (pressing)
            {
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(_touchPos);
                _touchPos = new Vector2(worldPosition.x, worldPosition.y); // save touch 
                GameManager.GetInstance().ReceiveInput(InputType.MOVEMENT, _touchPos);
            }
        } // if
#endif
    } // Update
} // InputManager

