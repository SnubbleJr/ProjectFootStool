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

    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonPressed;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonPressed;
    }

    private void buttonPressed(PlayerInputScheme player, string input, float value)
    {
        //quit out if no one is in selecter
        if (input == "ChangeMode")
            activateSelecter(player);
    }

    private void activateSelecter(PlayerInputScheme player)
    {
        int playerNo = player.id;

        //activate only if someone isn't already in there, and if that person is currently in game and not ready
        if (!gameModeSelector.getActive() && playerSelectorManger.getSelectorActive(playerNo) &! playerSelectorManger.getSelectorReady(playerNo))
        {
            //tell manager to disable that player selector the time being
            //wake up game mode slector, and give it the pplayer it should be listening to
            playerSelectorManger.setSelectorToGrey(playerNo, true);
            gameModeSelector.setPlayer(player);
        }
    }

    public void gameModeSelectorFinished(int playerNo)
    {
        //wait for the game mode sleector to tell us it is finished
        //tell the manager to free up the sleector again
        //try beacuse we acutomatically cancle on a controller change
        try
        {
            playerSelectorManger.setSelectorToGrey(playerNo, false);
        }
        catch
        {
        }
        //update teammode
        playerSelectorManger.setTeamMode(gameModeSelector.getTeamMode());
    }

    public void setScript(bool value)
    {
        if (gameModeSelector == null)
            gameModeSelector = GetComponent<GameModeSelectionScript>();

        gameModeSelector.enabled = value;
    }

    public int getStockCount()
    {
        return gameModeSelector.getStockCount();
    }

    public GameMode GetGameMode()
    {
        return gameModeSelector.getGameMode();
    }

    public void setGameMode(GameMode gameMode)
    {
        gameModeSelector.setGameMode(gameMode);
    }

    public void setStockCount(int count)
    {
        gameModeSelector.setStockCount(count);
    }

    public void setTeamMode(bool mode)
    {
        gameModeSelector.setTeamMode(mode);
    }
}
