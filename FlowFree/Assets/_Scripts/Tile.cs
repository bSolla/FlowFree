using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public struct WallType
{
    public bool bottom, right, top, left;
}

public enum TrailType { NORTH, SOUTH, EAST, WEST, START };

public class Tile : MonoBehaviour
{
    #region variables
    [SerializeField]
    [Tooltip("Child component that stores the grid background sprite")]
    private GameObject _gridBackground;
    [SerializeField]
    [Tooltip("Child component that stores the top wall sprite")]
    private GameObject _wallEast;
    [SerializeField]
    [Tooltip("Child component that stores the left wall sprite")]
    private GameObject _wallBottom;
    [SerializeField]
    [Tooltip("Child component that stores the top wall sprite")]
    private GameObject _wallWest;
    [SerializeField]
    [Tooltip("Child component that stores the left wall sprite")]
    private GameObject _wallTop;
    [SerializeField]
    [Tooltip("Child component that stores the north trail")]
    private GameObject _trailNorth;
    [SerializeField]
    [Tooltip("Child component that stores the east trail")]
    private GameObject _trailEast;
    [SerializeField]
    [Tooltip("Child component that stores the south trail")]
    private GameObject _trailSouth;
    [SerializeField]
    [Tooltip("Child component that stores the west trail")]
    private GameObject _trailWest;
    [SerializeField]
    [Tooltip("Child component that stores the hint")]
    private GameObject _hint;
    [SerializeField]
    [Tooltip("Child component that stores the ball")]
    private GameObject _ball;

    private Color _color = Color.black;
    public Tile _next;
    public Tile _back;
    private Point _pos;
    #endregion //variables

    #region methods
    private void Start()
    {
#if UNITY_EDITOR
        if (_gridBackground == null)
        {
            Debug.LogError("gridBackground is null. Can't start without the grid background sprite");
            gameObject.SetActive(false);
            return;
        }
        if (_wallBottom == null)
        {
            Debug.LogError("wallTop is null. Can't start without the wall top sprite");
            gameObject.SetActive(false);
            return;
        }
        if (_wallEast == null)
        {
            Debug.LogError("wallLeft is null. Can't start without the wall left sprite");
            gameObject.SetActive(false);
            return;
        }
        if (_ball == null)
        {
            Debug.LogError("ball is null. Can't start without the goal sprite");
            gameObject.SetActive(false);
            return;
        }
        if (_trailEast == null || _trailNorth == null || _trailSouth == null || _trailWest == null)
        {
            Debug.LogError("some of the trails are null. Can't start without the trail sprites");
            gameObject.SetActive(false);
            return;
        }
#endif
    }

    // -----------------------------------------------
    // ----- methods that turn components on/off -----
    // -----------------------------------------------
    public void SetPosition(int x, int y)
    {
        _pos.x = x;
        _pos.y = y;
    }
    public Point GetPosition()
    {
        return _pos;
    }

    /// <summary> Set the next tile </summary>
    public void SetNextTile(Tile t)
    {
        _next = t;
        if (_pos.x + 1 == t.GetPosition().x) EnableTrail(TrailType.EAST);
        else if (_pos.x - 1 == t.GetPosition().x) EnableTrail(TrailType.WEST);
        else if (_pos.y - 1 == t.GetPosition().y) EnableTrail(TrailType.SOUTH);
        else if (_pos.y + 1 == t.GetPosition().y) EnableTrail(TrailType.NORTH);

        t.SetBackTile(this);
    }

    /// <summary> Set the back tile </summary>
    public void SetBackTile(Tile t)
    {
        _back = t;
        if (t.GetPosition().x - 1 == _pos.x) EnableTrail(TrailType.EAST);
        else if (t.GetPosition().x + 1 == _pos.x) EnableTrail(TrailType.WEST);
        else if (t.GetPosition().y + 1 == _pos.y) EnableTrail(TrailType.SOUTH);
        else if (t.GetPosition().y - 1 == _pos.y) EnableTrail(TrailType.NORTH);
    }
    public bool backIsInit()
    {
        return (_back == null) ? false : (_back.IsBall()) ? true : _back.backIsInit();
    }
    public bool forwardIsInit()
    {
        return (_next == null) ? false : (_next.IsBall()) ? true : _next.forwardIsInit();
    }

    public int TrailBackward()
    {
        return (_back != null) ? 1 + _back.TrailBackward() : 0;
    }
    public int TrailFordward()
    {
        return (_next != null) ? 1 + _next.TrailFordward() : 0;
    }
    public void TrailDeletion(ref List<Tile> list, bool condition)
    {
        list.Add(this);
        _trailNorth.SetActive(false);
        _trailSouth.SetActive(false);
        _trailEast.SetActive(false);
        _trailWest.SetActive(false);
        if(!IsBall())
            SetColor(Color.black);


        if (condition)
        {
            if (_next != null)
            {
                if (_next.IsBall())
                {
                    _next.DisableTrails();
                    _next._back = null;
                }
                else
                    _next.TrailDeletion(ref list, condition);
            }
        }
        else if (_back != null)
        {
            if (_back.IsBall())
            {
                _back.DisableTrails();
                _back._next = null;
            }
            else
                _back.TrailDeletion(ref list, condition);
        }
        _next = null;
        _back = null;
    }

    public void emptyTrail()
    {
        _trailNorth.SetActive(false);
        _trailSouth.SetActive(false);
        _trailEast.SetActive(false);
        _trailWest.SetActive(false);
        SetColor(Color.black);
        _next = null;
        _back = null;
    }

    /// <summary> Enables the ice sprite </summary>
    public void EnableGridBackground()
    {
        _gridBackground.SetActive(true);
    }
    /// <summary> Disables the ice sprite </summary>
    public void DisableGridBackground()
    {
        _gridBackground.SetActive(false);
    }

    public void SetColor(Color c)
    {
        _color = c;
        _trailEast.GetComponent<SpriteRenderer>().color = c;
        _trailWest.GetComponent<SpriteRenderer>().color = c;
        _trailNorth.GetComponent<SpriteRenderer>().color = c;
        _trailSouth.GetComponent<SpriteRenderer>().color = c;
        //_hintEast.GetComponent<SpriteRenderer>().color = c;
        //_hintWest.GetComponent<SpriteRenderer>().color = c;
        //_hintNorth.GetComponent<SpriteRenderer>().color = c;
        //_hintSouth.GetComponent<SpriteRenderer>().color = c;

        _ball.GetComponent<SpriteRenderer>().color = c;
        c.a = 0.3f;
        _gridBackground.GetComponent<SpriteRenderer>().color = c;
    } // SetTrailColor

    public Color getColor()
    {
        return _color;
    }

    public void setWallColor(Color c)
    {
        _wallBottom.GetComponent<SpriteRenderer>().color = c;
        _wallTop.GetComponent<SpriteRenderer>().color = c;
        _wallEast.GetComponent<SpriteRenderer>().color = c;
        _wallWest.GetComponent<SpriteRenderer>().color = c;
    }

    /// <summary> Enables the given wall sprites </summary>
    public void EnableWalls(WallType walls)
    {
        if (walls.bottom) _wallBottom.SetActive(true);
        if (walls.right) _wallEast.SetActive(true);
        if (walls.top) _wallTop.SetActive(true);
        if (walls.left) _wallWest.SetActive(true);
    }
    public void EnableWallTop()
    {
        _wallTop.SetActive(true);
    }
    public void EnableWallBottom()
    {
        _wallBottom.SetActive(true);
    }
    public void EnableWallEast()
    {
        _wallEast.SetActive(true);
    }
    public void EnableWallWest()
    {
        _wallWest.SetActive(true);
    }
    /// <summary> Disables the given wall sprites </summary>
    public void DisableWalls(WallType walls)
    {
        if (walls.bottom) _wallBottom.SetActive(false);
        if (walls.right) _wallEast.SetActive(false);
        if (walls.top) _wallTop.SetActive(false);
        if (walls.left) _wallWest.SetActive(false);
    }
    public void enableHint()
    {
        _hint.SetActive(true);
    }

    /// <summary> Enables the goal sprite </summary>
    public void EnableBall()
    {
        _ball.SetActive(true);
    }
    /// <summary> Disables the goal sprite </summary>
    public void DisableGoal()
    {
        _ball.SetActive(false);
    }

    /// <summary> Enables the specified trail sprite </summary>
    public void EnableTrail(TrailType tt)
    {
        switch (tt)
        {
            case TrailType.NORTH:
                _trailNorth.SetActive(true);
                break;
            case TrailType.SOUTH:
                _trailSouth.SetActive(true);
                break;
            case TrailType.EAST:
                _trailEast.SetActive(true);
                break;
            case TrailType.WEST:
                _trailWest.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void CalculateTrails()
    {
        DisableTrails();
        if (_next != null)
        {
            if (_pos.x + 1 == _next.GetPosition().x) EnableTrail(TrailType.EAST);
            else if (_pos.x - 1 == _next.GetPosition().x) EnableTrail(TrailType.WEST);
            else if (_pos.y - 1 == _next.GetPosition().y) EnableTrail(TrailType.SOUTH);
            else if (_pos.y + 1 == _next.GetPosition().y) EnableTrail(TrailType.NORTH);
        }
        if (_back != null)
        {
            if (_back.GetPosition().x - 1 == _pos.x) EnableTrail(TrailType.EAST);
            else if (_back.GetPosition().x + 1 == _pos.x) EnableTrail(TrailType.WEST);
            else if (_back.GetPosition().y + 1 == _pos.y) EnableTrail(TrailType.SOUTH);
            else if (_back.GetPosition().y - 1 == _pos.y) EnableTrail(TrailType.NORTH);
        }
    }

    public void DisableTrails()
    {
        _trailNorth.SetActive(false);
        _trailSouth.SetActive(false);
        _trailEast.SetActive(false);
        _trailWest.SetActive(false);
    }



    /// <summary> Disables the specified trail sprite </summary>
    public void DisableTrail(TrailType tt)
    {
        switch (tt)
        {
            case TrailType.NORTH:
                _trailNorth.SetActive(false);
                break;
            case TrailType.SOUTH:
                _trailSouth.SetActive(false);
                break;
            case TrailType.EAST:
                _trailEast.SetActive(false);
                break;
            case TrailType.WEST:
                _trailWest.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void EnableHint(TrailType dir)
    {
        //switch (dir)
        //{
        //    case TrailType.NORTH:
        //        _hintNorth.SetActive(true);
        //        break;
        //    case TrailType.SOUTH:
        //        _hintSouth.SetActive(true);
        //        break;
        //    case TrailType.EAST:
        //        _hintEast.SetActive(true);
        //        break;
        //    case TrailType.WEST:
        //        _hintWest.SetActive(true);
        //        break;
        //    default:
        //        break;
        //}
    }


    public void DisableHint(TrailType dir)
    {
        //switch (dir)
        //{
        //    case TrailType.NORTH:
        //        _hintNorth.SetActive(false);
        //        break;
        //    case TrailType.SOUTH:
        //        _hintSouth.SetActive(false);
        //        break;
        //    case TrailType.EAST:
        //        _hintEast.SetActive(false);
        //        break;
        //    case TrailType.WEST:
        //        _hintWest.SetActive(false);
        //        break;
        //    default:
        //        break;
        //}
    }

    /// <summary>
    /// Checks if the pointer object is over the image
    /// </summary>
    /// <param name="pointerPosition">(Vector3) current pointer position</param>
    /// <returns>(bool) Wether or not the pointer is inside the image</returns>
    public bool IsPointed(Vector3 pointerPosition)
    {
        RectTransform rectTransform = gameObject.GetComponent<Image>().rectTransform;

        return rectTransform.rect.Contains(rectTransform.InverseTransformPoint(pointerPosition));
    }

    public bool hasConection()
    {
        return (_next != null || _back != null);
    }

    #region setter/getters
    // -----------------------------------------------
    // -----           setters/getters           -----
    // -----------------------------------------------
    public bool IsBottomWall()
    {
        return _wallBottom.active;
    }

    public bool IsRightWall()
    {
        return _wallEast.active;
    }

    public bool IsBall()
    {
        return _ball.active;
    }

    public bool IsGridBackground()
    {
        return _gridBackground.active;
    }

    public bool IsNorthTrail()
    {
        return _trailNorth.active;
    }

    public bool IsSouthTrail()
    {
        return _trailSouth.active;
    }

    public bool IsWestTrail()
    {
        return _trailWest.active;
    }

    public bool IsEastTrail()
    {
        return _trailEast.active;
    }
    public bool IsTrail()
    {
        return _trailNorth.active || _trailSouth.active || _trailWest.active || _trailEast.active;
    }

    #endregion

    #endregion //methods
}
