using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LoadStatsFile : MonoBehaviour {
    
    private const string path = "/Files/stats.txt";

    private static PlayerGlobalStats playerStats;

    //Here is a private reference only this class can access
    private static LoadStatsFile instance;

    //This is the public reference that other classes will use
    public static LoadStatsFile Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<LoadStatsFile>();
            return instance;
        }
    }

    public IEnumerator loadStatsFile()
    {
        yield return StartCoroutine(loadStats());
    }

    private IEnumerator loadStats()
    {
        //try and read
        try
        {
            //populate settings from the file
            string[] statsInFile = File.ReadAllLines(Application.dataPath + path);
            int[] valuesInFile = new int[statsInFile.Length];

            for (int i = 0; i < valuesInFile.Length; i++)
                int.TryParse(statsInFile[i], out valuesInFile[i]);

            playerStats = new PlayerGlobalStats(valuesInFile);

        }
        //assing
        catch
        {
            Debug.Log("Could not find " + Application.dataPath + path);
            playerStats = new PlayerGlobalStats();
        }

        saveStats();
        yield return null;
    }

    private void saveStats()
    {
        //write settings to a file,
        int[] statArray = playerStats.ToArray();
        string[] statsInFile = new string[statArray.Length];

        for (int i = 0; i < statsInFile.Length; i++)
            statsInFile[i] = statArray[i].ToString();

        File.WriteAllLines(Application.dataPath + path, statsInFile);
        Debug.Log("Wrote to " + Application.dataPath + path);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public void updateStats(PlayerGlobalStats newPlayerStats)
    {
        playerStats.level += newPlayerStats.level;
        playerStats.xp += newPlayerStats.xp;
        playerStats.kills += newPlayerStats.kills;
        playerStats.deaths += newPlayerStats.deaths;
        playerStats.SDs += newPlayerStats.SDs;
        playerStats.jumps += newPlayerStats.jumps;
        playerStats.flumps += newPlayerStats.flumps;
        playerStats.gamesPlayed += newPlayerStats.gamesPlayed;
        playerStats.gamesWon += newPlayerStats.gamesWon;
    }  
}      
       
       
       
       
       
       