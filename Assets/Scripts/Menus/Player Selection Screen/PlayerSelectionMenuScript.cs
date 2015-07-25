using UnityEngine;
using System.Collections;

//activated player selection script amanger
//also worries about the current game mode
//talks to main menu

public class PlayerSelectionMenuScript : MonoBehaviour {

    public GameObject playerSelectionManager;

    private PlayerManagerBehaviour playerManager;
    private PlayerSelectionScriptManager playerSelecterManager;
    private GameModeSelectionScriptManager gameModeSelecterManager;
    private MainMenuScript mainMenu;

    private bool allReady = false;

	// Use this for initialization
	void Awake ()
    {
        playerManager = GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>();
        mainMenu = GetComponent<MainMenuScript>();

        if (playerManager == null)
        {
            UnityEngine.Debug.LogError("Player manager has not been set!");
            this.enabled = false;
        }
	}

    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonPressed;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonPressed;
    }

    void OnGUI()
    {
        GUI.skin = playerManager.skin;
        GUI.skin.box.padding.left = ((Screen.width / 8) * 3) -150;

        if(allReady)
            GUI.Box(new Rect(-30, Screen.height / 2 - 8, Screen.width + 60, 50), "<size=30><color=white>PRESS START (SPACE)</color></size>");
    }

    void Update()
    {
        allReady = checkReady();
    }

    private void buttonPressed(PlayerInputScheme player, string input, float value)
    {
        //quit out if no one is in selecter
        if (input == "Cancel" && getActivePlayers().Length == 0)
        {
            playerSelectionManager.SetActive(false);
            this.enabled = false;

            //tell main menu that we've quit
            mainMenu.hidePlayerMenu();
        }

        //don't let someone else start the game unless they're active
        bool validPlayer = false;
        Player[] readyPlayers = getReadyPlayers();
        foreach (Player readyRlayer in readyPlayers)
            if (readyRlayer.playerNo == player.id)
                validPlayer = true;

        if (input == "Submit" && allReady && validPlayer)
            mainMenu.startGame();
    }

    private bool checkReady()
    {
        //check to see if everyone is ready
        return playerSelecterManager.checkReady();
    }

    public Player[] getReadyPlayers()
    {
        return playerSelecterManager.getReadyPlayers();
    }

    public Player[] getActivePlayers()
    {
        return playerSelecterManager.getActivePlayers();
    }

    public GameMode GetGameMode()
    {
        if (gameModeSelecterManager == null)
            gameModeSelecterManager = playerSelectionManager.GetComponent<GameModeSelectionScriptManager>();

        return gameModeSelecterManager.GetGameMode();
    }

    public bool getTeamMode()
    {
        return playerSelecterManager.getTeamMode();
    }

    public int getStockCount()
    {
        if (gameModeSelecterManager == null)
            gameModeSelecterManager = playerSelectionManager.GetComponent<GameModeSelectionScriptManager>();
        
        return gameModeSelecterManager.getStockCount();
    }

    public void setScript(bool value)
    {
        playerSelectionManager.SetActive(true);

        if (playerSelecterManager == null)
            playerSelecterManager = playerSelectionManager.GetComponent<PlayerSelectionScriptManager>();
        playerSelecterManager.setScript(value);

        if (gameModeSelecterManager == null)
            gameModeSelecterManager = playerSelectionManager.GetComponent<GameModeSelectionScriptManager>();
        gameModeSelecterManager.setScript(value);
    }

    public void setGameMode(GameMode gameMode)
    {
        gameModeSelecterManager.setGameMode(gameMode);
    }
}
