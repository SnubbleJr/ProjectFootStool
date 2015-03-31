using UnityEngine;
using System.Collections;

public class StockGameMode : MonoBehaviour, IGameMode {

    public int stockCount;

    private PlayerManagerBehaviour playerManager;

    private GameObject[] players;
    private PlayerControl[] playerControls;
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
            }
        }
    }

    //returns the transform of the round winner if they are teh last one standing
    public Transform playerHit(PlayerControl playerControl)
    {
        playerControl.decreaseScore();

        int alivePlayerCount = 0;
        int currentAlivePlayer = 0;
        
        //check to see if anyone else is alive for the round
        for (int i = 0; i < playerControls.Length; i++)
        {
            //if we found someone who hasn't died
            if (playerControls[i].enabled == true)
            {
                alivePlayerCount++;
                currentAlivePlayer = i;
            }
        }

        //if only 1 player left alive
        if (alivePlayerCount <= 1)
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

        int playersLeftAlive = 0;
        int currentAlivePlayer = 0;

        for (int i = 0; i < playerControls.Length; i++)
        {
            if (playerControls[i].getScore() > 0)
            {
                playersLeftAlive++;
                currentAlivePlayer = playerControls[i].getPlayerNo();
            }
        }

        if (playersLeftAlive == 1)
            return currentAlivePlayer;

        return 0;
    }

    public void setPlayers(GameObject[] p)
    {
        players = p;
    }

    public void setPlayerControls(PlayerControl[] pc)
    {
        playerControls = pc;

        foreach(PlayerControl playerControl in playerControls)
        {
            playerControl.setScore(stockCount);
        }
    }

    public void setColors(PlayerColor[] col)
    {
        playerColors = col;
    }

    public void setScore(int score)
    {
        stockCount = score;
    }
}
