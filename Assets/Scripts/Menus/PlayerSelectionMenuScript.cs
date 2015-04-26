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
    private MainMenu mainMenu;

    private bool allReady = false;

	// Use this for initialization
	void Awake ()
    {
        playerManager = GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>();
        mainMenu = GetComponent<MainMenu>();

        if (playerManager == null)
        {
            UnityEngine.Debug.LogError("Player manager has not been set!");
            this.enabled = false;
        }
	}
	
    void OnGUI()
    {
        GUI.skin = playerManager.skin;
        GUI.skin.box.padding.left = ((Screen.width / 8) * 3) -150;

        if(allReady)
            GUI.Box(new Rect(-30, Screen.height / 2 - 8, Screen.width + 60, 50), "<size=30><color=white>PRESS START (SPACE)</color></size>");
    }

	// Update is called once per frame
	void Update () 
    {
        //quit out if no one is in selecter
	    if (Input.GetButtonDown("Cancel") && getPlayers().Length == 0)
        {
            playerSelectionManager.SetActive(false);
            this.enabled = false;

            //tell main menu that we've quit
            mainMenu.hidePlayerMenu();
        }

        allReady = checkReady();

        if (Input.GetButtonDown("StartGame") && allReady)
            mainMenu.startGame();
	}

    private bool checkReady()
    {
        //check to see if everyone is ready
        return playerSelecterManager.checkReady();
    }

    public Player[] getPlayers()
    {
        return playerSelecterManager.getPlayers();
    }

    public GameMode GetGameMode()
    {
        if (gameModeSelecterManager == null)
            gameModeSelecterManager = playerSelectionManager.GetComponent<GameModeSelectionScriptManager>();

        return gameModeSelecterManager.GetGameMode();
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
