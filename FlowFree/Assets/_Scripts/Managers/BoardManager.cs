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
        //_tiles = new Tile[map.X, map.Y];
        //_hintArray = map.hintArray; _tilesHint = Mathf.CeilToInt(_hintArray.Length / 3.0f);

        // Calculate space available for board
        //CalculateSpace();

        // Set ball colors
        // ...

        // Get size in pixels of tile (using ice sprite as reference because it fills one tile completely) and resize
        //SpriteRenderer iceFloor = _tilePrefab.transform.GetChild(0).GetComponent<SpriteRenderer>();
        //Vector2 tam = CalculateSize(iceFloor);

        // Instantiate tiles ------------------------------------------------------------------ !!
        //for (int y = 0; y < map.Y; ++y)
        //{
        //    for (int x = 0; x < map.X; ++x)
        //    {
        //        _tiles[x, y] = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity, _board.transform);
        //        SetTile(map.tileInfoMatrix[x, y], _tiles[x, y]);
        //    }
        //}

        // Scale tiles and Board
        //Vector2 oScale = _tilePrefab.transform.localScale;
        //Vector2 nScale = GameManager.GetInstance().GetScaling().ResizeObjectScale(iceFloor.bounds.size * GameManager.GetInstance().GetScaling().TransformationFactor(), tam, oScale);
        //gameObject.transform.localScale = nScale;
        //_board.transform.localScale = nScale;

        // Relocate board
        //gameObject.transform.Translate(new Vector3((-(map.X - 1) / 2.0f) * factor, ((-(map.Y - 1) / 2.0f) * factor) - 2));
        //_board.transform.Translate(new Vector3((-(map.X - 1) / 2.0f) * factor, ((-(map.Y - 1) / 2.0f) * factor) - 1));
    }


    // ------------------ PRIVATE -------------------

    /// <summary>
    /// 
    /// Method to set the tile with it's information.
    /// 
    /// </summary>
    /// <param name="info"> (TileInfo) Info of the tile. </param>
    /// <param name="tile"> (Tile) Tile to set. </param>
    private void SetTile(TileInfo info, Tile tile)
    {
        // set walls
        WallType infoWalls;
        infoWalls.left = info.wallLeft;
        infoWalls.top = info.wallTop;
        if (info.wallLeft || info.wallTop) tile.EnableWalls(infoWalls);

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
