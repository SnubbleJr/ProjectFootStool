using UnityEngine;
using System.Collections;

public class GameModeSelectionScript : MonoBehaviour {

    //used to select what gamemode to play

    public GameModeSelecterOption teamOption, gameModeOption, gameModeValueOption;

    private GameModeSelectionScriptManager gameModeSelectionManager;

    private GameMode[] gameModes = { GameMode.Stock, GameMode.Koth, GameMode.Race };
    private int currentGameMode = 0;
    private int stocks = 4;
    private bool teamMode = false;
    private int prevGameMode;
    private int prevStocks;
    private bool prevTeam;
    
    private int playerNo;
    private string modeSwitchKey;

    private bool exit = false;          //used to exit next frame so we don't get double input register

    private bool active = false;
    private bool switchLetGo = false;               //used so we don't immediately quit because the switch key was already held down
    private bool stockSelected = false;             //used to differ between when stock is being changed and gamemode

    private enum optionSelected {team, gameMode, gameModeValue}

    private optionSelected currentOption = optionSelected.team;

	// Use this for initialization
    void Start()
    {
        gameModeSelectionManager = GetComponent<GameModeSelectionScriptManager>();

        //display options
        teamOption.setOptionText("Teams");
        gameModeOption.setOptionText("GameMode");
        updateGameModeValue();

        //display to clear things
        updateOptions();
	}

    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonDetected;
        InputManagerBehaviour.playerRemoved += playerRemoved;

        //display options
        teamOption.setOptionText("Teams");
        gameModeOption.setOptionText("GameMode");
        updateGameModeValue();

        //display to clear things
        updateOptions();
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonDetected;
        InputManagerBehaviour.playerRemoved -= playerRemoved;

        //clear every thing
        teamOption.setOptionText("");
        teamOption.setValueText("");
        teamOption.setValueText("", "");

        gameModeOption.setOptionText("");
        gameModeOption.setValueText("");
        gameModeOption.setValueText("", "");

        gameModeValueOption.setOptionText("");
        gameModeValueOption.setValueText("");
        gameModeValueOption.setValueText("", "");
    }

    void buttonDetected(PlayerInputScheme player, string inputName, float value)
    {
        if (player.id == playerNo)
        {
            switch (inputName)
            {
                case "Submit":
                    confirmChanges();
                    break;
                case "ChangeMode":
                    if (switchLetGo)
                        cancelChanges();
                    break;
                case "Cancel":
                    cancelChanges();
                    break;
                case "Horizontal":
                        if (value < 0)
                            changeOption(1);
                        if (value > 0)
                            changeOption(-1);
                    break;
                case "Vertical":
                        if (value < 0)
                            changeValue(-1);
                        if (value > 0)
                            changeValue(1);
                    break;
            }
        }
    }

    private void playerRemoved(PlayerInputScheme player)
    {
        if (player.id == playerNo)
            cancelChanges();
    }

	// Update is called once per frame
	void Update () 
    {
	    if (active)
        {
            if (Input.GetButtonUp(modeSwitchKey))
                switchLetGo = true;
        }
        else if (exit)
        {
            exit = false;
            gameModeSelectionManager.gameModeSelectorFinished(playerNo);
            playerNo = -1;
        }
	}

    private void updateOptions()
    {
        string highlightedColor = "#ffff00ff";
        string selectedColor = "#ffffffff";
        string unSelectedColor = "#ffffff88";

        //clear every option first and then set coressponding option

        //remove options for each value
        //send blank text to privous value of each option
        teamOption.setValueText("");
        gameModeOption.setValueText("");
        gameModeValueOption.setValueText("");

        //send slected value (with selected color) and blank next value
        teamOption.setValueText("<color=" + selectedColor + ">" + (teamMode ? "Yes" : "No") + "</color>", "");
        gameModeOption.setValueText("<color=" + selectedColor + ">" + System.Enum.GetName(typeof(GameMode), currentGameMode) + "s" + "</color>", "");
        gameModeValueOption.setValueText("<color=" + selectedColor + ">" + stocks.ToString() + "</color>", "");

        if (active)
        {
            switch (currentOption)
            {
                case optionSelected.team:
                    teamOption.setValueText("<color=" + unSelectedColor + ">" + (!teamMode ? "Yes" : "No") + "</color>");
                    teamOption.setValueText("<color=" + highlightedColor + ">" + (teamMode ? "Yes" : "No") + "</color>", "<color=" + unSelectedColor + ">" + (!teamMode ? "Yes" : "No") + "</color>");
                    break;
                case optionSelected.gameMode:
                    gameModeOption.setValueText("<color=" + unSelectedColor + ">" + System.Enum.GetName(typeof(GameMode), getNextGameMode(1)) + "</color>");
                    gameModeOption.setValueText("<color=" + highlightedColor + ">" + System.Enum.GetName(typeof(GameMode), currentGameMode) + "s" + "</color>", "<color=" + unSelectedColor + ">" + System.Enum.GetName(typeof(GameMode), getNextGameMode(-1)) + "</color>");
                    break;
                case optionSelected.gameModeValue:
                    gameModeValueOption.setValueText("<color=" + unSelectedColor + ">" + getNextStock(1) + "</color>");
                    gameModeValueOption.setValueText("<color=" + highlightedColor + ">" + stocks + "</color>", "<color=" + unSelectedColor + ">" + getNextStock(-1) + "</color>");
                    break;
            }
        }
    }

    private void changeOption(int direction)
    {
        switch (currentOption)
        {
            case optionSelected.team:
                teamMode = !teamMode;
                break;
            case optionSelected.gameMode:
                currentGameMode = getNextGameMode(direction);
                updateGameModeValue();
                break;
            case optionSelected.gameModeValue:
                stocks = getNextStock(direction);
                break;
        }
        updateOptions();
    }

    private void changeValue(int direction)
    {
        switch (currentOption)
        {
            case optionSelected.team:
                if (direction > 0)
                    currentOption = optionSelected.gameModeValue;
                else
                    currentOption = optionSelected.gameMode;
                break;
            case optionSelected.gameMode:
                if (direction > 0)
                    currentOption = optionSelected.team;
                else
                    currentOption = optionSelected.gameModeValue;
                break;
            case optionSelected.gameModeValue:
                if (direction > 0)
                    currentOption = optionSelected.gameMode;
                else
                    currentOption = optionSelected.team;
                break;
        }
        updateOptions();
    }

    private int getNextGameMode(int direction)
    {
        int mode = currentGameMode - direction;

        if (mode < 0)
            mode = gameModes.Length - 1;

        if (mode > gameModes.Length - 1)
            mode = 0;
        
        return mode;
    }

    private int getNextStock(int direction)
    {
        int stock = stocks - direction;

        if (stock <= 0)
            stock = 0;

        if (stock >= 100)
            stock = 100;

        return stock;
    }

    private void updateGameModeValue()
    {
        //set correct text for game mode
        string optionText;
        switch(gameModes[currentGameMode])
        {
            case GameMode.Stock:
                optionText = "Lives";
                break;
            case GameMode.Race:
                optionText = "First to";
                break;
            case GameMode.Koth:
                optionText = "First to";
                break;
            default:
                optionText = "Score";
                break;
        }
        gameModeValueOption.setOptionText(optionText);

        //update visuliser to new gamemode
        GameObject.Find("Music Visualiser").SendMessage("setGameMode", currentGameMode);
    }

    private void cancelChanges()
    {
        currentGameMode = prevGameMode;
        stocks = prevStocks;
        teamMode = prevTeam;

        confirmChanges();
    }

    private void confirmChanges()
    {
        UnsetPlayer();
        updateOptions();
    }

    private void UnsetPlayer()
    {
        modeSwitchKey = "";

        active = false;
        exit = true;
        switchLetGo = false;
    }

    public void setPlayer(PlayerInputScheme player)
    {
        playerNo = player.id;

        //set up the inputs for this player
        modeSwitchKey = player.inputs[PlayerInput.ChangeModeInput].inputName;

        prevGameMode = currentGameMode;
        prevStocks = stocks;
        prevTeam = teamMode;

        active = true;
        switchLetGo = false;

        updateOptions();
    }

    public GameMode getGameMode()
    {
        return gameModes[currentGameMode];
    }

    public void setGameMode(GameMode gMode)
    {
        currentGameMode = (int)gMode;
    }

    public int getStockCount()
    {
        return stocks;
    }

    public void setStockCount(int count)
    {
        stocks = count;
    }

    public bool getTeamMode()
    {
        return teamMode;
    }

    public void setTeamMode(bool team)
    {
        teamMode = team;
    }

    public bool getActive()
    {
        return active;
    }
}
