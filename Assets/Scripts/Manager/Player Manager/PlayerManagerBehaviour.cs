using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//spawns players and manages them
public class PlayerManagerBehaviour : MonoBehaviour {

    public bool debug;
    public GameObject playerPrefab;
    public GUISkin skin;

    private GameObject[] spawnPointHolders; //holds the different points for players
    private Transform[] spawns;

    private GameObject[] players;
    private PlayerStats[] playerStats;
    private List<int> spawnOrder = new List<int>();  //to keep track of what order we are spawning in

    private PlayerControl[] playerControls;

    private PlayerFollower mainCamera;

    private GameObject[] fadingColorObjects;

    private PlayerColor[] playerColors;

    private IGameMode gameMode;

    private bool team;

    private StockGameMode stockGame;
    private KOTHGameMode kothGame;
    private RaceGameMode raceGame;
    private PlayerStatsManagerBehaviour playerStatsManager;

    private int winner;
    private bool gameOver;
    private bool displayWinner = false;

    // Use this for initialization
    void Awake()
    {
        if (playerPrefab == null)
        {
            UnityEngine.Debug.LogError("No player prefab given to the player manager!");
            this.enabled = false;
        }

        stockGame = GetComponent<StockGameMode>();
        kothGame = GetComponent<KOTHGameMode>();
        raceGame = GetComponent<RaceGameMode>();
        playerStatsManager = GetComponent<PlayerStatsManagerBehaviour>();
    }

    void OnEnable()
    {
        InputManagerBehaviour.playerAdded += playerAdded;
        InputManagerBehaviour.playerRemoved += playerRemoved;
    }

    void OnDisable()
    {
        InputManagerBehaviour.playerAdded -= playerAdded;
        InputManagerBehaviour.playerRemoved -= playerRemoved;
    }

    public void startGame(Player[] Players, GameMode mode, bool teamMode, int scoreAmount)
    {
        spawnPointHolders = SpawnPointGenerator.Instance.getSpawnPointHolders();

        if (spawnPointHolders.Length <= 0)
            UnityEngine.Debug.LogError("Cannot find spawn points under player manager!");

        fadingColorObjects = GameObject.FindGameObjectsWithTag("FadingColor");

        gameOver = false;

        team = teamMode;

        GameObject[] changingColorObjects = GameObject.FindGameObjectsWithTag("ChangeableColor");

        playerColors = getPlayerColors(Players);
        
        //initilise palyers into the world
        spawnPlayers(Players);
        
        initialseCameras();

        playerControls = initilialsePlayerControl(Players);

        //reseting values
        Time.timeScale = 1;

        GetComponentInChildren<CountdownBehaviour>().startCountdown();

        foreach (GameObject obj in changingColorObjects)
            obj.SendMessage("startFade");

        //now set the camera to not follow disabled players
        mainCamera.startedGame = false;

        setGameMode(mode, scoreAmount);
    }

    public void countDownFinished()
    {
        //called when countdown has reached 0

        mainCamera.startedGame = true;

        foreach (PlayerControl playerControl in playerControls)
        {
            if (playerControl)
                playerControl.enabled = true;
        }
    }

    private void spawnPlayers(Player[] Players)
    {
        //set players and spawn them
        players = new GameObject[Players.Length];
        playerStats = new PlayerStats[Players.Length];
        spawns = selectSpawns(Players.Length);

        //we have to invert the spawn locations, as they go backwards
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = Instantiate(playerPrefab, spawns[i].position, Quaternion.identity) as GameObject;
            PlayerStats stat = new PlayerStats();
            stat.id = Players[i].playerNo;
            stat.connected = true;
            playerStats[i] = stat;
        }

        //reorder players in random order
        List<int> usedSpawns = new List<int>();

        for (int i = 0; i < players.Length; i++)
        {
            int j;

            //if 2 players, we want to ensure same spawns
            if (players.Length == 2)
                j = i;
            else
                j = getSpawn();

            while (usedSpawns.Contains(j))
                j = getSpawn();

            usedSpawns.Add(j);

            players[i].transform.position = spawns[j].position;
        }
        //log the order we spawned so we can re arrange the ui
        spawnOrder = usedSpawns;
    }

    private void playerAdded(PlayerInputScheme scheme)
    {
        if (playerStats != null)
            //auto ready on removal of player
            foreach (PlayerStats playerStat in playerStats)
                if (playerStat.id == scheme.id)
                    playerStat.connected = true;
    }

    private void playerRemoved(PlayerInputScheme scheme)
    {
        if (playerStats != null)
            //auto ready on removal of player
            foreach (PlayerStats playerStat in playerStats)
                if (playerStat.id == scheme.id)
                    playerStat.connected = false;
    }

    private PlayerColor[] getPlayerColors(Player[] players)
    {
        //extracts an array of PlayerColors from players
        PlayerColor[] colors = new PlayerColor[players.Length];

        for (int i = 0; i < players.Length; i++)
            colors[i] = players[i].color;

        return colors;
    }

    private Transform[] selectSpawns(int noOfPlayers)
    {
        //determin what spawn to use based on player size
        List<Transform> spawnsList = new List<Transform>();

        foreach (GameObject spawnPointHolder in spawnPointHolders)
        {
            if (spawnPointHolder.transform.childCount == noOfPlayers)
            {
                foreach (Transform child in spawnPointHolder.transform)
                    spawnsList.Add(child);
            }
        }

        return spawnsList.ToArray();
    }

    private void initialseCameras()
    {
        //find camera and give it the players
        mainCamera = Camera.main.GetComponent<PlayerFollower>();
        mainCamera.setPlayers(players);

        //set camera colors
        Color cameaBG = Camera.main.backgroundColor;
        Color inverseCameraBg = new Color(1.0f - cameaBG.r, 1.0f - cameaBG.g, 1.0f - cameaBG.b);

        mainCamera.setDebug(debug);
    }

    private List<int> getOpponenets(Player player, Player[] Players)
    {
        //create a list of opponents by copy Players and remove themselves
        List<int> opponents = new List<int>();

        for (int i = 0; i < Players.Length; i++)
            if (Players[i].teamNo != player.teamNo)
                opponents.Add(Players[i].teamNo);

        return opponents;
    }

    private PlayerControl[] initilialsePlayerControl(Player[] Players)
    {
        //create player control array
        PlayerControl[] playerControls = new PlayerControl[players.Length];

        for (int i = 0; i < playerControls.Length; i++)
        {
            playerControls[i] = players[i].GetComponent<PlayerControl>();
            playerControls[i].setDebug(debug);

            List<int> opponents = getOpponenets(Players[i], Players);

            playerControls[i].setPlayer(Players[i], opponents);
            playerControls[i].enabled = false;
        }

        return playerControls;
    }

    private void setGameMode(GameMode mode, int scoreAmount)
    {
        //setting stock here just for testing
        switch (mode)
        {
            case GameMode.Stock:
                stockGame.enabled = true;
                gameMode = stockGame;
                break;
            case GameMode.Koth:
                kothGame.enabled = true;
                gameMode = kothGame;
                break;
            case GameMode.Race:
                raceGame.enabled = true;
                gameMode = raceGame;
                break;
        }

        gameMode.setScore(scoreAmount);
        gameMode.setPlayers(players);
        gameMode.setPlayerControls(playerControls);
        gameMode.setColors(playerColors);
    }

    void OnGUI()
    {
        GUI.skin = skin;
        if (gameOver && displayWinner)
        {
            if (winner != 0)
            {
                if (team)
                {
                    GUI.Label(new Rect((Screen.width / 2) - 248, (Screen.height / 2) + 2, 500, 50), "<color=black>Team " + winner + " has won!</color>");
                    GUI.Label(new Rect((Screen.width / 2) - 250, (Screen.height / 2), 500, 50), "<color=white>Team " + winner + " has won!</color>");
                }
                else
                {
                    GUI.Label(new Rect((Screen.width / 2) - 248, (Screen.height / 2) + 2, 500, 50), "<color=black>Player " + winner + " has won!</color>");
                    GUI.Label(new Rect((Screen.width / 2) - 250, (Screen.height / 2), 500, 50), "<color=white>Player " + winner + " has won!</color>");
                }
            }
            else
            {
                GUI.Label(new Rect((Screen.width / 2) - 248, (Screen.height / 2) + 2, 500, 50), "<color=black>No contest!</color>");
                GUI.Label(new Rect((Screen.width / 2) - 250, (Screen.height / 2), 500, 50), "<color=white>No contest!</color>");
            }
        }
    }

    void Update()
    {
        //end of game check
        if (!gameOver)
        {
            winner = gameMode.checkForWinner();

            if (winner != 0)
                endMatch();
        }
    }

    private void logKO(PlayerControl playerControl, int opponent)
    {
        //add a point to the player that just killed this guy

        //we minus 8 from the killer's layer position to get their id
        int killer = (int)Mathf.Log(opponent, 2) - 8;

        //log TKO for player, and KO for killer
        playerStats[findPlayerStat(playerControl.getPlayerNo())].addTKO(killer);
        playerStats[findPlayerStat(killer)].addKO(playerControl.getPlayerNo());
    }

    private int findPlayerStat(int id)
    {
        for (int i = 0; i < playerStats.Length; i++)
            if (playerStats[i].id == id)
                return i;
        return 0;
    }

    public void playerHit(PlayerControl playerControl, int opponent)
    {
        logKO(playerControl, opponent);

        playerControl.enabled = false;

        Transform winnerTrans = null;

        //call the current game mode with player hit
        winnerTrans = gameMode.playerHit(playerControl);

        //check what gamemode we are in
        //stock and race game mode, no respawning
        if (stockGame.enabled == true || raceGame.enabled == true)
        {
            //if round winner
            if (winnerTrans != null)
            {
                foreach (GameObject obj in fadingColorObjects)
                    obj.SendMessage("setFade", true);

                Camera.main.gameObject.SendMessage("setFade", true);

                mainCamera.zoomInOnWinner(winnerTrans);

                //respawn all if we don't have a winner
                if (winner == 0)
                    StartCoroutine(respawnAllDelayed(0.5f));
            }
        }

        //if not in stocks, then respawn player in a bit
        if (kothGame.enabled == true)
            StartCoroutine(respawnPlayerDelayed(playerControl, 0.5f));
    }

    private IEnumerator respawnAllDelayed(float time)
    {
        yield return new WaitForSeconds(time);

        if (!gameOver)
        {
            Camera.main.gameObject.SendMessage("setFade", false);

            foreach (GameObject obj in fadingColorObjects)
                obj.SendMessage("setFade", false);

            mainCamera.resetZoom();

            List<int> usedSpawns = new List<int>();

            for (int i = 0; i < players.Length; i++)
            {
                int j;

                //if 2 players, we want to ensure same spawns
                if (players.Length == 2)
                    j = i;
                else
                    j = getSpawn();

                while (usedSpawns.Contains(j))
                    j = getSpawn();

                usedSpawns.Add(j);

                respawnPlayer(i, j);
            }

            //log the order we spawned so we can re arrange the ui
            spawnOrder = usedSpawns;

            gameMode.restartRound();
        }
    }

    private int getSpawn()
    {
        int spawn = (int)Random.Range(0, spawns.Length);
        if (spawn >= spawns.Length)
            spawn = spawns.Length - 1;
        return spawn;
    }

    private IEnumerator respawnPlayerDelayed(PlayerControl playerControl, float time)
    {
        int playerIndex = System.Array.IndexOf(playerControls, playerControl);

        yield return new WaitForSeconds(time);

        respawnPlayer(playerIndex, playerIndex);
    }

    private void respawnPlayer(int playerIndex, int spawnIndex)
    {
        if (!gameOver)
        {
            //don't respawn if stock game and no stocks
            //so only respawn if more than 1 stock or not playing stock game
            if (playerControls[playerIndex].getScore() > 0 || stockGame.enabled == false)
            {
                players[playerIndex].transform.position = spawns[spawnIndex].position;
                playerControls[playerIndex].enabled = true;
                playerControls[playerIndex].respawn();
            }
        }
    }

    public int[] getSpawnOrder()
    {
        return spawnOrder.ToArray();
    }

    public void StopMatch()
    {
        //public way to call end game, if this has happened, the match has been forfitted
        winner = 0;
        endMatch();
    }

    private void endMatch()
    {
        SFXManagerBehaviour.Instance.stopAllLoops();

        MusicManagerBehaviour.Instance.setGameOver(true);

        foreach (PlayerControl playerControl in playerControls)
            playerControl.enabled = false;
        
        gameMode.endGame();

        //set all gamemodes to false
        stockGame.enabled = false;

        kothGame.deactivateHills();
        kothGame.enabled = false;

        raceGame.enabled = false;
        
        gameOver = true;

        playerStatsManager.enabled = false;

        mainCamera.startedGame = false;

        //fancy zoom of last kill
        //set off the kill cam
        displayEndGamePanels();
    }

    private void displayEndGamePanels()
    {
        //send off for winner to be displayed, and wait
        GetComponent<EndGamePanelManager>().displayPanels();
    }

    public void winnerDisplayed()
    {
        //once wwe have gotten the last panel into place, show the winner
        displayWinner = true;

        //get rid of this zoom in

        //do normal end game stuff in on winner if it's a koth game
        //if not no contest
        if (winner != 0)
        {
            if (kothGame.enabled)
            {
                foreach (GameObject obj in fadingColorObjects)
                    obj.SendMessage("setFade", true);

                Camera.main.gameObject.SendMessage("setFade", true);

                mainCamera.zoomInOnWinner(kothGame.getPlayerTrans());
            }
        }
    }

    public void endGame()
    {
        LevelManagerBehaviour.Instance.resetLevels();

        foreach (GameObject player in players)
            Destroy(player);

        GameObject.Find("Menu Manager").GetComponent<MainMenuScript>().endGame();

        displayWinner = false;

        //temp measure
        mainCamera.resetZoom();

        displayResults();
    }

    private void displayResults()
    {
        //send off for results to be displayed, and wait
        playerStatsManager.enabled = true;
        playerStatsManager.displayStats(playerStats, playerControls);
    }

    public void resultsClosed()
    {
        finishMatch();
    }

    private void finishMatch()
    {
        MusicManagerBehaviour.Instance.setGameOver(false);

        GameObject.Find("Menu Manager").GetComponent<MainMenuScript>().exitLevel();

        Camera.main.gameObject.SendMessage("setFade", false);

        foreach (GameObject obj in fadingColorObjects)
            obj.SendMessage("setFade", false, SendMessageOptions.DontRequireReceiver);

        GameObject[] changingColorObjects = GameObject.FindGameObjectsWithTag("ChangeableColor");
        foreach (GameObject obj in changingColorObjects)
            obj.SendMessage("setFade", false);
    }

    public PlayerStats[] getStats()
    {
        return playerStats;
    }

    public int getWinner()
    {
        return winner;
    }

    public GameObject[] getPlayers()
    {
        return players;
    }
}
