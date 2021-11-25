using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public BoardManager _boardManager;            // BoardManager instance

    /// <summary>
    /// 
    /// Sets all the scene to play the next level. 
    /// 
    /// </summary>
    public void PlayLevel()
    {
        // Set info
        // ... ad manager, pause, etc

        // Prepare board
        //LevelPackage lp = GameManager.GetInstance().GetLevelPackage();
        //int level = GameManager.GetInstance().GetLevel();

        //Map map = Map.FromLot(""/*lp.levels[level].ToString()*/);

        //_boardManager.EmptyBoard();
        _boardManager.SetMap(null/*created map*/);
    }
}
