using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceGameMode : MonoBehaviour, IGameMode {

    //game mode is race, the last one standing in a round gains a point, first to stock count wins
    //dying does not make a player lose a life, but they do not respwan until the next round
    //level slowly moves up with procedural level

    private int stockCount;

    private bool team = false;

    private PlayerManagerBehaviour playerManager;
    private LevelMoverScript levelMover;
    private TerrainGeneratorBehaviour terrainGenerator;

    private GameObject[] players;
    private PlayerControl[] playerControls;
    private Dictionary<int, List<PlayerControl>> teams = new Dictionary<int, List<PlayerControl>>();
    private PlayerColor[] playerColors;

    private int platChunckAmount = 15;
    private int currentChunkCount = 0;

    void Awake()
    {
        playerManager = GetComponent<PlayerManagerBehaviour>();
        levelMover = GetComponentInChildren<LevelMoverScript>();
        terrainGenerator = GetComponentInChildren<TerrainGeneratorBehaviour>();
    }

    void Start()
    {
        tryStartGame();
    }

    void OnEnable()
    {
        tryStartGame();
    }

    void OnDisable()
    {
        levelMover.enabled = false;
        terrainGenerator.enabled = false;
    }

    private void tryStartGame()
    {
        try
        {
            terrainGenerator.enabled = true;
            restartRound();
        }
        catch
        {
            playerManager = GetComponent<PlayerManagerBehaviour>();
            levelMover = GameObject.Find("Level Mover").GetComponent<LevelMoverScript>();
            terrainGenerator = GameObject.Find("Terrain Generator").GetComponent<TerrainGeneratorBehaviour>();

            terrainGenerator.enabled = true;
            restartRound();
        }
    }

    void Update()
    {
        //check if we need more platforms
        if (levelMover.transform.position.y > (terrainGenerator.transform.position.y + currentChunkCount - 15))
        {
            terrainGenerator.spawnPlatforms(currentChunkCount + 1, platChunckAmount);
            currentChunkCount += platChunckAmount;
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
                if (playerControls[i].getTeamNo() != currentAliveTeam)
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
        if (aliveTeamCount <= 1 || alivePlayerCount <= 1)
        {
            //stop level mover
            levelMover.enabled = false;

            //increase each team mates score
            foreach (PlayerControl player in teams[currentAliveTeam])
                player.increaseScore();
            
            //amd return round winner
            return players[currentAlivePlayer].transform;
        }
        else
            return null;
    }

    public int checkForWinner()
    {
        //checking to see if any one team has hit the win condition

        foreach(PlayerControl playerControl in playerControls)
        {
            if (playerControl.getScore() >= stockCount)
            {
                return playerControl.getTeamNo();
            }
        }

        updateScores();

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

        clearScores();
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
        if (levelMover != null)
        {
            levelMover.enabled = true;
            levelMover.resetScript();
        }
        terrainGenerator.resetScript(platChunckAmount);
        currentChunkCount = platChunckAmount;

        if (playerControls != null)
        {
            setScores();
            updateScores();
        }
    }

    public void endGame()
    {
        clearScores();
    }
}
