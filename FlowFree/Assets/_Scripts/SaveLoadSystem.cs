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
    public int _totalLevelsCompleted;                        // Time played this match
    public bool _adsRemoved;                                 // if the player paid for no ads
    public int _themeIndex;                                  // current theme for the player
    private int _hash;                                       // hash code 
    

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
        _hash = 0;
        _totalLevelsCompleted = 0;
        _themeIndex = theme;

        GetTimePlayed();
    } // PlayerData

    /// <summary>
    /// 
    /// Calculates the total number of levels completed
    /// 
    /// </summary>
    /// <returns> (int) number of levels completed </returns>
    public int GetTimePlayed()
    {
        _totalLevelsCompleted = 0;
        foreach (var data in _completedLevelsLot)
        {
            for (int i = 0; i < 150; ++i)
            {
                if (data.Value[i] != 0)
                    _totalLevelsCompleted++;
            }
        } // foreach

        _totalLevelsCompleted += (_hints + Convert.ToInt32(_adsRemoved));

        return _totalLevelsCompleted;
    } // GetTimePlayed

    /// <summary>
    /// 
    /// Give access to the hash. Consult only. 
    /// 
    /// </summary>
    /// <returns> (int) Hash's actual value. </returns>
    public int GetHash()
    {
        return _hash;
    } // GetHash

    /// <summary>
    /// 
    /// Used to set the new hash value after calculation. 
    /// 
    /// </summary>
    /// <param name="h">(int) New hash's value. </param>
    public void SetHash(int h)
    {
        _hash = h;
    } // SetHash
} // PlayerData


/// <summary>
/// 
/// Class used to save data and read data from a file. Singleton. Gives
/// access to some methods useful for storing Player's data.
/// 
/// </summary>
public class SaveLoadSystem : MonoBehaviour
{
    // Num packages
    static private int _releaseDay = 171221; 

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
    /// Creates a hash code based on the information provided. 
    /// 
    /// </summary>
    /// <param name="b"> (BinaryFormatter) Formater to serialize. </param>
    /// <param name="d"> (PlayerData) Data to encode. </param>
    /// <returns></returns>
    public static int Encrypt(BinaryFormatter b, PlayerData d)
    {
        // Create MemoryStream
        MemoryStream ms = new MemoryStream();

        // Serialize info in memoryStream
        b.Serialize(ms, d);

        // Seek 0 value
        ms.Seek(0, SeekOrigin.Begin);

        // Create and return hash code
        byte[] bytes = new byte[ms.Length];
        return ms.Read(bytes, 0, (int)ms.Length.GetHashCode());
    } // Encrypt

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

            // Calculate checks
            int totalLevelsComplete = playerData._totalLevelsCompleted;
            int hash = playerData.GetHash();

            playerData.SetHash(0);
            int checkPlayer = _releaseDay + playerData._hints + Convert.ToInt32(playerData._adsRemoved);
            foreach (var data in playerData._completedLevelsLot)
            {
                for (int i = 0; i < 150; ++i)
                {
                    if (data.Value[i] != 0)
                        checkPlayer++;
                }
            } // foreach
            int checkHash = Encrypt(bf, playerData);
            f.Close();

            // If data is corrupted, create new data
            if (hash == checkHash && totalLevelsComplete == checkPlayer)
            {
                // New packages
                if (playerData._completedLevelsLot.Count < lots.Count)
                {
                    for (int i = 0; i < lots.Count; i++)
                    {
                        // Check if there is no ad
                        if (lots[i] != "ad")
                        {
                            // Check if it is in dictionary
                            if (!playerData._completedLevelsLot.ContainsKey(lots[i]))
                            {
                                // If not, add it to it
                                playerData._completedLevelsLot.Add(lots[i], new int[150]);
                            } // if
                        } // if
                    } // for
                } // if

                return playerData;
            } // if
            else
            {
                return NewPlayerData(lots);
            } // else
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

        // Check data 
        d._totalLevelsCompleted = _releaseDay + d.GetTimePlayed();

        // Reset the hash for new codification
        if (d.GetHash() != 0)
        {
            d.SetHash(0);
        }

        // Create new hash code and write info in the file
        d.SetHash(Encrypt(bf, d));
        bf.Serialize(file, d);

        file.Close();
    } // SavePlayerData
}
