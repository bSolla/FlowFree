using System;

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

//CLASS ---------------------------------------
public class Map
{
    public int width;
    public int height;
    public Wall[] walls;
    public Point[] empties;
    public Point[,] solutions;
    public static Map FromLot(LevelLot path, int level)
    {
        // parsing ..................
        // adapt from Reader
        string[] lines = path._lotLevel.text.Split(Environment.NewLine.ToCharArray());
        String[] levelInfo = lines[level*2].Split(';');

        String[] basicInfo = levelInfo[0].Split(',');
        Map readenMap = new Map();

        //Nos saltamos el nivel dentro del paquete y el 0 reservado
        //int jump = 3;
        String[] posInfo;
        if (basicInfo[0].Contains(":"))
        {
            posInfo = basicInfo[0].Split(':', '+', 'B');
            readenMap.width = int.Parse(posInfo[0]);
            readenMap.height = int.Parse(posInfo[1]);
            //jump = 5;
        }
        else readenMap.width = readenMap.height = int.Parse(basicInfo[0]);

        readenMap.solutions = new Point[levelInfo.Length - 1, readenMap.width * readenMap.height];

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
                        readenMap.walls[z].pos.y = Math.Abs(fil - (readenMap.height - 1));
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
                        readenMap.empties[z].y = Math.Abs(fil - (readenMap.height - 1));
                    }
                }
            }
        }
        if (4 >= basicInfo.Length)
        {
            readenMap.walls = new Wall[0];
            readenMap.empties = new Point[0];
        }

        for (int i = 1; i < levelInfo.Length; i++)
        {
            posInfo = levelInfo[i].Split(',');

            fil = int.Parse(posInfo[0]) / readenMap.width;
            col = int.Parse(posInfo[0]) - (readenMap.width * fil);

            for (int z = 0; z < posInfo.Length; z++)
            {
                fil = int.Parse(posInfo[z]) / readenMap.width;
                col = int.Parse(posInfo[z]) - (readenMap.width * fil);
                readenMap.solutions[i - 1, z].x = col;
                readenMap.solutions[i - 1, z].y = Math.Abs(fil - (readenMap.height - 1));
            }
            readenMap.solutions[i - 1, posInfo.Length].x = -1;
            readenMap.solutions[i - 1, posInfo.Length].y = -1;
        }

        return readenMap;
    }
}