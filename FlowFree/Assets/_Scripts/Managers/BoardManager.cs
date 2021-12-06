using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Configuration")]
    public int _sideMargin;                     // Space to the sides
    public int _topMargin;                      // Space to the top
    public GameObject _board;                   // Board gameObject
    public Tile _tilePrefab;                    // Tile prefab to instantiate

    // Calculate space remaining for the board
    private float _topPanel;                    // Top panel in canvas
    private float _bottomPanel;                 // Bottom panel in canvas

    private Tile[,] _tiles;                     // Map
    private Tile _lastTile = null;

    private LevelManager _levelManager;         // LevelManager

    // Space that the board will take
    private Vector2 _resolution;                // Space for the board


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
    /// Sets the map, scaling everything to fit in the 
    /// space left by the top and bottom panel.
    /// 
    /// </summary>
    /// <param name="map"> (Map) Map to read the data. </param>
    public void SetMap(Map map)
    {
        // Init board sizes and variables
        _tiles = new Tile[map.X, map.Y];
        //_hintArray = map.hintArray; _tilesHint = Mathf.CeilToInt(_hintArray.Length / 3.0f);

        // Calculate space available for board
        CalculateSpace();


        for (int x = 0; x < map.X; x++)
        {
            for (int y = 0; y < map.Y; y++)
            {
                _tiles[x, y] = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity, _board.transform);
                SetTile(map.tileInfoMatrix[x, y], _tiles[x, y], x, y);
            }
        }


        //-----------------------------------------DEBUG-----------------------------------------
        Point lastPos = new Point(); 
        int flowed = 0;
        Point sol, lastNext;
        Point[,] solutions = map.getFlowSolution();
        for (int flowNumber = 0; flowNumber < solutions.GetLength(0); flowNumber++)
        {
            sol = solutions[flowNumber, flowed];
            if      (sol.x + 1 == solutions[flowNumber, flowed + 1].x) _tiles[sol.x, sol.y].EnableTrail(TrailType.EAST);
            else if (sol.x - 1 == solutions[flowNumber, flowed + 1].x) _tiles[sol.x, sol.y].EnableTrail(TrailType.WEST);
            else if (sol.y - 1 == solutions[flowNumber, flowed + 1].y) _tiles[sol.x, sol.y].EnableTrail(TrailType.SOUTH);
            else if (sol.y + 1 == solutions[flowNumber, flowed + 1].y) _tiles[sol.x, sol.y].EnableTrail(TrailType.NORTH);
            lastNext = sol;
            flowed++;
            while (solutions[flowNumber, flowed + 1].x != -1)
            {
                sol = solutions[flowNumber, flowed];
                if      (sol.x + 1 == solutions[flowNumber, flowed + 1].x) _tiles[sol.x, sol.y].EnableTrail(TrailType.EAST);
                else if (sol.x - 1 == solutions[flowNumber, flowed + 1].x) _tiles[sol.x, sol.y].EnableTrail(TrailType.WEST);
                else if (sol.y - 1 == solutions[flowNumber, flowed + 1].y) _tiles[sol.x, sol.y].EnableTrail(TrailType.SOUTH);
                else if (sol.y + 1 == solutions[flowNumber, flowed + 1].y) _tiles[sol.x, sol.y].EnableTrail(TrailType.NORTH);

                if      (lastNext.x - 1 == sol.x) _tiles[sol.x, sol.y].EnableTrail(TrailType.EAST);
                else if (lastNext.x + 1 == sol.x) _tiles[sol.x, sol.y].EnableTrail(TrailType.WEST);
                else if (lastNext.y + 1 == sol.y) _tiles[sol.x, sol.y].EnableTrail(TrailType.SOUTH);
                else if (lastNext.y - 1 == sol.y) _tiles[sol.x, sol.y].EnableTrail(TrailType.NORTH);
                flowed++;
                lastNext = sol;
            }
            sol = solutions[flowNumber, flowed];
            if      (lastNext.x - 1 == sol.x) _tiles[sol.x, sol.y].EnableTrail(TrailType.EAST);
            else if (lastNext.x + 1 == sol.x) _tiles[sol.x, sol.y].EnableTrail(TrailType.WEST);
            else if (lastNext.y + 1 == sol.y) _tiles[sol.x, sol.y].EnableTrail(TrailType.SOUTH);
            else if (lastNext.y - 1 == sol.y) _tiles[sol.x, sol.y].EnableTrail(TrailType.NORTH);
            flowed = 0;
        }

        //for (int flow = 0; flow < map.flowNumber(); flow++)
        //{
        //    Point[,] solutions = map.getFlowSolution();
        //    int x = solutions[flow, 0].x, y =solutions[flow, 0].y,
        //        xNext = map.tileInfoMatrix[x, y].next.x, yNext = map.tileInfoMatrix[x, y].next.y,
        //        xLast = 0, yLast = 0;
        //    lastPos = map.tileInfoMatrix[x, y].next;
        //    if      (x + 1 == xNext) _tiles[x, y].EnableTrail(TrailType.EAST);
        //    else if (x - 1 == xNext) _tiles[x, y].EnableTrail(TrailType.WEST);
        //    else if (y + 1 == yNext) _tiles[x, y].EnableTrail(TrailType.SOUTH);
        //    else if (y - 1 == yNext) _tiles[x, y].EnableTrail(TrailType.NORTH);
        //    for (int tl = 1; tl < solutions.GetLength(1); tl++)
        //    {
        //        y = solutions[flow, tl].y;
        //        x = solutions[flow, tl].x;

        //        xLast = lastPos.x;
        //        yLast = lastPos.y;
        //        if (xLast + 1 == x) _tiles[x, y].EnableTrail(TrailType.EAST);
        //        else if (xLast - 1 == x) _tiles[x, y].EnableTrail(TrailType.WEST);
        //        else if (yLast + 1 == y) _tiles[x, y].EnableTrail(TrailType.SOUTH);
        //        else if (yLast - 1 == y) _tiles[x, y].EnableTrail(TrailType.NORTH);

        //        xNext = map.tileInfoMatrix[x, y].next.x;
        //        yNext = map.tileInfoMatrix[x, y].next.y;
        //        lastPos = map.tileInfoMatrix[x, y].next;
        //        if (x + 1 == xNext) _tiles[x, y].EnableTrail(TrailType.EAST);
        //        else if (x - 1 == xNext) _tiles[x, y].EnableTrail(TrailType.WEST);
        //        else if (y + 1 == yNext) _tiles[x, y].EnableTrail(TrailType.SOUTH);
        //        else if (y - 1 == yNext) _tiles[x, y].EnableTrail(TrailType.NORTH);

        //        lastPos = map.tileInfoMatrix[x, y].next;
        //    }
        //}
        //-----------------------------------------DEBUG-----------------------------------------


        // Scale tiles and Board
        //Vector2 oScale = _tilePrefab.transform.localScale;
        //Vector2 nScale = GameManager.GetInstance().GetScaling().ResizeObjectScale(iceFloor.bounds.size * GameManager.GetInstance().GetScaling().TransformationFactor(), tam, oScale);
        //gameObject.transform.localScale = nScale;
        //_board.transform.localScale = nScale;

        // Relocate board
        //gameObject.transform.Translate(new Vector3((-(map.X - 1) / 2.0f) * factor, ((-(map.Y - 1) / 2.0f) * factor) - 2));
        //_board.transform.Translate(new Vector3((-(map.X - 1) / 2.0f) * factor, ((-(map.Y - 1) / 2.0f) * factor) - 1));
    }



    /// <summary>
    /// 
    /// Receives the input and processes it.
    /// 
    /// </summary>
    /// <param name="it"> (InputType) Type of the input. </param>
    public void ReceiveInput(InputManager.InputType it, Vector2 pos)
    {
        if (it == InputManager.InputType.NONE)
        {
            _lastTile = null;
        }
        if (it == InputManager.InputType.MOVEMENT)
        {
            if (_lastTile == null)
            {
                // delete all forward flows 
            }

            //TODO: DRAW FLOW
            foreach (Tile tile in _tiles)
            {
                if (tile.IsPointed(pos) && tile != _lastTile)
                {
                    //...
                    // check directions

                    // if (tile.isFlow)  --------------- DELETION OF FLOWS
                    //      tile.deleteNextFlow()
                    //                  -> if last tile in flow is an end and connected,
                    //                     do flowCount--

                    // else if (right)          -------- MOVEMENT AND LINK
                    //      if(last != null)
                    //          last.trailR.Active()
                    //      tile.trailL.Active()
                    //      tile.connect(_lastTile)

                    // left, down, up...

                    // if (flowEnd)
                    //      flowCount++

                    _lastTile = tile;
                    break;
                }
            }
        } // if
    } // ReceiveInput

    // ------------------ PRIVATE -------------------

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
        infoWalls.left = info.wallEast;
        infoWalls.top = info.wallDown;
        if (info.wallEast || info.wallDown) tile.EnableWalls(infoWalls);
        tile.gameObject.SetActive(!info.empty);
        tile.SetColor(info.ballColor);
        if(info.uroboros)tile.EnableBall();

        
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
        Vector2 resolutionTemp = _resolution * GameManager.GetInstance().GetScaling().TransformationFactor();

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
        _topPanel = GameManager.GetInstance().GetTopPanelHeight() * GameManager.GetInstance().GetCanvas().scaleFactor;
        _bottomPanel = GameManager.GetInstance().GetBottomPanelHeight() * GameManager.GetInstance().GetCanvas().scaleFactor;

        Vector2 actRes = GameManager.GetInstance().GetScaling().CurrentResolution();

        // Calculates the available space in the current resolution 
        float dispY = (actRes.y - (_bottomPanel + _topPanel)) - (2 * GameManager.GetInstance().GetScaling().ResizeY(_topMargin));
        float dipsX = actRes.x - (2 * GameManager.GetInstance().GetScaling().ResizeX(_sideMargin));

        // Creates the available screen space for the game in pixels
        _resolution = new Vector2(dipsX, dispY);

        // Change resolution to Unity units to use it for positions
        _resolution /= GameManager.GetInstance().GetScaling().TransformationFactor();
    } // CalculateSpace
}
