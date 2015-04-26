using UnityEngine;
using System.Collections;

public class GameModeSelectionScriptManager : MonoBehaviour {

    //waits for input, and tells the player selection manager to disable the current selector until finished

    private PlayerSelectionScriptManager playerSelectorManger;
    private GameModeSelectionScript gameModeSelector;
    
	// Use this for initialization
	void Start () 
    {
        playerSelectorManger = GetComponent<PlayerSelectionScriptManager>();
        gameModeSelector = GetComponent<GameModeSelectionScript>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //wait for anyone to try and change mode
        if (Input.GetButtonDown("P1SwitchMode"))
            activateSelecter(1);
        if (Input.GetButtonDown("P2SwitchMode"))
            activateSelecter(2);
        if (Input.GetButtonDown("P3SwitchMode"))
            activateSelecter(3);
        if (Input.GetButtonDown("P4SwitchMode"))
            activateSelecter(4);
	}

    private void activateSelecter(int playerNo)
    {
        //activate only if someone isn't already in there, and if that person is currently in game and not ready
        if (!gameModeSelector.getActive() && playerSelectorManger.getSelectorActive(playerNo) &! playerSelectorManger.getSelectorReady(playerNo))
        {
            //tell manager to disable that player selector the time being
            //wake up game mode slector, and give it the pplayer it should be listening to
            playerSelectorManger.setSelectorToGrey(playerNo, true);
            gameModeSelector.setPlayer(playerNo);
        }
    }

    public void gameModeSelectorFinished(int playerNo)
    {
        //wait for the game mode sleector to tell us it is finished
        //tell the manager to free up the sleector again
        playerSelectorManger.setSelectorToGrey(playerNo, false);
    }

    public void setScript(bool value)
    {
        if (gameModeSelector == null)
            gameModeSelector = GetComponent<GameModeSelectionScript>();

        gameModeSelector.enabled = value;
    }

    public GameMode GetGameMode()
    {
        return gameModeSelector.getGameMode();
    }

    public int getStockCount()
    {
        return gameModeSelector.getStockCount();
    }

    public void setGameMode(GameMode gameMode)
    {
        gameModeSelector.setGameMode(gameMode);
    }
}
