using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// CONTAINERS ---------------------------------
[System.Serializable]
public struct Point
{
    public int x;
    public int y;
}

[System.Serializable]
public struct PointDouble
{
    public double x;
    public double y;
}

[System.Serializable]
public struct Wall
{
    public Point o;
    public Point d;
}

[System.Serializable]
public struct Trap
{
    string d;

    Point v1, v2;
}

[System.Serializable]
public class LotMap
{
    // .....
    public int width;
    public int height;
    public Wall[] walls;
    public Color[] color;
}

//[System.Serializable]
//public class JSONmap
//{
//    public int c, r;
//    public Point s;
//    public Point f;
//    public Point[] h;
//    public Wall[] w;
//    public PointDouble[] i;
//    public Point[] e;
//    public object[] t;
//}

// used for board creation in play scene
public class TileInfo
{
    public bool wallTop = false, wallLeft = false;
    public bool ball = false;
    public Color ballColor = default;
    public Point next = default;
}


public class Map : MonoBehaviour
{
    /// <summary> 
    /// Matrix that stores all the information needed for the creation of tiles.
    /// Access with tileInfoMatrix[x,y]
    /// </summary>
    public TileInfo[,] tileInfoMatrix;

    public Map(/*lotMap*/)
    {
        // make tile matrix
        /*
         * X = c + 1; Y = r + 1;
        tileInfoMatrix = new TileInfo[X, Y]; //one extra col & row to draw the bottom and right walls of the map
        for (int col = 0; col <= c; ++col)
        {
            for (int row = 0; row <= r; ++row)
            {
                tileInfoMatrix[col, row] = new TileInfo();
            } // for
        } // for
         */

        // set special points like ball start and finish

        // hint info

        // wall info

        // other info (bridges and other stuff)
    }

    /// <summary>
    /// Reads a json level file and creates an instance of Map with a filled TileInfo matrix
    /// </summary>
    /// <param name="path">Path to the json file</param>
    /// <returns>An completed instance of the Map class</returns>
    public static Map FromLot(string path)
    {
        // LotMap lotMap = ParseLot();

        // Map map = new Map(info from lotMap)

        // return map;
        return null;
    } // FromJson

    private static LotMap ParseLot()
    {
        // parsing ..................
        // adapt from Reader
        return null;
    }
}
