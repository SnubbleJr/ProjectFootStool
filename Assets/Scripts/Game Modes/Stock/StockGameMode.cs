using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StockGameMode : MonoBehaviour, IGameMode {

    //game mode is stock, everytime a player is hit they lose a life, the last person left standing wins

    private int stockCount;

    private bool team = false;

    private PlayerManagerBehaviour playerManager;

    private GameObject[] players;
    private PlayerControl[] playerControls;
    private Dictionary<int, List<PlayerControl>> teams = new Dictionary<int, List<PlayerControl>>();
    private PlayerColor[] playerColors;

	// Use this for initialization
	void OnEnable()
    {
        playerManager = GetComponent<PlayerManagerBehaviour>();
	}

    private void setScores()
    {
        int[] spawnOrder = playerManager.getSpawnOrder();

        string teamPrefix = team ? "T" : "P";

        clearScores();
        
        //set score based on the order that they spawned
        for (int i = 0; i < playerControls.Length; i++)
        {
            int index = 0;

            for (int j = 0; j < spawnOrder.Length; j++)
                if (i == spawnOrder[j])
                    index = j;
            PlayerControl playerControl = playerControls[index];
            GameModeCanavsBehaviour.Instance.setPlayer(playerControl.getPlayerNo(), playerControl.getColor(), teamPrefix, playerControl.getTeamNo());
            GameModeCanavsBehaviour.Instance.setPlayerScore(playerControl.getPlayerNo(), playerControl.getScore());
        }
    }

    private void clearScores()
    {
        foreach (PlayerControl playerControl in playerControls)
            GameModeCanavsBehaviour.Instance.unsetPlayer(playerControl.getPlayerNo());
    }

    private void updateScores()
    {
        foreach (PlayerControl playerControl in playerControls)
        {
            int playerNo = playerControl.getPlayerNo();

            GameModeCanavsBehaviour.Instance.setPlayerScore(playerNo, playerControl.getScore());

            if (playerControl.getHit())
                GameModeCanavsBehaviour.Instance.setPlayerInactive(playerNo);
            else
                GameModeCanavsBehaviour.Instance.setPlayerActive(playerNo);
        }
    }

    //returns the transform of the round winner if they are teh last one standing
    public Transform playerHit(PlayerControl playerControl)
    {
        playerControl.decreaseScore();
        
        int alivePlayerCount = 0;
        int currentAlivePlayer = 0;

        int aliveTeamCount = 0;
        int currentAliveTeam = 0;

        //check to see if anyone else is alive for the round
        for (int i = 0; i < playerControls.Length; i++)
        {
            //if we found someone who hasn't died
            if (playerControls[i].getHit() == false)
            {
                alivePlayerCount++;
                currentAlivePlayer = i;

                //if new team
                if(playerControls[i].getTeamNo() != currentAliveTeam)
                {
                    currentAliveTeam = playerControls[i].getTeamNo();
                    aliveTeamCount++;
                }
            }
        }

        updateScores();

        //if only one team remains
        //or
        //if only 1 player left alive, round is over
        if (aliveTeamCount <=1 || alivePlayerCount <= 1)
            return players[currentAlivePlayer].transform;
        else
            return null;
    }

    public int checkForWinner()
    {
        //checking to see if anyone has hit the win condition

        int alivePlayerCount = 0;
        int currentAlivePlayer = 0;

        int aliveTeamCount = 0;
        int currentAliveTeam = 0;

        //check to see if anyone else is alive for the round
        foreach (PlayerControl playerControl in playerControls)
        {
            //if we found someone who hasn't died
            if (playerControl.getScore() > 0)
            {
                alivePlayerCount++;
                currentAlivePlayer = playerControl.getPlayerNo();

                //if new team
                if (playerControl.getTeamNo() != currentAliveTeam)
                {
                    currentAliveTeam = playerControl.getTeamNo();
                    aliveTeamCount++;
                }
            }
        }

        updateScores();

        //if only one team remains
        //or
        //if only 1 player left alive
        if (aliveTeamCount <= 1 || alivePlayerCount <= 1)
            return currentAliveTeam;
        else
            return 0;
    }

    public void setPlayers(GameObject[] p)
    {
        players = p;
    }

    public void setPlayerControls(PlayerControl[] pc)
    {
        playerControls = pc;

        //building the team list
        //checking to see if any one team has hit the win condition

        teams = new Dictionary<int, List<PlayerControl>>();

        foreach (PlayerControl playerControl in playerControls)
        {
            playerControl.setScore(stockCount);

            //if new team found
            if (!teams.ContainsKey(playerControl.getTeamNo()))
            {
                List<PlayerControl> teamPlayers = new List<PlayerControl>();
                teamPlayers.Add(playerControl);
                teams.Add(playerControl.getTeamNo(), teamPlayers);
            }
            //else it's a team we know, and add this player ot the team
            else
                teams[playerControl.getTeamNo()].Add(playerControl);
        }

        //we're in team mode if we've actually got teams!
        if (teams.Count != playerControls.Length)
            team = true;
        else
            team = false;

        setScores();
    }

    public void setColors(PlayerColor[] col)
    {
        playerColors = col;
    }

    public void setScore(int score)
    {
        stockCount = score;
    }

    public void restartRound()
    {
        setScores();
        updateScores();
    }

    public void endGame()
    {
        clearScores();
    }
}
