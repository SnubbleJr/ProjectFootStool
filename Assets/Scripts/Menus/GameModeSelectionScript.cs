using UnityEngine;
using System.Collections;

public class GameModeSelectionScript : MonoBehaviour {

    //used to select what gamemode to play

    private PlayerManagerBehaviour playerManager;
    private GameModeSelectionScriptManager gameModeSelectionManager;

    private GameMode[] gameModes = { GameMode.Stock, GameMode.Koth, GameMode.Race };
    private int gameMode = 0;
    private int stocks = 4;
    private int prevGameMode;
    private int prevStocks;

    private int playerNo;
    private string horizontalAxis = "Horizontal";
    private string verticallAxis = "Vertical";
    private string modeSwitchKey = "SwitchMode";
    private string submitKey = "Submit";
    private string cancelKey = "Cancel";

    private bool hitRight = false;      //used so moving the horizontal axis only moves one tile
    private bool hitLeft = false;
    private bool hitUp = false;
    private bool hitDown = false;

    private bool active = false;
    private bool switchLetGo = false;               //used so we don't immediately quit because the switch key was already held down
    private bool stockSelected = false;             //used to differ between when stock is being changed and gamemode

	// Use this for initialization
    void Start()
    {
        playerManager = GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>();

        if (playerManager == null)
        {
            UnityEngine.Debug.LogError("Player manager has not been set!");
            this.enabled = false;
        }

        gameModeSelectionManager = GetComponent<GameModeSelectionScriptManager>();
	}

    void OnGUI()
    {
        GUI.skin = playerManager.skin;

        string modeColor;
        string stockColor;

        string modeTopBotColor = "#ffffff88"; ;
        string stockTopBotColor = "#ffffff88"; ;

        string modeTextTop;
        string modeTextBot;
        string stockTextTop;
        string stockTextBot;

        string modeSelectionTextR;

        if (!active)
        {
            modeColor = "yellow";
            stockColor = modeColor;
            modeSelectionTextR = ": ";

            modeTextTop = "";
            modeTextBot = "";
            stockTextTop = "";
            stockTextBot = "";
        }
        else
        {
            if (stockSelected)
            {
                modeColor = "yellow";
                stockColor = "white";      

                modeSelectionTextR = ": ";

                modeTextTop = "";
                modeTextBot = "";
                stockTextTop = "  " + getNextStock(-1).ToString();
                stockTextBot = getNextStock(1).ToString();
            }
            else
            {
                modeColor = "white";
                stockColor = "yellow";                             

                modeSelectionTextR = " ";

                modeTextTop = "  " + System.Enum.GetName(typeof(GameMode), getNextGameMode(-1));
                modeTextBot = "" + System.Enum.GetName(typeof(GameMode), getNextGameMode(1));
                stockTextTop = "";
                stockTextBot = "";
            }
        }

        string textStringBG = "<size=30><color=black>" + System.Enum.GetName(typeof(GameMode), gameMode) + "s" + modeSelectionTextR + stocks.ToString() + "</color></size>";
        string textStringFG = "<size=30><color=" + modeColor + ">" + System.Enum.GetName(typeof(GameMode), gameMode) + "s" + modeSelectionTextR + "</color><color=" + stockColor + ">" + stocks.ToString() + "</color></size>";

        //selectors gui part
        GUI.Label(new Rect(((Screen.width / 8) * 3) - 248, Screen.height / 2 - 28, 450, 30), "<size=30><color=#00000088>" + modeTextTop + "           " + stockTextTop + "</color></size>");
        GUI.Label(new Rect(((Screen.width / 8) * 3) - 250, Screen.height / 2 - 30, 450, 30), "<size=30><color=" + modeTopBotColor + ">" + modeTextTop + "            " + "</color><color=" + stockTopBotColor + ">" + stockTextTop + "</color></size>");

        GUI.Label(new Rect(((Screen.width / 8) * 3) - 248, Screen.height / 2 + 32, 450, 30), "<size=30><color=#00000088>" + modeTextBot + "           " + stockTextBot + "</color></size>");
        GUI.Label(new Rect(((Screen.width / 8) * 3) - 250, Screen.height / 2 + 30, 450, 30), "<size=30><color=" + modeTopBotColor + ">" + modeTextBot + "            " + "</color><color=" + stockTopBotColor + ">" + stockTextBot + "</color></size>");

        //actual content
        GUI.Label(new Rect(((Screen.width / 8)*3) - 248, Screen.height / 2 + 2, 500, 30), textStringBG);
        GUI.Label(new Rect(((Screen.width / 8) * 3) - 250, Screen.height / 2, 500, 30), textStringFG);
    }

	// Update is called once per frame
	void Update () 
    {
	    if (active)
        {           
            if (Input.GetButtonUp(modeSwitchKey))
                switchLetGo = true;

            inputCheck();

            if (Input.GetButtonDown(submitKey) || (Input.GetButtonDown(modeSwitchKey) && switchLetGo))
                confirmChanges();
            if (Input.GetButtonDown(cancelKey))
                cancelChanges();
        }
	}

    private void inputCheck()
    {
        float v = Input.GetAxisRaw(verticallAxis);

        if (v < 0)
        {
            if (!hitUp)
            {
                hitUp = true;
                changeValue(1);
            }
        }
        else
            hitUp = false;

        if (v > 0)
        {
            if (!hitDown)
            {
                hitDown = true;
                changeValue(-1);
            }
        }
        else
            hitDown = false;

        float h = Input.GetAxisRaw(horizontalAxis);

        if (h < 0)
        {
            if (!hitLeft)
            {
                hitLeft = true;
                moveFocus();
            }
        }
        else
            hitLeft = false;

        if (h > 0)
        {
            if (!hitRight)
            {
                hitRight = true;
                moveFocus();
            }
        }
        else
            hitRight = false;        
    }

    private void moveFocus()
    {
        stockSelected = !stockSelected;
    }

    private void changeValue(int direction)
    {
        if (stockSelected)
        {
            stocks = getNextStock(direction);
        }
        else
        {
            gameMode = getNextGameMode(direction);
        }
    }

    private int getNextGameMode(int direction)
    {
        int mode = gameMode - direction;

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
            stock = 1;

        if (stock >= 99)
            stock = 99;

        return stock;
    }

    private void cancelChanges()
    {
        gameMode = prevGameMode;
        stocks = prevStocks;

        confirmChanges();
    }

    private void confirmChanges()
    {
        UnsetPlayer();
        gameModeSelectionManager.gameModeSelectorFinished(playerNo);
    }

    private void UnsetPlayer()
    {
        active = false;
        
        horizontalAxis = "Horizontal";
        verticallAxis = "Vertical";
        modeSwitchKey = "SwitchMode";
        submitKey = "Submit";
        cancelKey = "Cancel";

        switchLetGo = false;
    }

    public void setPlayer(int playerNumber)
    {
        playerNo = playerNumber;

        //set up the inputs for this player
        horizontalAxis = "P" + playerNumber + horizontalAxis;
        verticallAxis = "P" + playerNumber + verticallAxis;
        modeSwitchKey = "P" + playerNumber + modeSwitchKey;
        submitKey = "P" + playerNumber + submitKey;
        cancelKey = "P" + playerNumber + cancelKey;

        prevGameMode = gameMode;
        prevStocks = stocks;

        active = true;
        switchLetGo = false;
    }

    public GameMode getGameMode()
    {
        return gameModes[gameMode];
    }

    public int getStockCount()
    {
        return stocks;
    }

    public bool getActive()
    {
        return active;
    }
}
