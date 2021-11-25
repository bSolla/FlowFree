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
    public Dictionary<string, int> _completedLevelsPackage;  // Levels completed per package
    public int _hints;                                       // Hints available
    public int _timePlayed;                                  // Time played this match
    public bool _adsRemoved;                                 // if the player paid for no ads
    private int _hash;                                       // hash code 

    /// <summary>
    /// 
    /// Constructor. Creates the object with the parameters provided.
    /// 
    /// </summary>
    /// <param name="level"> (float) Current level. </param>
    /// <param name="completed"> (Dictionary) Number of levels completed per package. </param>
    /// <param name="hints"> (int) Number hints available. </param>
    /// <param name="removed"> (bool) Ads removed flag. </param>
    public PlayerData(float level, Dictionary<string, int> completed, int hints, bool removed)
    {
        _playerLevel = level;
        _completedLevelsPackage = completed;
        _hints = hints;
        _adsRemoved = removed;
        _hash = 0;
        _timePlayed = 0;

        GetTimePlayed();
    } // PlayerData

    /// <summary>
    /// 
    /// Calculates the time played, which is the number
    /// of levels completed.
    /// 
    /// </summary>
    /// <returns> (int) Total time played. </returns>
    public int GetTimePlayed()
    {
        _timePlayed = 0;
        foreach (var data in _completedLevelsPackage)
        {
            _timePlayed += data.Value;
        } // foreach

        _timePlayed += (_hints + Convert.ToInt32(_adsRemoved));

        return _timePlayed;
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
    static private int numPackages; //TODO: set to appropriate number

    /// <summary>
    /// 
    /// Creates new player data based on the packages that are 
    /// asigned to the GameManager. Ignores the ads. 
    /// 
    /// </summary>
    /// <param name="packages"> (string[]) Packages. </param>
    /// <returns> (PlayerData) New player data. </returns>
    public static PlayerData NewPlayerData(string[] packages)
    {
        Dictionary<string, int> completed = new Dictionary<string, int>();

        for (int i = 0; i < packages.Length; i++)
        {
            if (packages[i] != "ad")
                completed.Add(packages[i], 0);
        } // for

        PlayerData dat = new PlayerData(0.0f, completed, 0, false);

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
    /// <param name="packages"> (string) List of packages names. </param>
    /// <returns> (PlayerData) Data loaded or created. </returns>
    public static PlayerData ReadPlayerData(string[] packages)
    {
        if (File.Exists(Application.persistentDataPath + "/vmFlowFree.dat"))
        {
            // Initialize everything
            BinaryFormatter bf = new BinaryFormatter();
            FileStream f = File.Open(Application.persistentDataPath + "/vmFlowFree.dat", FileMode.Open);
            PlayerData d = (PlayerData)bf.Deserialize(f);

            // Calculate checks
            int totalTimePlayed = d._timePlayed;
            int hash = d.GetHash();

            d.SetHash(0);
            int checkTime = numPackages + d._hints + Convert.ToInt32(d._adsRemoved);
            foreach (var data in d._completedLevelsPackage)
            {
                checkTime += data.Value;
            } // foreach
            int checkHash = Encrypt(bf, d);
            f.Close();

            // If data is corrupted, create new data
            if (hash == checkHash && totalTimePlayed == checkTime)
            {
                // New packages
                if (d._completedLevelsPackage.Count < packages.Length)
                {
                    for (int i = 0; i < packages.Length; i++)
                    {
                        // Check if there is no ad
                        if (packages[i] != "ad")
                        {
                            // Check if it is in dictionary
                            if (!d._completedLevelsPackage.ContainsKey(packages[i]))
                            {
                                // If not, add it to it
                                d._completedLevelsPackage.Add(packages[i], 0);
                            } // if
                        } // if
                    } // for
                } // if

                return d;
            } // if
            else
            {
                return NewPlayerData(packages);
            } // else
        } // if
        else
        {
            return NewPlayerData(packages);
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
        d._timePlayed = numPackages + d.GetTimePlayed();

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
