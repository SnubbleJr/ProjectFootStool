using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StockGameMode : MonoBehaviour, IGameMode {

    //game mode is stock, everytime a player is hit they lose a life, the last person left standing wins

    public int stockCount;

    private bool team = false;

    private PlayerManagerBehaviour playerManager;

    private GameObject[] players;
    private PlayerControl[] playerControls;
    private Dictionary<int, List<PlayerControl>> teams = new Dictionary<int, List<PlayerControl>>();
    private PlayerColor[] playerColors;

	// Use this for initialization
	void Start ()
    {
        playerManager = GetComponent<PlayerManagerBehaviour>();
	}
	
    void OnGUI()
    {
        GUI.skin = playerManager.skin;

        //player gui

        if (playerControls != null)
        {
            //if we get odd numbers of double the no of players (1,3,5,7 out of 8) then we get nice quartiles
            int width = Screen.width / (playerControls.Length * 2);

            int j = -1;

            for (int i = 0; i < playerControls.Length; i++)
            {
                j = j + 2;

                playerColors[i].color.a = 1;
                GUI.contentColor = playerColors[i].color;

                GUI.Label(new Rect((width * j) + 2, 5, 50, 30), "<color=black>" + playerControls[i].getScore().ToString() + "</color>");
                GUI.Label(new Rect((width * j), 3, 50, 30), playerControls[i].getScore().ToString());
                
                if (team)
                {
                    //draw team in little number underneath
                    GUI.Label(new Rect((width * j) + 1, 5 + 20, 50, 30), "<size=15><color=black>T " + playerControls[i].getTeamNo().ToString() + "</color></size>");
                    GUI.Label(new Rect((width * j), 4 + 20, 50, 30), "<size=15>T " + playerControls[i].getTeamNo().ToString() + "</size>");
                }
                else
                {
                    //draw player in little number underneath
                    GUI.Label(new Rect((width * j) + 1, 5 + 20, 50, 30), "<size=15><color=black>P " + playerControls[i].getTeamNo().ToString() + "</color></size>");
                    GUI.Label(new Rect((width * j), 4 + 20, 50, 30), "<size=15>P " + playerControls[i].getTeamNo().ToString() + "</size>");
                }
            }
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
            if (playerControls[i].enabled == true)
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

        //if only one team remains
        //or
        //if only 1 player left alive, round is over
        if (aliveTeamCount <=1 || alivePlayerCount <= 1)
        {
            return players[currentAlivePlayer].transform;
        }
        else
        {
            return null;
        }
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
            {
                teams[playerControl.getTeamNo()].Add(playerControl);
            }
        }

        //we're in team mode if we've actually got teams!
        if (teams.Count != playerControls.Length)
            team = true;
        else
            team = false;
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
    }
}
