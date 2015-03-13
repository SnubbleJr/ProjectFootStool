using UnityEngine;
using System.Collections;

public class KOTHGameMode : MonoBehaviour, IGameMode {

    //game mode is koth, but if you're not in the hill, you start lossing any score that you had
    //if the hill is contested then everyone's points remain the same

    public int timeToWin;

    private PlayerManagerBehaviour playerManager;

    private GameObject[] players;
    private PlayerControl[] playerControls;
    private PlayerColor[] playerColors;

    private GameObject[] hills;
    private HillTriggerScript[] hillTriggers;
    
    private PlayerFollower mainCamera;

    // Use this for initialization
    void Start()
    {
        playerManager = GetComponent<PlayerManagerBehaviour>();
        mainCamera = Camera.main.GetComponent<PlayerFollower>();

        InvokeRepeating("timerTick", 0, 1);

        hills = GameObject.FindGameObjectsWithTag("Hill");

        if (hills.Length <= 0)
        {
            UnityEngine.Debug.LogError("Can't find hills to be king of!");
            this.enabled = false;
        }

        //generate trigger array, and enable all the hills
        hillTriggers = new HillTriggerScript[hills.Length];

        for (int i=0; i < hills.Length; i++)
        {
            hillTriggers[i] = hills[i].GetComponent<HillTriggerScript>();
            hills[i].SendMessage("activate");
        }
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

    //here we change each player's score based on their location
    //instead of having to calcuate weither a player is not in a trigger (to decrease their score)
    //we just decrease every players score by default
    //and increase by 2 if only one person is inside
    //by 1 if more than 1 is inside
    private void timerTick()
    {
        //increase score first, so we don't get jump ups from 0
        foreach (HillTriggerScript trigger in hillTriggers)
        {
            GameObject[] playersInHill = trigger.getPlayersInHill();
            applyScore(playersInHill);
        }

        //decrease everyone's score, if it isn't already 0
        foreach (PlayerControl playerControl in playerControls)
            if (playerControl.getScore() > 0)
                playerControl.decreaseScore();
    }

    private void applyScore(GameObject[] playersInHill)
    {
        int scoreToAdd;

        //if the hill is being contested, then only add 1 to each player's score
        //if just 1 player in the hill, then give them 2

        switch (playersInHill.Length)
        {
            case 0:
                return;
            case 1:
                scoreToAdd = 2;
                break;
            default:
                scoreToAdd = 1;
                break;
        }

        foreach (GameObject player in playersInHill)
            if (player)
                player.GetComponent<PlayerControl>().increaseScore(scoreToAdd);
    }

    //return nothing really, here killing isn't a big deal
    public Transform playerHit(PlayerControl playerControl)
    {
        //spawn inverter
        mainCamera.spawnInverter(playerControl.transform);

        //invert the colors of all alive players
        foreach (PlayerControl playControl in playerControls)
            if (playControl.enabled == true)
                playControl.invertColors();

        return null;
    }

    public int checkForWinner()
    {
        //checking to see if anyone has hit the win condition

        for (int i = 0; i < playerControls.Length; i++)
        {
            if (playerControls[i].getScore() >= timeToWin)
            {
                return (i + 1);
            }
        }

        return 0;
    }

    public void setPlayers(GameObject[] p)
    {
        players = p;
    }

    public void setPlayerControls(PlayerControl[] pc)
    {
        playerControls = pc;

        foreach (PlayerControl playerControl in playerControls)
        {
            playerControl.setScore(0);
        }
    }

    public void setColors(PlayerColor[] col)
    {
        playerColors = col;
    }

    public void deactivateHills()
    {
        try
        {
            foreach (GameObject hill in hills)
            {
                hill.SendMessage("deactivate");
            }
        }
        catch
        {
        }
    }

    public void setScore(int score)
    {
        timeToWin = score;
    }
}
