using System;
using UnityEngine;

public class Reader : MonoBehaviour
{
    #region variables
    private string _path;
    [SerializeField]
    [Tooltip("level file name")]
    private string _levelTxt;
    [SerializeField]
    [Tooltip("level")]
    private int _level;
    private int _x, _y, _flows;
    #endregion //variables

    //esto es solo para debugear
    public enum Typito {normal, init, empty};
    struct flowFree
    {
        public int _color;
        public Typito _type;
        public bool n;
        public bool s;
        public bool w;
        public bool e;
        public void init()
        {
            _color = 0;
            _type = Typito.normal;
            n = false;
            s = false;
            w = false;
            e = false;
        }
    }

    #region methods
    void Start()
    {
        _path = Application.streamingAssetsPath + "/";
        ReadNewLevel();
    }


    void ReadNewLevel()
    {
        try
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(_path + _levelTxt);
            for (int i = 1; i < _level; i++) reader.ReadLine();
            String[] levelInfo = reader.ReadLine().Split(';');
            String[] basicInfo = levelInfo[0].Split(',');
            reader.Close();
            //Nos saltamos el nivel dentro del paquete y el 0 reservado
            int jump = 3;
            String[] posInfo;
            if (basicInfo[0].Contains(":"))
            {
                posInfo = basicInfo[0].Split(':', '+', 'B');
                _x = int.Parse(posInfo[0]);
                _y = int.Parse(posInfo[1]);
                jump = 5;
            }
            else _x = _y = int.Parse(basicInfo[0]);

            _flows = int.Parse(basicInfo[3]);

            flowFree[,] table = new flowFree[_x, _y];
            foreach (flowFree ff in table) ff.init();
            //colocamos muros y huecos
            int fil, col;
            string[] muroInfo;
            for (int i = 4; i < basicInfo.Length; i++)
            {
                if (basicInfo[i] != "")
                {
                    //Muros
                    if (basicInfo[i].Contains("|"))
                    {
                        posInfo = basicInfo[i].Split(':');
                        for (int z = 0; z < posInfo.Length; z++)
                        {
                            muroInfo = posInfo[z].Split('|');
                            fil = int.Parse(muroInfo[0]) / _x;
                            col = int.Parse(muroInfo[0]) - (_x * fil);
                            int first = int.Parse(muroInfo[0]), second = int.Parse(muroInfo[1]);
                            table[col, fil].n = first > second && first - 1 != second;
                            table[col, fil].s = first < second && first + 1 != second;
                            table[col, fil].w = first - 1 == second;
                            table[col, fil].e = first + 1 == second;
                        }
                    }
                    //Celdas huecas
                    else if (basicInfo[i].Contains(":"))
                    {
                        posInfo = basicInfo[i].Split(':');
                        for (int z = 0; z < posInfo.Length; z++)
                        {
                            fil = int.Parse(posInfo[z]) / _x;
                            col = int.Parse(posInfo[z]) - (_x * fil);
                            table[col, fil]._type = Typito.empty;
                        }
                    }
                }
            }
            for (int i = 1; i < levelInfo.Length; i++)
            {
                posInfo = levelInfo[i].Split(',');

                fil = int.Parse(posInfo[0]) / _x;
                col = int.Parse(posInfo[0]) - (_x * fil);
                table[col, fil]._type = Typito.init;
                table[col, fil]._color = i;

                for (int z = 1; z < posInfo.Length; z++)
                {
                    fil = int.Parse(posInfo[z]) / _x;
                    col = int.Parse(posInfo[z]) - (_x * fil);
                    table[col, fil]._color = i;
                }
            }
            for (int y = 0; y < _y; y++)
            {
                for (int x = 0; x < _x; x++)
                {
                    if (table[x, y].n) Console.WriteLine("---");
                    else Console.WriteLine("   ");
                }
                Debug.Log('\n');
                for (int x = 0; x < _x; x++)
                {
                    if (table[x, y].w) Debug.Log("|");
                    else Debug.Log(" ");
                    if (table[x, y]._type == Typito.init) Debug.Log("%");
                    else if(table[x, y]._type == Typito.empty) Debug.Log(" ");
                    else Debug.Log(table[x, y]._color);
                    if (table[x, y].e) Debug.Log("|");
                    else Debug.Log(" ");
                }
                Debug.Log('\n');
                for (int x = 0; x < _x; x++)
                {
                    if (table[x, y].s) Debug.Log("---");
                    else Debug.Log("   ");
                }
                Debug.Log('\n');
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("No existe el nivel " + _levelTxt + ": " + _level);
        }
    }
    void Update()
    {

    }
    #endregion //metodos
}
