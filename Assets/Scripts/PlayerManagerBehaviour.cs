using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//spawns players and manages them

public class PlayerManagerBehaviour : MonoBehaviour {

    public bool debug;
    public GameObject playerPrefab;
    public int firstTo = 5;
    public GUISkin skin;

    private GameObject[] spawnPointHolders; //holds the different points for players
    private Transform[] spawns;

    private GameObject[] players;

    private PlayerControl[] playerControls;

    private PlayerFollower mainCamera;

    private GameObject[] fadingColorObjects;

    private PlayerColor[] playerColors;

    private IGameMode gameMode;

    private StockGameMode stockGame;
    private KOTHGameMode kothGame;
    private RaceGameMode raceGame;

    private int winner;
    private bool gameOver;
    private int countDownStart = 5;
    private int countDown;
    
	// Use this for initialization
    void Awake()
    {
        spawnPointHolders = GameObject.FindGameObjectsWithTag("SpawnPoints");

        if (spawnPointHolders.Length <= 0)
        {
            UnityEngine.Debug.LogError("Cannot find spawn points under player manager!");
            this.enabled = false;
        }

        if (playerPrefab == null)
        {
            UnityEngine.Debug.LogError("No player prefab given to the player manager!");
            this.enabled = false;
        }

        stockGame = GetComponent<StockGameMode>();
        kothGame = GetComponent<KOTHGameMode>();
        raceGame = GetComponent<RaceGameMode>();
    }
    
    public void startGame(Player[] Players, GameMode mode, int scoreAmount)
    {
        fadingColorObjects = GameObject.FindGameObjectsWithTag("FadingColor");

        gameOver = false;

        GameObject[] changingColorObjects = GameObject.FindGameObjectsWithTag("ChangeableColor");

        playerColors = getPlayerColors(Players);

        //set players and spawn them
        players = new GameObject[Players.Length];
        spawns = selectSpawns(Players.Length);        
        //we have to invert the spawn locations, as they go backwards
        for (int i = 0; i < players.Length; i++)
            players[i] = Instantiate(playerPrefab, spawns[i].position, Quaternion.identity) as GameObject;
        
        initialseCameras();

        playerControls = initilialsePlayerControl(Players);

        //reseting values for when we start again
        Time.timeScale = 1;
        countDown = countDownStart;
        InvokeRepeating("countdown", 0, 1f);
        foreach (GameObject obj in changingColorObjects)
            obj.SendMessage("startFade");

        setGameMode(mode, scoreAmount);
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
            if (Players[i].playerNo != player.playerNo)
                opponents.Add(Players[i].playerNo);

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

    private void countdown()
    {
        //countdown to 0
        if (countDown > 0)
            countDown--;
        else
        {
            //wehen hit 0, set to -1 so go on screen disapears
            countDown = -1;
            CancelInvoke("countdown");
        }

        //checked at 0, so players are active on go
        if (countDown == 0)
        {
            //now set the camera to not follow disabled players
            mainCamera.start = true;

            foreach (PlayerControl playerControl in playerControls)
            {
                playerControl.enabled = true;
            }
        }
    }

    void OnGUI()
    {
        GUI.skin = skin;

        if (countDown > 0)
        {
            GUI.Label(new Rect((Screen.width / 2) - 23, Screen.height / 2 + 2, 50, 30), "<color=black>" + countDown.ToString() + "</color>");
            GUI.Label(new Rect((Screen.width / 2) - 25, Screen.height / 2, 50, 30), "<color=white>" + countDown.ToString() + "</color>");
        }

        if (countDown == 0)
        {
            GUI.Label(new Rect((Screen.width / 2) - 48, Screen.height / 2 + 2, 100, 30), "<color=black>GO!</color>");
            GUI.Label(new Rect((Screen.width / 2) - 50, Screen.height / 2, 100, 30), "<color=yellow>GO!</color>");
        }
                
        if (winner != 0)
        {
            GUI.Label(new Rect((Screen.width / 2) - 248, Screen.height / 2 + 2, 500, 50), "<color=black>Player " + winner + " has won!</color>");
            GUI.Label(new Rect((Screen.width / 2) - 250, Screen.height / 2, 500, 50), "<color=white>Player " + winner + " has won!</color>");
        }
    }

    void Update()
    {
        //end of game check
        if (!gameOver)
        {
            winner = gameMode.checkForWinner();

            if (winner != 0)
            {
                endOfGame();
                gameOver = true;
            }
        }
    }

    public void playerHit(PlayerControl playerControl)
    {
        playerControl.enabled = false;
        
        foreach (GameObject obj in fadingColorObjects)
            obj.SendMessage("setFade", true);

        Camera.main.gameObject.SendMessage("setFade", true);

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
            {
                obj.SendMessage("setFade", false);
            }

            mainCamera.resetZoom();
            gameMode.restartRound();

            for (int i = 0; i < players.Length; i++)
            {
                respawnPlayer(i);
            }
        }
    }

    private IEnumerator respawnPlayerDelayed(PlayerControl playerControl, float time)
    {
        int i = System.Array.IndexOf(playerControls, playerControl);

        yield return new WaitForSeconds(time);

        respawnPlayer(i);
    }

    private void respawnPlayer(int i)
    {
        //don't respawn if stock game and no stocks
        //so only respawn if more than 1 stock or not playing stock game
        if (playerControls[i].getScore() > 0 || stockGame.enabled == false)
        {
            players[i].transform.position = spawns[i].position;
            playerControls[i].enabled = true;
            playerControls[i].respawn();
        }
    }
    
    private void endOfGame()
    {
        //play sounds

        //zoom in on winner if it's a koth game
        if (kothGame.enabled == true)
            mainCamera.zoomInOnWinner(players[winner-1].transform);

        Invoke("reloadLevel", 0.8f);
    }

    private void reloadLevel()
    {
        foreach (GameObject player in players)
        {
            Destroy(player);
        }

        //set all gamemodes to false
        stockGame.enabled = false;

        kothGame.deactivateHills();
        kothGame.enabled = false;

        raceGame.enabled = false;

        mainCamera.start = false;
        
        winner = 0;

        GameObject.Find("Menu Manager").GetComponent<MainMenu>().resetGame();

        Camera.main.gameObject.SendMessage("setFade", false);

        foreach (GameObject obj in fadingColorObjects)
        {
            obj.SendMessage("setFade", false);
        }
    }
}
