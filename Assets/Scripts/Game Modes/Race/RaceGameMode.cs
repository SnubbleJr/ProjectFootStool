using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceGameMode : MonoBehaviour, IGameMode {

    //game mode is race, the last one standing in a round gains a point, first to stock count wins
    //dying does not make a player lose a life, but they do not respwan until the next round
    //level slowly moves up with procedural level

    public int stockCount;

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
        restartRound();

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
                Color color1 = playerColors[i].color;
                Color color2 = Color.black;

                //if player is hit, grey out their score
                if (playerControls[i].getHit())
                {
                    color1.a = 0.5f;
                    color2.a = 0.5f;
                }

                GUI.contentColor = color2;
                GUI.Label(new Rect((width * j) + 2, 5, 50, 30), playerControls[i].getScore().ToString());
                GUI.contentColor = color1;
                GUI.Label(new Rect((width * j), 3, 50, 30), playerControls[i].getScore().ToString());

                if (team)
                {
                    //draw team in little number underneath
                    GUI.contentColor = color2;
                    GUI.Label(new Rect((width * j) + 1, 5 + 20, 50, 30), "<size=15>T " + playerControls[i].getTeamNo().ToString() + "</size>");
                    GUI.contentColor = color1;
                    GUI.Label(new Rect((width * j), 4 + 20, 50, 30), "<size=15>T " + playerControls[i].getTeamNo().ToString() + "</size>");
                }
                else
                {
                    //draw player in little number underneath
                    GUI.contentColor = color2;
                    GUI.Label(new Rect((width * j) + 1, 5 + 20, 50, 30), "<size=15>P " + playerControls[i].getTeamNo().ToString() + "</size>");
                    GUI.contentColor = color1;
                    GUI.Label(new Rect((width * j), 4 + 20, 50, 30), "<size=15>P " + playerControls[i].getTeamNo().ToString() + "</size>");
                }
            }
        }
    }

    void Update()
    {
        //check if we need more platforms
        if (levelMover.transform.position.y > (terrainGenerator.transform.position.y + currentChunkCount - 6))
        {
            terrainGenerator.spawnPlatforms(currentChunkCount + 1, platChunckAmount);
            currentChunkCount += platChunckAmount;
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
        {
            return null;
        }
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
        levelMover.enabled = true;
        levelMover.resetScript();
        terrainGenerator.resetScript(platChunckAmount);
        currentChunkCount = platChunckAmount;
    }
}
