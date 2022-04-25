using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [Header("Resolution Configuration")]
    public Canvas _cnv;                           // Canvas of the scene
    public Camera _cam;                           // Camera of the scene

    [Header("Configuration")]
    public RectTransform _topPanel;              // Top panel of the canvas
    public RectTransform _bottomPanel;           // Bottom panel of the canvas
    public int _sideMargin;                     // Space to the sides
    public int _topMargin;                      // Space to the top
    public GameObject _board;                   // Board gameObject
    public Tile _tilePrefab;                    // Tile prefab to instantiate
    public GameObject _cursor;
    public GameObject _lastileIndicator;

    // SCALING DATA
    private Vector2 _scalingReferenceResolution;  // Reference resolution for scaling
    private Scaling _scalator;                    // Scaling object

    private Colorway _themeNow;
    private int _hintsUsed = 0;
    private Point[,] _hints;
    private int _realSize;

    // Calculate space remaining for the board
    private float _topPanelSize;                    // Top panel in canvas
    private float _bottomPanelSize;                 // Bottom panel in canvas

    private Tile[,] _tiles;                     // Map
    private Tile _lastTile = null;
    
    struct Ghost
    {
        public List<Tile> _tileList;
        public Tile _back;
        public Tile _next;
        public Color _color;
        public Ghost(List<Tile> t, Tile b, Tile n, Color c)
        {
            _tileList = t;
            _back = b;
            _next = n;
            _color = c;
        }
    }
    private List<Ghost> _ghostTiles;
    private int _numberFlows;
    private int _flowCount;
    private List<Tile[]> _flowPoints;
    private int _numberMoves = 0;
    private bool _boardComplete = false;

    private LevelManager _levelManager;         // LevelManager

    // Space that the board will take
    private Vector2 _resolution;                // Space for the board

    private void Awake()
    {
        // Store canvas' scaling reference resolution
        _scalingReferenceResolution = _cnv.GetComponent<CanvasScaler>().referenceResolution;

        // Create scalator
        Vector2 res = new Vector2(Screen.width, Screen.height);
        _scalator = new Scaling(res, _scalingReferenceResolution, (int)_cam.orthographicSize);
    }

    // ----------------------------------------------
    // --------------- CUSTOM METHODS ---------------
    // ----------------------------------------------

    // ------------------- PUBLIC -------------------

    /// <summary>
    /// 
    /// Initiates the BoardManager. 
    /// 
    /// </summary>
    /// <param name="levelManager"> (LevelManager) Manager of the level. </param>
    public void Init(LevelManager levelManager)
    {
        _levelManager = levelManager;
    } // Init


    /// <summary>
    /// 
    /// Checks if a flow is completed by following it along
    /// 
    /// </summary>
    /// <param name="tile">(Tile) used for color comparison</param>
    /// <returns>(bool) true if the flow is complete</returns>
    public bool CompleteFlow(Tile tile)
    {
        bool completeFlow = false;
        foreach (Tile[] t in _flowPoints)
        {
            if (t[0].getColor() == tile.getColor())  // if the color is the same
                if (t[0].hasConection() && t[1].hasConection()) //and the flow is complete
                    completeFlow = true;
        }
        return completeFlow;
    }

    /// <summary>
    /// 
    /// Complete a flow for the player if he have hints
    /// 
    /// </summary>
    public void GiveAHint()
    {
        if(_hintsUsed < _hints.GetLength(0))
        {
            if(GameManager.GetInstance().GetPlayerData()._hints > 0)
            {
                int flowed = 1;
                int x, y;
                Vector2 fakePos;
                x = _hints[_hintsUsed, 0].x;
                y = _hints[_hintsUsed, 0].y;
                _tiles[x, y].enableHint();
                fakePos = new Vector2((x * _board.transform.localScale.x) + _board.transform.position.x, (y * _board.transform.localScale.y) + _board.transform.position.y);
                ProcessMovement(fakePos);
                while (_hints[_hintsUsed, flowed + 1].x != -1)
                {
                    x = _hints[_hintsUsed, flowed].x;
                    y = _hints[_hintsUsed, flowed].y;
                    fakePos = new Vector2((x * _board.transform.localScale.x) + _board.transform.position.x, (y * _board.transform.localScale.y) + _board.transform.position.y);
                    ProcessMovement(fakePos);
                    flowed++;
                }
                x = _hints[_hintsUsed, flowed].x;
                y = _hints[_hintsUsed, flowed].y;
                _tiles[x, y].enableHint();
                fakePos = new Vector2((x * _board.transform.localScale.x) + _board.transform.position.x, (y * _board.transform.localScale.y) + _board.transform.position.y);
                ProcessMovement(fakePos);
                _hintsUsed++;
                GameManager.GetInstance().DecreaseHints();
            } // if (hints > 0)
        }
    }

    /// <summary>
    /// 
    /// Sets the map, scaling everything to fit in the 
    /// space left by the top and bottom panel.
    /// 
    /// </summary>
    /// <param name="map"> (Map) Map to read the data. </param>
    public void SetMap(Map map)
    {
        _hintsUsed = 0;
        // Init board sizes and variables
        _tiles = new Tile[map._X, map._Y];
        _ghostTiles = new List<Ghost>();
        _numberFlows = map.GetFlowSolution().GetLength(0);
        _hints = map.GetFlowSolution();
        _flowPoints = new List<Tile[]>();
        // Calculate space available for board
        CalculateSpace();

        _realSize = _tiles.Length;

        // Get size in pixels of tile (using background sprite as reference because it fills one tile completely) and resize
        SpriteRenderer background = _tilePrefab.transform.GetChild(0).GetComponent<SpriteRenderer>();
        Vector2 tam = CalculateSize(background);

        // Instantiate tiles
        for (int x = 0; x < map._X; x++)
        {
            for (int y = 0; y < map._Y; y++)
            {
                _tiles[x, y] = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity, _board.transform);
                SetTile(map._tileInfoMatrix[x, y], _tiles[x, y], x, y);
                _tiles[x, y].setWallColor(GameManager.GetInstance().GetPackageColor());
            }
        }

        Point[,] solutions = map.GetFlowSolution();
        int flowed = 0;

        // Decorate the map
        for (int x = 0; x < map._X; x++)
        {
            for (int y = 0; y < map._Y; y++)
            {
                if (!_tiles[x, y].gameObject.activeSelf)
                {
                    if (x + 1 < _tiles.GetLength(0)) _tiles[x + 1, y].EnableWallEast();
                    if (x - 1 >= 0) _tiles[x - 1, y].EnableWallWest();
                    if (y + 1 < _tiles.GetLength(1)) _tiles[x, y + 1].EnableWallBottom();
                    if (y - 1 >= 0) _tiles[x, y - 1].EnableWallTop();
                }
            }
        }
        for (int flowNumber = 0; flowNumber < _numberFlows; flowNumber++)
        {
            Point sol1 = solutions[flowNumber, flowed];
            while (solutions[flowNumber, flowed + 1].x != -1)
            {
                flowed++;
            }
            Point sol2 = solutions[flowNumber, flowed];
            Tile[] auxT = { _tiles[sol1.x, sol1.y], _tiles[sol2.x, sol2.y] };
            _flowPoints.Add(auxT);
            flowed = 0;
        }

        // Scale tiles and Board
        Vector2 oScale = _tilePrefab.transform.localScale;
        Vector2 nScale = _scalator.ResizeObjectScale(background.bounds.size * _scalator.TransformationFactor(), tam, oScale);
        gameObject.transform.localScale = nScale;
        _board.transform.localScale = nScale;
        float factor = nScale.x / oScale.x;

        // Relocate board
        _board.transform.Translate(new Vector3((-(map._X - 1f) / 2.0f) * factor, ((-(map._Y - 1.5f) / 2.0f) * factor))); 
    }


    /// <summary>
    /// 
    /// Cleans the board to use it again. 
    /// 
    /// </summary>
    public void EmptyBoard()
    {
        DestroyImmediate(_board);
        _board = new GameObject("Board");

        gameObject.transform.position = new Vector3(0, 0, 0);
        _board.transform.position = new Vector3(0, 0, 0);
    } // EmptyBoard


    /// <summary>
    /// 
    /// Receives the input and processes it.
    /// 
    /// </summary>
    /// <param name="it"> (InputType) Type of the input. </param>
    public void ReceiveInput(InputManager.InputType it, Vector2 pos)
    {
            if (_themeNow != GameManager.GetInstance().GetTheme())
            {
                UpdateColors();
                _themeNow = GameManager.GetInstance().GetTheme();
            }

            if (it == InputManager.InputType.NONE)
                ProcessEndOfTouch();
            else if (it == InputManager.InputType.MOVEMENT)
                ProcessMovement(pos);
    } // ReceiveInput

    /// <summary>
    /// 
    /// Returns the total number of flows in this board
    /// 
    /// </summary>
    /// <returns>(int) _numberFlows</returns>
    public int GetTotalFlows()
    {
        return _numberFlows;
    }

    // ------------------ PRIVATE -------------------
    /// <summary>
    /// 
    /// Do the necesary logic for finish the touch cicle
    /// 
    /// </summary>
    private void ProcessEndOfTouch()
    {
        if (_lastTile != null)
        {
            _numberMoves++;
            _levelManager.UpdateInfoUI(null, _numberMoves.ToString(), null, null);
            _lastileIndicator.GetComponent<SpriteRenderer>().color = (_lastTile.getColor() != Color.black) ? _lastTile.getColor() : new Color(0, 0, 0, 0);
            _lastileIndicator.gameObject.SetActive(true);
            _lastileIndicator.transform.localScale = _board.transform.localScale;
            _lastileIndicator.transform.position = new Vector2((_lastTile.GetPosition().x * _board.transform.localScale.x) + _board.transform.position.x, (_lastTile.GetPosition().y * _board.transform.localScale.y) + _board.transform.position.y);
            Tile b = _lastTile;
            while (b != null)
            {
                b.ActiveBackGround();
                b = b._back;
            }
        }
        // level complete
        if (_flowCount == _numberFlows && !_boardComplete)
        {
            _boardComplete = true;
            _levelManager.SetPause(true);
            _levelManager.ShowEndPanel((_numberMoves == _numberFlows), _numberMoves);
        }
        else _boardComplete = false;
        _lastTile = null;
        _cursor.SetActive(false);
    }
    
    /// <summary>
    /// 
    /// Update the color of all tiles by the new theme selected
    /// 
    /// </summary>
    private void UpdateColors()
    {
        Colorway theme = GameManager.GetInstance().GetTheme();
        int flowNumber = 0;
        Tile n;

        foreach (Tile[] t in _flowPoints)
        {
            bool aux = false;
            Color auxColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            foreach (Tile b in t)
            {
                b.SetColor((flowNumber < theme._arrayColors.Length) ? theme._arrayColors[flowNumber] : auxColor);
                n = b._next;
                while (n != null)
                {
                    n.SetColor((flowNumber < theme._arrayColors.Length) ? theme._arrayColors[flowNumber] : auxColor);
                    n = n._next;
                }
                if (aux)
                    flowNumber++;
                else
                    aux = !aux;
            }
        }
    }
    
    /// <summary>
    /// 
    /// calculate and update the value of the pipe
    /// 
    /// </summary>
    private void CalculatePipe()
    {
        float tilesWithFlow = 0;
        foreach (Tile[] t in _flowPoints)
        {
            if (t[0]._next != null)
            {
                tilesWithFlow += t[0].TrailFordward() + 1;
            }
            else if (t[0]._back != null)
            {
                tilesWithFlow += t[0].TrailBackward() + 1;
            }
            else if (t[1]._next != null)
            {
                tilesWithFlow += t[1].TrailFordward() + 1;
            }
            else if (t[1]._back != null)
            {
                tilesWithFlow += t[1].TrailBackward() + 1;
            }
        }
        _levelManager.UpdateInfoUI(null, null, null, (Mathf.RoundToInt((tilesWithFlow / _realSize) * 100)).ToString());
    }
    
    /// <summary>
    /// 
    /// Do the necesary logic when the player touch and hold the cursor on screen
    /// 
    /// </summary>
    /// <param name="pos">(Vector 2) cursor position</param>
    private void ProcessMovement(Vector2 pos)
    {
        //transform the cursor position to the position that will have in our table
        Vector2 realPos = new Vector2((pos.x - _board.transform.position.x) / _board.transform.localScale.x, (pos.y - _board.transform.position.y) / _board.transform.localScale.y);
        
        int x = Mathf.RoundToInt(realPos.x), y = Mathf.RoundToInt(realPos.y);
        
        if ((x >= 0 && x < _tiles.GetLength(0)) &&
            (y >= 0 && y < _tiles.GetLength(1)))
        {
            Tile tile = _tiles[x, y];

            _lastileIndicator.gameObject.SetActive(false);
            _cursor.SetActive(true);
            _cursor.transform.position = pos;
            _cursor.GetComponent<SpriteRenderer>().color = new Color(tile.getColor().r, tile.getColor().g, tile.getColor().b, 0.5f);
            
            // new click
            ProcessNewClick(tile);
            // ongoing click
            ProcessOngoingClick(tile, x, y);
            CalculatePipe();
        }
    }

    /// <summary>
    /// 
    /// Processes a new possible click and checks changes to board
    /// 
    /// </summary>
    /// <param name="tile">(Tile) current tile</param>
    private void ProcessNewClick(Tile tile)
    {
        if (_lastTile == null)
        {
            _ghostTiles.Clear();
            if (tile.IsBall() || tile.IsTrail())
            {
                //are you pressing other init?
                if (tile.IsBall())
                {
                    List<Tile> tileList = new List<Tile>(); // list of tiles deleted
                    foreach (Tile[] t in _flowPoints)
                    {
                        foreach (Tile b in t)
                        {
                            if (b.getColor() == tile.getColor())
                            {
                                if (b._next != null)
                                {
                                    if (CompleteFlow(b._next))
                                        _flowCount--;
                                    _levelManager.UpdateInfoUI(_flowCount.ToString(), null, null, null);

                                    b._next.TrailDeletion(ref tileList, true);
                                    b._next = null;
                                }
                                if (b._back != null)
                                {
                                    if (CompleteFlow(b._back))
                                    {
                                        _flowCount--;
                                        _levelManager.UpdateInfoUI(_flowCount.ToString(), null, null, null);
                                    }

                                    b._back.TrailDeletion(ref tileList, false);
                                    b._back = null;
                                }
                                b.CalculateTrails();
                            }
                        }
                    }
                }
                //delete all the flow
                if (tile._next != null)
                {
                    if (tile._next.IsBall())
                    {
                        _flowCount--;
                        _levelManager.UpdateInfoUI(_flowCount.ToString(), null, null, null);
                    }
                    List<Tile> tileList = new List<Tile>(); // list of tiles deleted
                    tile._next.TrailDeletion(ref tileList, true);
                    tile._next = null;
                    tile.CalculateTrails();
                }
                _lastTile = tile;
            }
            return;
        }
    }

    /// <summary>
    /// 
    /// Processes a possible ongoing click and checks for changes to board
    /// 
    /// </summary>
    /// <param name="tile">(Tile) current tile</param>
    /// <param name="x">(int) x coord</param>
    /// <param name="y">(int) y coord</param>
    private void ProcessOngoingClick(Tile tile, int x, int y)
    {
        if (tile != _lastTile)
        {
            //Are u trying to pass through THE CURSED HOLLOWS CORRIDOR, or a empty tile... yep... u cant go trough empties tiles
            if (!tile.gameObject.activeSelf) return;

            //The next tile is really next to the last?
            if (Mathf.Abs(x - _lastTile.GetPosition().x) +
                Mathf.Abs(y - _lastTile.GetPosition().y) > 1) return;
            //You can pass over here?
            if (_lastTile.IsRightWall() && (_lastTile.GetPosition().x < x)) return;
            if (_lastTile.IsBottomWall() && (_lastTile.GetPosition().y > y)) return;
            if (tile.IsRightWall() && (x < _lastTile.GetPosition().x)) return;
            if (tile.IsBottomWall() && (y > _lastTile.GetPosition().y)) return;

            if (tile.IsBall() && tile.getColor() != _lastTile.getColor()) return;
            if (_lastTile.IsBall() && _lastTile._back != null && _lastTile._back != tile) return;

            // new tile is empty tile
            if (tile.getColor() == Color.black)
            {
                _lastTile.SetNextTile(tile);
                tile.SetColor(_lastTile.getColor());
                _lastTile = tile;
                return;
            }
            // flow finish!
            //
            if (tile.IsBall() && tile.getColor() == _lastTile.getColor())
            {
                if (!tile.hasConection())
                {
                    _flowCount++;
                    _lastTile.SetNextTile(tile);
                    tile.SetColor(_lastTile.getColor());
                    _levelManager.UpdateInfoUI(_flowCount.ToString(), null, null, null);
                }
                else
                {   //if(tile._next._next != null)
                    DeleteTrails(tile, true, CompleteFlow(tile));
                    tile._next = null;
                    tile.CalculateTrails();
                    if (tile.hasConection())
                        _lastTile.emptyTrail();

                }
                _lastTile = tile;
                return;
            }
            // delete other flow
            if (tile.IsTrail())
            {
                bool completeFlow = CompleteFlow(tile);
                DeleteTrails(tile, tile.getColor() == _lastTile.getColor(), completeFlow);

                _lastTile.SetNextTile(tile);
                tile.SetColor(_lastTile.getColor());
                _lastTile = tile;
            }
        }
    }


    /// <summary>
    /// 
    /// Make flow with the data stored in our ghost struct
    /// 
    /// </summary>
    /// <param name="g">(Ghost) tiles that will be revive</param>
    private void ReviveTrails(Ghost g)
    {
        Tile aux = g._tileList[0];
        aux.SetColor(g._color);
        g._back.SetNextTile(aux);
        g._back.ActiveBackGround();
        aux.ActiveBackGround();
        aux._back = g._back;
        g._tileList.Remove(aux);

        foreach (Tile t in g._tileList)
        {
            aux.SetNextTile(t);
            aux.ActiveBackGround();
            aux = t;
            aux.SetColor(g._color);
            aux.ActiveBackGround();
            //tileList.Remove(aux);
        }
        if (g._next != null)
        {
            aux.SetNextTile(g._next);
            aux.ActiveBackGround();
            g._next.ActiveBackGround();
            _flowCount++;
            _levelManager.UpdateInfoUI(_flowCount.ToString(), null, null, null);
        }
    }
    /// <summary>
    /// 
    /// Delete all the flow
    /// 
    /// </summary>
    /// <param name="tile">(Tile) Tile that starts all the flow</param>
    /// <param name="sameColor">(bool) Are we and tile from the same color?</param>
    /// <param name="completeTrail">(bool) the flow that are we going to delete is a complete flow?</param>
    private void DeleteTrails(Tile tile, bool sameColor, bool completeTrail)
    {
        List<Tile> tileList = new List<Tile>(); // list of tiles deleted
        if (sameColor && !tile.IsBall()) 
            _lastTile = tile._back;
        bool f = tile.forwardIsInit(), b = tile.backIsInit();
        bool direction = (f && b) ? !(tile.TrailFordward() > tile.TrailBackward()) : !tile.forwardIsInit();
        Ghost gh;
        if (!direction)
        {
            gh =  new Ghost (tileList, tile._next, null, tile.getColor());
            if (tile._next != null)
            {
                tile._next._back = null;
                tile._next.CalculateTrails();
            }
        }
        else
        {
            gh = new Ghost(tileList, tile._back, null, tile.getColor());
            if (tile._back != null)
            {
                tile._back._next = null;
                tile._back.CalculateTrails();
            }
        }

        tile.TrailDeletion(ref tileList, direction);
        if (sameColor && !tile.IsBall())
            tileList.Remove(tile);
        if (completeTrail)
        {
            foreach (Tile[] t in _flowPoints)
            {
                if (t[0].getColor() == gh._color)  // if the color is the same
                    if (!t[0].hasConection()) //and the flow is complete
                        gh._next = t[0];
                else if(!t[1].hasConection())
                        gh._next = t[1];
            }
            _flowCount--;
            _levelManager.UpdateInfoUI(_flowCount.ToString(), null, null, null);
        }
        if (!sameColor)
        {
            _ghostTiles.Add(gh);
        }
        else    // check if the tile deletes is the begining of a ghost list
        {
            foreach (Tile t in tileList)
            {
                List<Ghost> toRemove = new List<Ghost>();
                foreach (Ghost g in _ghostTiles)
                {

                    if (g._tileList[0] == t)
                    {   // has found a ghost tile in the list of tiles deleted
                        ReviveTrails(g);   //Revive all the tiles in the list g
                        toRemove.Add(g);
                       // _ghostTiles.Remove(g);  // Remove the list of tiles ghost from the ghost list
                    }
                }
                foreach(Ghost rm in toRemove)
                {
                    _ghostTiles.Remove(rm); // Remove the list of tiles ghost from the ghost list
                }
            }
        }
    }

    /// <summary>
    /// 
    /// Method to set the tile with it's information.
    /// 
    /// </summary>
    /// <param name="info"> (TileInfo) Info of the tile. </param>
    /// <param name="tile"> (Tile) Tile to set. </param>
    private void SetTile(TileInfo info, Tile tile, int x, int y)
    {
        // set walls
        WallType infoWalls;
        infoWalls.right = info.wallEast || x >= _tiles.GetLength(0) - 1;
        infoWalls.bottom = info.wallDown || y <= 0;
        infoWalls.left = x <= 0;
        infoWalls.top = y >= _tiles.GetLength(1) - 1;
        tile.EnableWalls(infoWalls);
        if (info.empty)
        {
            _realSize--;
            tile.gameObject.SetActive(false);
        }
        if (info.uroboros)  // ball type
        {
            tile.SetColor(info.ballColor);
            tile.EnableBall();
        }

        tile.SetPosition(x, y);

        // keep setting and unsetting tile objects for example ball/flow starts or ends
        // InfoBall infoBall; 
        // if(ball) set ball; tile.EnableBall(infoBall)
        // ...
    } // SetTile


    /// <summary>
    /// 
    /// Calculates the size that one tile will have with the space available
    /// calculated previously. 
    /// 
    /// </summary>
    /// <param name="sprite"> (SpriteRenderer) Sprite to use as reference. </param>
    /// <returns> (Vector2) Size in pixels that will take te Tile. </returns>
    private Vector2 CalculateSize(SpriteRenderer sprite)
    {
        // Transform resolution to pixels 
        Vector2 resolutionTemp = _resolution * _scalator.TransformationFactor();

        // Calculate how many space requires a Tile breadthways and upwards related to that resolution
        float tileSizeX = resolutionTemp.x / _tiles.GetLength(0);
        float tileSizeY = resolutionTemp.y / _tiles.GetLength(1);

        // Choose the lower one to scale it fitting the shortest one
        if (tileSizeX < tileSizeY)
        {
            tileSizeY = sprite.bounds.size.y * tileSizeX / sprite.bounds.size.x;
        } // if
        else
        {
            tileSizeX = sprite.bounds.size.x * tileSizeY / sprite.bounds.size.y;
        } // else

        // Return result as a Vector
        return new Vector2(tileSizeX, tileSizeY);
    } // CalculateSize


    /// <summary>
    /// 
    /// Calculates the space available for the board, using the top and bottom panel 
    /// of the scene.
    /// 
    /// </summary>
    private void CalculateSpace()
    {
        // Calculates the space of the top and bottom panels in pixels
        _topPanelSize = _topPanel.rect.height * _cnv.scaleFactor;
        _bottomPanelSize = _bottomPanel.rect.height * _cnv.scaleFactor;

        Vector2 actRes = _scalator.CurrentResolution();

        // Calculates the available space in the current resolution 
        float dispY = (actRes.y - (_bottomPanelSize + _topPanelSize)) - (2 * _scalator.ResizeY(_topMargin));
        float dipsX = actRes.x - (2 * _scalator.ResizeX(_sideMargin));

        // Creates the available screen space for the game in pixels
        _resolution = new Vector2(dipsX, dispY);

        // Change resolution to Unity units to use it for positions
        _resolution /= _scalator.TransformationFactor();
    } // CalculateSpace
}
