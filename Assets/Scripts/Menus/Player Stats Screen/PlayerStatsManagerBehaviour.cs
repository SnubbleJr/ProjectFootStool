using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerStats
{
    public int id;
    public bool connected;

    //keep a list of kills delt and received
    private List<int> KOs = new List<int>();
    private List<int> TKOs = new List<int>();
    private int SDs;

    public void addKO(int victim)
    {
        if (victim != id)
            KOs.Add(victim);
    }

    public void addTKO(int attacker)
    {
        if (attacker != id)
            TKOs.Add(attacker);
        else
            //we KO'd ourselves
            SDs++;
    }

    public List<int> getKOs()
    {
        return KOs;
    }

    public List<int> getTKOs()
    {
        return TKOs;
    }

    public int getBullied()
    {
        int mode = getMode(KOs);
        if (mode <= 0)
            return -1;
        else
            return mode;
    }

    public int getBully()
    {
        int mode = getMode(TKOs);
        if (mode <= 0)
            return -1;
        else
            return mode;
    }

    public int getSDs()
    {
        return SDs;
    }

    private int getMode(List<int> list)
    {
        return list.GroupBy(i => i)  //Grouping same items
                .OrderByDescending(g => g.Count()) //now getting frequency of a value
                .Select(g => g.Key) //selecting key of the group
                .FirstOrDefault();   //Finally, taking the most frequent value
    }
}

public class PlayerState
{
    public Player player;
    public bool ready;
}

public class PlayerStatsManagerBehaviour : MonoBehaviour {

    public PlayerStatsDisplayer statsDisplayer;

    private PlayerState[] playerStates;
    private bool displaying = false;

    void Awake()
    {
        if (statsDisplayer == null)
            Debug.LogError("Stats Displayer Panel not connected to Player Stats Manager!");
    }

    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonPressed;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonPressed;
    }

    private void buttonPressed(PlayerInputScheme playerInputScheme, string name, float value)
    {
        if (displaying)
        {
            try
            {
                int playerIndex = findPlayerState(playerInputScheme.id);

                if (name == playerInputScheme.inputs[PlayerInput.SubmitInput].shortName)
                    playerStates[playerIndex].ready = true;

                if (name == playerInputScheme.inputs[PlayerInput.CancelInput].shortName)
                    playerStates[playerIndex].ready = false;
            }
            catch
            {
            }

            statsDisplayer.updateStatBoardState(playerStates);
            if (name == playerInputScheme.inputs[PlayerInput.SubmitInput].shortName && checkReady())
                finished();
        }
    }

    private int findPlayerState(int id)
    {
        if (playerStates != null)
            for (int i = 0; i < playerStates.Length; i++)
                if (playerStates[i].player.playerInputScheme.id == id)
                    return i;
        return -1;
    }

    private bool checkReady()
    {
        bool allReady = true;

        foreach (PlayerState playerState in playerStates)
            if (!playerState.ready)
                allReady = false;

        return allReady;
    }

    private void finished()
    {
        statsDisplayer.sendOffBoard();
    }

    public void removeStats()
    {
        displaying = false;
        statsDisplayer.gameObject.SetActive(false);
        GetComponent<PlayerManagerBehaviour>().resultsClosed();
    }

    public void displayStats(PlayerStats[] stats, PlayerControl[] playerControls)
    {
        playerStates = new PlayerState[stats.Length];

        for (int i = 0; i < playerStates.Length; i++)
        {
            playerStates[i] = new PlayerState();
            playerStates[i].player = playerControls[i].getPlayer();
            playerStates[i].ready = !stats[i].connected;
        }

        displaying = true;
        statsDisplayer.gameObject.SetActive(true);
        statsDisplayer.buildStatBoard(stats, playerControls, this);
        statsDisplayer.updateStatBoardState(playerStates);
    }

    public int getWinner()
    {
        return GetComponent<PlayerManagerBehaviour>().getWinner();
    }
}
