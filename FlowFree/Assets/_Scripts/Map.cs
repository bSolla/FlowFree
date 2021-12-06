using System;
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
    public Point pos;
    public bool s;
    public bool e;
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
    public Point[] empties;
    public Point[,] solutions;
    //public Point[,] initANDend;
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
    public bool wallDown = false,
                wallEast = false;
    public bool uroboros = false;
    public bool empty = false;
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
    public int X, Y;
    private Point[,] solutions;

    public Point[,] getFlowSolution()
    {
        return solutions;
    }

    public int flowNumber()
    {
        return solutions.GetLength(0);
    }
    public Map(int c, int r, Wall[] w, Point[] e, Point[,] s/*, Point[,] iAe*/)
    {
        // make tile matrix
        
        X = c; Y = r;
        tileInfoMatrix = new TileInfo[X, Y]; //one extra col & row to draw the bottom and right walls of the map
        for (int col = 0; col < c; ++col)
        {
            for (int row = 0; row < r; ++row)
            {
                tileInfoMatrix[col, row] = new TileInfo();
            } // for
        } // for


        // set special points like ball start and finish
        /*
        for(int row = 0; row < iAe.GetLength(1); row++) 
        {
            tileInfoMatrix[iAe[0, row].x, iAe[0, row].y].uroboros = true;
            tileInfoMatrix[iAe[1, row].x, iAe[1, row].y].uroboros = true;
        }
        */
        // hint info
        solutions = s;
        //------------------------------------------------DEBUG------------------------------------------------
        int flowed = 0;
        Point sol;

        Color[] colores = new Color[12];
        colores[0] = Color.red;
        colores[1] = Color.green;
        colores[2] = Color.blue;
        colores[3] = Color.yellow;
        colores[4] = Color.cyan;
        colores[5] = Color.gray;
        colores[6] = Color.red;
        colores[7] = Color.green;
        colores[8] = Color.blue;
        colores[9] = Color.yellow;
        colores[10] = Color.cyan;
        colores[11] = Color.gray;
        for (int flowNumber = 0; flowNumber < solutions.GetLength(0); flowNumber++)
        {
            tileInfoMatrix[solutions[flowNumber, flowed].x, solutions[flowNumber, flowed].y].uroboros = true;
            while (solutions[flowNumber, flowed + 1].x != -1) {
                sol = solutions[flowNumber, flowed];
                tileInfoMatrix[sol.x, sol.y].next.x = solutions[flowNumber, flowed + 1].x;
                tileInfoMatrix[sol.x, sol.y].next.y = solutions[flowNumber, flowed + 1].y;
                tileInfoMatrix[sol.x, sol.y].ballColor = colores[flowNumber];
                flowed++;

            }
            tileInfoMatrix[solutions[flowNumber, flowed].x, solutions[flowNumber, flowed].y].uroboros = true;
            flowed = 0;
        }
        //------------------------------------------------DEBUG------------------------------------------------
        // wall info
        foreach (Wall wall in w)
        {
            tileInfoMatrix[wall.pos.x, wall.pos.y].wallDown = wall.s;
            tileInfoMatrix[wall.pos.x, wall.pos.y].wallEast = wall.e;
        }
        // other info (bridges and other stuff)
        foreach (Point empty in e) tileInfoMatrix[empty.x, empty.y].empty = true; 
    }
    /// <summary>
    /// Reads a json level file and creates an instance of Map with a filled TileInfo matrix
    /// </summary>
    /// <param name="path">Path to the json file</param>
    /// <returns>An completed instance of the Map class</returns>
    public static Map FromLot(LevelLot path, int level)
    {
        LotMap lotMap = ParseLot(path, level);

        Map map = new Map(lotMap.width, lotMap.height, lotMap.walls, lotMap.empties, lotMap.solutions/*, lotMap.initANDend*/);

        return map;
    } // FromJson

    private static LotMap ParseLot(LevelLot path, int level)
    {
        // parsing ..................
        // adapt from Reader
        string[] lines = path._level.text.Split(Environment.NewLine.ToCharArray());
        String[] levelInfo = lines[level].Split(';');

        String[] basicInfo = levelInfo[0].Split(',');
        LotMap readenMap = new LotMap();

        //Nos saltamos el nivel dentro del paquete y el 0 reservado
        int jump = 3;
        String[] posInfo;
        if (basicInfo[0].Contains(":"))
        {
            posInfo = basicInfo[0].Split(':', '+', 'B');
            readenMap.width = int.Parse(posInfo[0]);
            readenMap.height = int.Parse(posInfo[1]);
            jump = 5;
        }
        else readenMap.width = readenMap.height = int.Parse(basicInfo[0]);

        readenMap.solutions = new Point[levelInfo.Length - 1, readenMap.width * readenMap.height];
        //readenMap.initANDend = new Point[levelInfo.Length - 1, 2];
        //colocamos muros y huecos
        int fil, col;
        string[] muroInfo;
        for (int i = 4; i < basicInfo.Length; i++)
        {
            if (basicInfo[i] != "")
            {
                posInfo = basicInfo[i].Split(':');
                //Muros
                if (basicInfo[i].Contains("|"))
                {
                    readenMap.walls = new Wall[posInfo.Length];
                    readenMap.empties = new Point[0];
                    for (int z = 0; z < posInfo.Length; z++)
                    {
                        muroInfo = posInfo[z].Split('|');
                        fil = int.Parse(muroInfo[0]) / readenMap.width;
                        col = int.Parse(muroInfo[0]) - (readenMap.width * fil);
                        int first = int.Parse(muroInfo[0]), second = int.Parse(muroInfo[1]);
                        readenMap.walls[z].pos.x = col;
                        readenMap.walls[z].pos.y = fil;
                        readenMap.walls[z].s = first < second && first + 1 != second;
                        readenMap.walls[z].e = first + 1 == second;
                    }
                }
                //Celdas huecas
                else if (basicInfo[i].Contains(":"))
                {
                    readenMap.walls = new Wall[0];
                    readenMap.empties = new Point[posInfo.Length];
                    for (int z = 0; z < posInfo.Length; z++)
                    {
                        fil = int.Parse(posInfo[z]) / readenMap.width;
                        col = int.Parse(posInfo[z]) - (readenMap.width * fil);
                        readenMap.empties[z].x = col;
                        readenMap.empties[z].y = fil;
                    }
                }
            }
        }


        for (int i = 1; i < levelInfo.Length; i++)
        {
            posInfo = levelInfo[i].Split(',');

            fil = int.Parse(posInfo[0]) / readenMap.width;
            col = int.Parse(posInfo[0]) - (readenMap.width * fil);
            //readenMap.initANDend[i - 1, 0].x = col;
            //readenMap.initANDend[i - 1, 0].y = fil;

            for (int z = 0; z < posInfo.Length; z++)
            {
                fil = int.Parse(posInfo[z]) / readenMap.width;
                col = int.Parse(posInfo[z]) - (readenMap.width * fil);
                readenMap.solutions[i - 1, z].x = col;
                readenMap.solutions[i - 1, z].y = fil;
            }
            readenMap.solutions[i - 1, posInfo.Length].x = -1;
            readenMap.solutions[i - 1, posInfo.Length].y = -1;
            //readenMap.initANDend[i - 1, 1].x = col;
            //readenMap.initANDend[i - 1, 1].y = fil;
        }

        return readenMap;
    }
}
