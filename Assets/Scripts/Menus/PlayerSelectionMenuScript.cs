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
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (checkReady())
                mainMenu.startGame();
        }
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
}
