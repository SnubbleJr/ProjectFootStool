using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public bool playPlayerMusic = false;
    public bool playGameMusic = false;

    private PlayerManagerBehaviour playerManager;
    private PlayerFollower playerFollower;
    private PauseMenuScript pauseMenue;
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
        pauseMenue = GetComponent<PauseMenuScript>();
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
    
    public void startGame()
    {
        inGame = true;
        playerManager.enabled = true;
        playerFollower.enabled = true;
        
        //game mode checking here
        GameMode gameMode = playerSelectionMenu.GetGameMode();
        int scoreCount = playerSelectionMenu.getStockCount();

        if (playGameMusic)
        {
            //play gamemode music
            musicComponent.setMusic(gameMode);
            musicComponent.playMusic();
        }

        levelManager.setLevel(gameMode);

        Player[] players = playerSelectionMenu.getPlayers();

        //disable player menu scipt
        playerSelectionMenu.setScript(false);
        playerSelectionMenu.enabled = false;

        playerManager.startGame(players, gameMode, scoreCount);
        Camera.main.transform.rotation = Quaternion.identity;
        pauseMenue.enabled = true;
    }

    public void exitLevel()
    {
        inGame = false;
        playerManager.enabled = false;
        playerFollower.enabled = false;
        pauseMenue.enabled = false;

        displayPlayerMenu();
        levelManager.resetLevels();
    }

    public void setGameMode(GameMode gameMode)
    {
        //starts up a player menu, with the given game mode
        displayPlayerMenu();
        playerSelectionMenu.setGameMode(gameMode);
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
