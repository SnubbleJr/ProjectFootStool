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
            GUI.Box(new Rect(0, (Screen.height / 2), Screen.width, 50), "<size=30><color=white>PRESS START (SPACE)</color></size>");
    }

    void Update()
    {
        allReady = checkReady();
    }

    private void buttonPressed(PlayerInputScheme player, string input, float value)
    {
        //quit out if no one is in selecter
        if (input == player.inputs[PlayerInput.CancelInput].shortName && getActivePlayers().Length == 0)
        {
            playerSelectionManager.SetActive(false);
            this.enabled = false;

            //tell main menu that we've quit
            mainMenu.hidePlayerMenu();
        }

        //don't let someone else start the game unless they're active
        bool validPlayer = false;
        Player[] readyPlayers = getReadyPlayers();
        foreach (Player readyPlayer in readyPlayers)
            if (readyPlayer.playerInputScheme.id == player.id)
                validPlayer = true;

        if (input == player.inputs[PlayerInput.SubmitInput].shortName || input == player.inputs[PlayerInput.StartGameInput].shortName)
            if (allReady && validPlayer)
                mainMenu.playersChosen();
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

    public void setGameMode(GameMode gameMode)
    {
        gameModeSelecterManager.setGameMode(gameMode);
    }

    public bool getTeamMode()
    {
        return playerSelecterManager.getTeamMode();
    }

    public void setTeamMode(bool mode)
    {
        gameModeSelecterManager.setTeamMode(mode);
    }

    public int getStockCount()
    {
        if (gameModeSelecterManager == null)
            gameModeSelecterManager = playerSelectionManager.GetComponent<GameModeSelectionScriptManager>();
        
        return gameModeSelecterManager.getStockCount();
    }

    public void setStockCount(int count)
    {
        gameModeSelecterManager.setStockCount(count);
    }

    public PlayerSelectionScript[] getPlayers()
    {
        return playerSelecterManager.getPlayers();
    }

    public void setPlayers()
    {
        playerSelecterManager.setPlayers();
    }

    public void setScript(bool value)
    {
        playerSelectionManager.SetActive(true);

        if (playerSelecterManager == null)
            playerSelecterManager = playerSelectionManager.GetComponent<PlayerSelectionScriptManager>();
        playerSelecterManager.setScript(value);
        playerSelecterManager.enabled = value;

        if (gameModeSelecterManager == null)
            gameModeSelecterManager = playerSelectionManager.GetComponent<GameModeSelectionScriptManager>();
        gameModeSelecterManager.setScript(value);
    }
}
