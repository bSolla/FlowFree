using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// 
/// This struct stores the data of the Player that will be saved
/// lates (when the games asks for it). 
/// 
/// </summary>
[System.Serializable]
public struct PlayerData
{
    // Player's information 
    public float _playerLevel;                               // Coins that the player has
    /// <summary> 0 : not completed // 1 : completed // 2 : completed and perfect</summary>
    public Dictionary<string, int[]> _completedLevelsLot;    // Levels completed per lot
    public int _hints;                                       // Hints available
    public bool _adsRemoved;                                 // if the player paid for no ads
    public int _themeIndex;                                  // current theme for the player
    

    /// <summary>
    /// 
    /// Constructor. Creates the object with the parameters provided.
    /// 
    /// </summary>
    /// <param name="level"> (float) Current level. </param>
    /// <param name="completed"> (Dictionary) levels completed per lot. </param>
    /// <param name="hints"> (int) Number hints available. </param>
    /// <param name="removed"> (bool) Ads removed flag. </param>
    public PlayerData(float level, Dictionary<string, int[]> completed, int hints, bool removed, int theme)
    {
        _playerLevel = level;
        _completedLevelsLot = completed;
        _hints = hints;
        _adsRemoved = removed;
        _themeIndex = theme;
    } // PlayerData
} // PlayerData


/// <summary>
/// 
/// Class used to save data and read data from a file. Singleton. Gives
/// access to some methods useful for storing Player's data.
/// 
/// </summary>
public class SaveLoadSystem : MonoBehaviour
{
    /// <summary>
    /// 
    /// Creates new player data based on the packages that are 
    /// asigned to the GameManager. Ignores the ads. 
    /// 
    /// </summary>
    /// <param name="lots"> (string[]) Packages. </param>
    /// <returns> (PlayerData) New player data. </returns>
    public static PlayerData NewPlayerData(List<string> lots)
    {
        Dictionary<string, int[]> completed = new Dictionary<string, int[]>();

        for (int i = 0; i < lots.Count; i++)
        {
            if (lots[i] != "ad")
            {
                int[] levels = new int[150];

                completed.Add(lots[i], levels);
            }
                
        } // for

        PlayerData dat = new PlayerData(0.0f, completed, 0, false, 0);

        return dat;
    } // NewPlayerData


    /// <summary>
    /// 
    /// Reads the player data stored previously (if it exists) and 
    /// checks if all data is correct. 
    /// 
    /// </summary>
    /// <param name="lots"> (string) List of lot names. </param>
    /// <returns> (PlayerData) Data loaded or created. </returns>
    public static PlayerData ReadPlayerData(List<string> lots)
    {
        if (File.Exists(Application.persistentDataPath + "/vmFlowFree.dat"))
        {
            // Initialize everything
            BinaryFormatter bf = new BinaryFormatter();

            FileStream f = File.Open(Application.persistentDataPath + "/vmFlowFree.dat", FileMode.Open);
            PlayerData playerData = (PlayerData)bf.Deserialize(f);

            f.Close();

            return playerData;
        } // if
        else
        {
            return NewPlayerData(lots);
        } // else
    } // ReadPlayerData

    /// <summary>
    /// 
    /// Saves the current player data into a file. 
    /// 
    /// </summary>
    /// <param name="d"> (PlayerData) Data to store. </param>
    public static void SavePlayerData(PlayerData d)
    {
        // Initialize BinaryFormatter
        BinaryFormatter bf = new BinaryFormatter();

        // Create the new player save file
        FileStream file = File.Create(Application.persistentDataPath + "/vmFlowFree.dat");

        bf.Serialize(file, d);

        file.Close();
    } // SavePlayerData
}
