using UnityEngine;
using System.Collections;

public class GameModeSelectionScript : MonoBehaviour {

    //used to select what gamemode to play

    public GameModeSelecterOption teamOption, gameModeOption, gameModeValueOption;

    private GameModeSelectionScriptManager gameModeSelectionManager;

    private GameMode[] gameModes = { GameMode.Stock, GameMode.Race, GameMode.Koth };
    private int[] stocks = { 4, 4, 8 };
    private int currentGameModeIndex = 0;
    private bool teamMode = false;
    private int prevGameMode;
    private int[] prevStocks;
    private bool prevTeam;
    
    private int playerNo;
    private string modeSwitchKey;

    private bool exit = false;          //used to exit next frame so we don't get double input register

    private bool active = false;
    private bool switchLetGo = false;               //used so we don't immediately quit because the switch key was already held down

    private enum optionSelected {team, gameMode, gameModeValue}

    private optionSelected currentOption = optionSelected.team;

	// Use this for initialization
    void Start()
    {
        gameModeSelectionManager = GetComponent<GameModeSelectionScriptManager>();

        prevStocks = new int [stocks.Length];

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
        //clear every thing
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);
        updateGameModeValue();

        //display to clear things
        updateOptions();
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonDetected;
        InputManagerBehaviour.playerRemoved -= playerRemoved;

        //clear every thing
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
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
                            changeOption(-1);
                        if (value > 0)
                            changeOption(1);
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
        gameModeOption.setValueText("<color=" + selectedColor + ">" + System.Enum.GetName(typeof(GameMode), gameModes[currentGameModeIndex]) + "s" + "</color>", "");
        gameModeValueOption.setValueText("<color=" + selectedColor + ">" + stocks[currentGameModeIndex] + "</color>", "");

        if (active)
        {
            switch (currentOption)
            {
                case optionSelected.team:
                    teamOption.setValueText("<color=" + unSelectedColor + ">" + (!teamMode ? "Yes" : "No") + "</color>");
                    teamOption.setValueText("<color=" + highlightedColor + ">" + (teamMode ? "Yes" : "No") + "</color>", "<color=" + unSelectedColor + ">" + (!teamMode ? "Yes" : "No") + "</color>");
                    break;
                case optionSelected.gameMode:
                    gameModeOption.setValueText("<color=" + unSelectedColor + ">" + System.Enum.GetName(typeof(GameMode), getNextGameMode(-1)) + "</color>");
                    gameModeOption.setValueText("<color=" + highlightedColor + ">" + System.Enum.GetName(typeof(GameMode), gameModes[currentGameModeIndex]) + "s" + "</color>", "<color=" + unSelectedColor + ">" + System.Enum.GetName(typeof(GameMode), getNextGameMode(1)) + "</color>");
                    break;
                case optionSelected.gameModeValue:
                    gameModeValueOption.setValueText("<color=" + unSelectedColor + ">" + getNextStock(-1) + "</color>");
                    gameModeValueOption.setValueText("<color=" + highlightedColor + ">" + stocks[currentGameModeIndex] + "</color>", "<color=" + unSelectedColor + ">" + getNextStock(1) + "</color>");
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
                currentGameModeIndex = getNextGameMode(direction);
                updateGameModeValue();
                break;
            case optionSelected.gameModeValue:
                stocks[currentGameModeIndex] = getNextStock(direction);
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
        int mode = currentGameModeIndex + direction;

        if (mode < 0)
            mode = gameModes.Length - 1;

        if (mode > gameModes.Length - 1)
            mode = 0;
        
        return mode;
    }

    private int getNextStock(int direction)
    {
        int stock = stocks[currentGameModeIndex] + direction;

        if (stock <= 0)
            stock = 1;

        if (stock >= 100)
            stock = 99;

        return stock;
    }

    private void updateGameModeValue()
    {
        //set correct text for game mode
        string optionText;
        switch(gameModes[currentGameModeIndex])
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
        GameObject.Find("Music Visualiser").SendMessage("setGameMode", gameModes[currentGameModeIndex]);
    }

    private void cancelChanges()
    {
        //reset values
        currentGameModeIndex = prevGameMode;

        for (int i = 0; i < prevStocks.Length; i++)
            stocks[i] = prevStocks[i];

        teamMode = prevTeam;

        //update visuliser to new gamemode
        GameObject.Find("Music Visualiser").SendMessage("setGameMode", gameModes[currentGameModeIndex]);

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

        //log old values
        prevGameMode = currentGameModeIndex;

        for (int i = 0; i < stocks.Length; i++)
            prevStocks[i] = stocks[i];

        prevTeam = teamMode;

        active = true;
        switchLetGo = false;

        updateOptions();
    }

    public GameMode getGameMode()
    {
        return gameModes[currentGameModeIndex];
    }

    public void setGameMode(GameMode gMode)
    {
        currentGameModeIndex = (int)gMode;
        updateOptions();
    }

    public int getStockCount()
    {
        return stocks[currentGameModeIndex];
    }

    public void setStockCount(int count)
    {
        stocks[currentGameModeIndex] = count;
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
