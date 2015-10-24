using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

    public bool playPlayerMusic = false;

    private PlayerManagerBehaviour playerManager;
    private PlayerFollower playerFollower;
    private PauseMenuScript pauseMenu;
    private PlayerSelectionMenuScript playerSelectionMenu;
    private LevelManagerBehaviour levelManager;
    private MusicComponent musicComponent;

    private float cameraSize;
    private Vector3 cameraPos;
    private bool inGame = false;

    // Use this for initialization
	void Awake () 
    {
        cameraSize = Camera.main.orthographicSize;
        cameraPos = Camera.main.transform.position;
        playerManager = GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>();
        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManagerBehaviour>();
        playerFollower = Camera.main.GetComponent<PlayerFollower>();
        pauseMenu = GetComponent<PauseMenuScript>();
        playerSelectionMenu = GetComponent<PlayerSelectionMenuScript>();
        musicComponent = GetComponent<MusicComponent>();
	}
    
	// Update is called once per frame
	void Update () 
    {
        if (!inGame)
        {
            float zoomSpeed = playerFollower.zoomSpeed;

            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, cameraSize, zoomSpeed * Time.deltaTime);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPos, zoomSpeed * Time.deltaTime);
        }
	}

    private void displayPlayerMenu()
    {
        if (!playerSelectionMenu.enabled)
        {
            playerSelectionMenu.enabled = true;
            playerSelectionMenu.setScript(true);

            if (playPlayerMusic)
            {
                //play playermenu music
                musicComponent.setMusic(MusicTrack.PlayerSelectionMenu);
                musicComponent.playMusic();
            }
        }
    }
    
    public void playersChosen()
    {
        //disable player menu scipt
        playerSelectionMenu.setScript(false);
        playerSelectionMenu.enabled = false;

        displayLevelMenu();
    }

    private void backToPlayerMenu()
    {
        //open up the player menu again
        playerSelectionMenu.setScript(true);
        playerSelectionMenu.enabled = true;

        displayLevelMenu();
    }

    private void displayLevelMenu()
    {
        //wait on level menu
        //pass gameMode for level to manager
        levelManager.openMenu(playerSelectionMenu.GetGameMode());
    }

    private void hideLevelMenu()
    {
        levelManager.closeMenu();
    }

    public void levelChosen(int option)
    {
        hideLevelMenu();

        if (option < 0)
            backToPlayerMenu();
        else
            startGame();
    }

    public void startGame()
    {
        //gather components for game
        Player[] players = playerSelectionMenu.getReadyPlayers();
        GameMode gameMode = playerSelectionMenu.GetGameMode();
        int scoreCount = playerSelectionMenu.getStockCount();
        bool team = playerSelectionMenu.getTeamMode();

        //setting of level
        levelManager.startLevel();

        //starting of the game
        inGame = true;
        playerManager.enabled = true;
        playerFollower.enabled = true;

        playerManager.startGame(players, gameMode, team, scoreCount);
        Camera.main.transform.rotation = Quaternion.identity;
        pauseMenu.enabled = true;
    }

    public void endGame()
    {
        inGame = false;
        playerFollower.enabled = false;
        pauseMenu.enabled = false;
    }

    public void exitLevel()
    {
        playerManager.enabled = false;

        displayPlayerMenu();
        levelManager.resetLevels();
    }

    public void setGameMode(GameMode gameMode)
    {
        //starts up a player menu, with the given game mode
        displayPlayerMenu();
        playerSelectionMenu.setGameMode(gameMode);

        //update visuliser to new gamemode
        GameObject.Find("Music Visualiser").SendMessage("setGameMode", gameMode);
    }

    public void hidePlayerMenu()
    {
        //used just to inform us of a quit

        if (playPlayerMusic)
        {         
            //play menu music
            musicComponent.setMusic(MusicTrack.MainMenu);
            musicComponent.playMusic();
        }
    }
}
