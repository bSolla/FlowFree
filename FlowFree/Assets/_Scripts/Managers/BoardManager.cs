using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

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
        // _tiles = new Tile[map.X, map.Y];
        //_hintArray = map.hintArray; _tilesHint = Mathf.CeilToInt(_hintArray.Length / 3.0f);

        // Calculate space available for board
        // ...

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
}
