using UnityEngine;
using System.Collections;

public class RaceGameMode : MonoBehaviour, IGameMode {

    //game mode is race, the last one standing in a round gains a point, first to stock count wins
    //dying does not make a player lose a life, but they do not respwan until the next round
    //level slowly moves up with procedural level

    public int stockCount;

    private PlayerManagerBehaviour playerManager;
    private LevelMoverScript levelMover;
    private TerrainGeneratorBehaviour terrainGenerator;

    private GameObject[] players;
    private PlayerControl[] playerControls;
    private PlayerColor[] playerColors;

    private int platChunckAmount = 15;
    private int currentChunkCount = 0;

    void Awake()
    {

        playerManager = GetComponent<PlayerManagerBehaviour>();
        levelMover = GameObject.Find("Level Mover").GetComponent<LevelMoverScript>();
        terrainGenerator = GameObject.Find("Terrain Generator").GetComponent<TerrainGeneratorBehaviour>();
    }

    void Start()
    {
        levelMover.enabled = true;
        terrainGenerator.enabled = true;

        restartRound();
    }

    void OnEnable()
    {
        restartRound();
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

        //if only 1 player left alive, round is over
        if (alivePlayerCount <= 1)
        {
            //stop level mover
            levelMover.enabled = false;

            //increase their score
            playerControls[currentAlivePlayer].increaseScore();
            
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
        //checking to see if anyone has hit the win condition

        for (int i = 0; i < playerControls.Length; i++)
        {
            if (playerControls[i].getScore() >= stockCount)
            {
                return playerControls[i].getPlayerNo();
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

        //everyone starts at zero in race
        foreach (PlayerControl playerControl in playerControls)
            playerControl.setScore(0);
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
