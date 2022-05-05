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

    /// <summary> is the init of the flow from back? </summary>
    public bool backIsInit()
    {
        return (_back == null) ? false : (_back.IsBall()) ? true : _back.backIsInit();
    }

    /// <summary> is the init of the flow from init? </summary>
    public bool forwardIsInit()
    {
        return (_next == null) ? false : (_next.IsBall()) ? true : _next.forwardIsInit();
    }

    /// <summary> how muchs tiles have that tiles back to him </summary>
    public int TrailBackward()
    {
        return (_back != null) ? 1 + _back.TrailBackward() : 0;
    }

    /// <summary> how muchs tiles have that tiles next to him </summary>
    public int TrailFordward()
    {
        return (_next != null) ? 1 + _next.TrailFordward() : 0;
    }

    /// <summary>
    /// 
    /// Delete all the trail from this tile
    /// 
    /// </summary>
    /// <param name="list"> (List<Tile>) a list from the tiles that have been removed </param>
    /// <param name="condition"> (bool) we are looking from back or next trail </param>
    public void TrailDeletion(ref List<Tile> list, bool condition)
    {
        list.Add(this);
        _trailNorth.SetActive(false);
        _trailSouth.SetActive(false);
        _trailEast.SetActive(false);
        _trailWest.SetActive(false);
        DesactivateBackGround();
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
                    _next.DesactivateBackGround();
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
                _back.DesactivateBackGround();
            }
            else
                _back.TrailDeletion(ref list, condition);
        }
        _next = null;
        _back = null;
    }

    /// <summary> make this Tile a empty  </summary>
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

    /// <summary> change and update the color of the tile </summary>
    public void SetColor(Color c)
    {
        _color = c;
        _trailEast.GetComponent<SpriteRenderer>().color = c;
        _trailWest.GetComponent<SpriteRenderer>().color = c;
        _trailNorth.GetComponent<SpriteRenderer>().color = c;
        _trailSouth.GetComponent<SpriteRenderer>().color = c;
        _ball.GetComponent<SpriteRenderer>().color = c;
    }

    public bool backgroundActive()
    {
        return  _gridBackground.GetComponent<SpriteRenderer>().color.r > 0.0 ||
                _gridBackground.GetComponent<SpriteRenderer>().color.g > 0.0 ||
                _gridBackground.GetComponent<SpriteRenderer>().color.b > 0.0;
    }

    /// <summary> Change the background decoration </summary>
    public void ChangeBackGroundColor(Color c)
    {
        c.a = 0.3f;
        _gridBackground.GetComponent<SpriteRenderer>().color = c;
    }

    /// <summary> Active the background decoration </summary>
    public void ActiveBackGround()
    {
        Color c = _color;
        ChangeBackGroundColor(c);
    }

    /// <summary> Desactivate the background decoration </summary>
    public void DesactivateBackGround()
    {
        Color c = Color.black;
        ChangeBackGroundColor(c);
    }

    /// <summary> Change the color from the walls </summary>
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

    /// <summary> Enables top wall </summary>
    public void EnableWallTop()
    {
        _wallTop.SetActive(true);
    }

    /// <summary> Enables bottom wall </summary>
    public void EnableWallBottom()
    {
        _wallBottom.SetActive(true);
    }

    /// <summary> Enables east wall </summary>
    public void EnableWallEast()
    {
        _wallEast.SetActive(true);
    }

    /// <summary> Enables west wall </summary>
    public void EnableWallWest()
    {
        _wallWest.SetActive(true);
    }

    /// <summary> Enables the hint sprite </summary>
    public void enableHint()
    {
        if (_color == Color.white) _hint.GetComponent<SpriteRenderer>().color = Color.black;
        _hint.SetActive(true);
    }

    /// <summary> Enables the goal sprite </summary>
    public void EnableBall()
    {
        _ball.SetActive(true);
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

    /// <summary> Activate the trails that the tile needs </summary>
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

    /// <summary> disable all the trails </summary>
    public void DisableTrails()
    {
        _trailNorth.SetActive(false);
        _trailSouth.SetActive(false);
        _trailEast.SetActive(false);
        _trailWest.SetActive(false);
    }

    /// <summary> the tile has a connection? </summary>
    public bool hasConection()
    {
        return (_next != null || _back != null);
    }

    #region setter/getters
    // -----------------------------------------------
    // -----           setters/getters           -----
    // -----------------------------------------------

    /// <summary> Is the bottom wall active? </summary>
    public bool IsBottomWall()
    {
        return _wallBottom.active;
    }

    /// <summary> Is the rigth wall active? </summary>
    public bool IsRightWall()
    {
        return _wallEast.active;
    }

    /// <summary> Is the ball active? </summary>
    public bool IsBall()
    {
        return _ball.active;
    }

    /// <summary> Is one or more trails active? </summary>
    public bool IsTrail()
    {
        return _trailNorth.active || _trailSouth.active || _trailWest.active || _trailEast.active;
    }

    /// <summary> return the position in the table </summary>
    public Point GetPosition()
    {
        return _pos;
    }

    /// <summary> return the color </summary>
    public Color getColor()
    {
        return _color;
    }
    #endregion

    #endregion //methods
}
