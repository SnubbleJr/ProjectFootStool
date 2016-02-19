using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KOTHGameMode : MonoBehaviour, IGameMode {

    //game mode is koth, but if you're not in the hill, you start lossing any score that you had
    //if the hill is contested then everyone's points remain the same

    public SFX closeToWinSound = SFX.PointAdded;

    private int timeToWin;

    private bool team = false;
    
    private PlayerManagerBehaviour playerManager;

    private GameObject[] players;
    private PlayerControl[] playerControls;
    private Dictionary<int, List<PlayerControl>> teams = new Dictionary<int, List<PlayerControl>>();
    private PlayerColor[] playerColors;

    private GameObject[] hills;
    private HillTriggerScript[] hillTriggers;
    
    private PlayerFollower mainCamera;

    private bool playerCloseToWinning = false;

    // Use this for initialization
    void OnEnable()
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
            hills[i].GetComponent<HillActivationScript>().activate();
        }
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
        //updates score to canvas, and sets sparticle system for players in hill

        foreach (PlayerControl playerControl in playerControls)
        {
            int playerNo = playerControl.getPlayerNo();
            
            if (playerControl.getHit())
                GameModeCanavsBehaviour.Instance.setPlayerInactive(playerNo);
            else
                GameModeCanavsBehaviour.Instance.setPlayerActive(playerNo);

            bool inHill = false;

            //then set particles for those who are in hills
            foreach (GameObject player in getPlayersInAllHills())
                if (player.GetComponent<PlayerControl>().getPlayerNo() == playerControl.getPlayerNo())
                    inHill = true;

            if (inHill)
                GameModeCanavsBehaviour.Instance.setInHill(playerControl.getPlayerNo());
            else
                GameModeCanavsBehaviour.Instance.unsetInHill(playerControl.getPlayerNo());
        }
    }

    //here we change each player's score based on their location
    //and increase by 2 if only one person is inside
    //by 1 if more than 1 is inside
    private void timerTick()
    {
        playerCloseToWinning = false;

        //increase score first, so we don't get jump ups from 0
        foreach (HillTriggerScript trigger in hillTriggers)
        {
            GameObject[] playersInHill = trigger.getPlayersInHill().ToArray();
            GameObject[] teamsInHill = trigger.getTeamsInHill().ToArray();
            applyScore(playersInHill, teamsInHill);
        }

        if (playerCloseToWinning)
            SFXManagerBehaviour.Instance.playSound(closeToWinSound);

        //decrease everyone's score, if it isn't already 0
        //and set their gui (done in tick so we don't get flitting between this and update)
        foreach (PlayerControl playerControl in playerControls)
        {
            GameModeCanavsBehaviour.Instance.setPlayerScore(playerControl.getPlayerNo(), playerControl.getScore());
        }
    }

    private void applyScore(GameObject[] playersInHill, GameObject[] teamsInHill)
    {
        int scoreToAdd;

        //if the hill is being contested, then only add 1 to each player's score
        //if just 1 player in the hill, then give them 2

        switch (teamsInHill.Length)
        {
            case 0:
                return;
            case 1:
                scoreToAdd = 1;
                break;
            default:
                scoreToAdd = 0;
                break;
        }

        //increase the score for all players that are int he ring, but have not reached the max yet
        foreach (GameObject player in playersInHill)
        {
            if (player)
            {
                PlayerControl playerControl = player.GetComponent<PlayerControl>();

                //if we are close to winning, and are the only one in the ring, then add a warning sound
                if (playerControl.getScore() >= (timeToWin - 4) && scoreToAdd == 1)
                    playerCloseToWinning = true;

                if (playerControl.getScore() >= timeToWin)
                    scoreToAdd = 1;
                playerControl.increaseScore(scoreToAdd);
            }
        }

        updateScores();
    }

    //return nothing really, here killing isn't a big deal
    public Transform playerHit(PlayerControl playerControl)
    {
        //spawn inverter
        mainCamera.spawnInverter(playerControl.transform);

        //invert the colors of all alive players
        foreach (PlayerControl playControl in playerControls)
            if (playControl.enabled == true)
                playControl.resetColors();

        //inform the hills that this player has been hit, and they are to remove this player
        foreach (HillTriggerScript hillTrigger in hillTriggers)
            hillTrigger.removePlayerFromIn(playerControl);

        return null;
    }

    public Transform getPlayerTrans()
    {
        //return first guy inside hill

        GameObject[] playersInHill = hillTriggers[0].getPlayersInHill().ToArray();
        return playersInHill[0].transform;
    }

    public int checkForWinner()
    {
        //checking to see if any one team has hit the win condition
        
        foreach (List<PlayerControl> team in teams.Values)
        {
            int winCount = 0;
            foreach (PlayerControl playerControl in team)
            {
                if (playerControl.getScore() >= timeToWin)
                    winCount++;
            }

            if (winCount >= team.Count)
                return team[0].getTeamNo();
        }

        updateScores();

        return 0;
    }

    private List<GameObject> getPlayersInAllHills()
    {
        List<GameObject> playersInHills = new List<GameObject>();

        foreach (HillTriggerScript hillTrigger in hillTriggers)
            foreach (GameObject playerInHill in hillTrigger.getPlayersInHill())
                playersInHills.Add(playerInHill);

        return playersInHills;
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
            playerControl.setScore(0);

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

        setScores();
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
                hill.GetComponent<HillActivationScript>().deactivate();
        }
        catch
        {
        }
        
        //stop timer
        CancelInvoke("timerTick");
    }

    public void setScore(int score)
    {
        timeToWin = score;
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
