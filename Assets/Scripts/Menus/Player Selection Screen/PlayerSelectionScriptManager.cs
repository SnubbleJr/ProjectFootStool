using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//makes player selection objects and gets each one's spreite and color
//passes it up to player selection menu

[System.Serializable]
public class PlayerSprite
{
    public Sprite sprite;
    public string name;

    private bool available;

    public void setAvailable(bool value)
    {
        available = value;
    }

    public bool getAvailable()
    {
        return available;
    }
}

[System.Serializable]
public class PlayerColor
{
    public Color color;
    public string name;

    private bool available;

    public void setAvailable(bool value)
    {
        available = value;
    }

    public bool getAvailable()
    {
        return available;
    }
}

public class Player
{
    public int playerNo;
    public int teamNo;
    public PlayerSprite sprite;
    public PlayerColor color;
    public Dictionary<PlayerInput, InputAxisAndButtons> inputs;
}

public class PlayerSelectionScriptManager : MonoBehaviour 
{
    public PlayerSprite[] playerSprites;    //depricated
    public PlayerColor[] playerColors;      //depricated

    public Sprite[] sprites;
    public Color[] colors;

    public GameObject playerSelecterPrefab;

    public GameObject pressToJoinCanvas;

    private PlayerSprite[] pSprites;
    private PlayerColor[] pColors;

    private SortedDictionary<int, PlayerSelectionScript> playerSelecters = new SortedDictionary<int, PlayerSelectionScript>();
    private Dictionary<PlayerSelectionScript, GameObject> joinCanvases = new Dictionary<PlayerSelectionScript, GameObject>();   //maps join image to a full controller
    private bool team = false;

    private List<PlayerInputScheme> activePlayers = new List<PlayerInputScheme>();

	// Use this for initialization
	void Awake ()
    {
        //build the private arrays
        
        List<PlayerSprite> pss = new List<PlayerSprite>();
        foreach(Sprite sprite in sprites)
        {
            PlayerSprite ps = new PlayerSprite();
            ps.sprite = sprite;
            pss.Add(ps);
        }
        pSprites = pss.ToArray();

        List<PlayerColor> pcs = new List<PlayerColor>();
        foreach(Color color in colors)
        {
            PlayerColor pc = new PlayerColor();
            pc.color = color;
            pcs.Add(pc);
        }
        pColors = pcs.ToArray();

	    //set all things to available
        foreach (PlayerSprite playerSprite in pSprites)
            playerSprite.setAvailable(true);

        foreach (PlayerColor playerColor in pColors)
            playerColor.setAvailable(true);

        //create player selectors for current inputs
        createPlayerSelecters();

        setTeamMode(false);
	}
	
    void OnEnable()
    {
        InputManagerBehaviour.playerAdded += createPlayerSelecter;
        InputManagerBehaviour.playerRemoved += deletePlayerSelecter;
    }

    void OnDisable()
    {
        InputManagerBehaviour.playerAdded -= createPlayerSelecter;
        InputManagerBehaviour.playerRemoved -= deletePlayerSelecter;
    }

    private void createPlayerSelecters()
    {
        List<PlayerInputScheme> playerInputs = InputManagerBehaviour.Instance.getInputs();

        foreach (PlayerInputScheme scheme in playerInputs)
            createPlayerSelecter(scheme);
    }

    private void updatePlayerSelectersPos()
    {
        //rearranges player selctors based on their schema id
        //also scales them

        float xOffset;
        float baseXOffset = 0.05f;
        float maxXOffset = 0.15f;
        float deltaXOffset = 0;

        float yOffset;
        float baseYOffset = 0.95f;
        float maxYOffset = 0.05f;
        float deltaYOffset = 0;

        List<List<GameObject>> rows = generateRows();

        if (rows.Count > 1)
        {
            //calculate the how much each row needs to be xOffset and yOffset by
            deltaXOffset = (maxXOffset - baseXOffset) / (rows.Count - 1);
            deltaYOffset = (baseYOffset - maxYOffset) / (rows.Count - 1);
        }

        for (int i = 0; i < rows.Count; i++)
        {
            //this is basically a foreach loop on each colum, but we want to know it's index

            List<GameObject> row = rows[i];

            //NOTE yOffset is from top down

            //for yOffset, it's
            //95% - (delta% * row number)

            yOffset = baseYOffset - (deltaYOffset * i);

            for (int j = 0; j < rows[i].Count; j++)
            {
                //same as above but for each game object in the row

                //if even, then on right side, else left
                //NOTE: highest xOffset for each row is at the start
                //ALSO NOTE: base and max offset refers to the offset of the colum

                //delta% being the deference in xOffset betweenn each row

                //left side
                //5%(base xOffset) + 0-10%(15% total being max)
                //15% - (delta% * col number)

                //right side
                //85%(base xOffset) - 0-10%(95% total being max)
                //95% - (delta% * col number)

                GameObject gO = row[j];

                //check if even or odd
                if (j % 2 == 0)
                    xOffset = maxXOffset;
                else
                    xOffset = 1 - baseXOffset;

                xOffset -= deltaXOffset * i;

                Vector3 pos = Vector3.zero;
                Camera viewingCamera = GameObject.FindGameObjectWithTag("ViewingCamera").GetComponent<Camera>();

                //apply new position as a scale of screen width and height
                pos = viewingCamera.ScreenToWorldPoint(new Vector3(Screen.width * xOffset, Screen.height * yOffset, 10));
                //scaling position based on count
                pos *= (1f - (1f / (rows.Count + 1)));
                pos.x++;
                //removing z component
                pos.z = transform.position.z;
                gO.transform.position = pos;

                float scale = 2f / (rows.Count + 1);

                if (!float.IsInfinity(scale))
                    gO.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }

    private List<List<GameObject>> generateRows()
    {
        //build the rows
        List<List<GameObject>> rows = new List<List<GameObject>>();
        List<GameObject> row = new List<GameObject>();

        int colCount = 2;

        int rowCounter = 0;         //used to deal with each row
        bool newRow = false;        //indicates when we start a new row

        foreach(KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
        {
            PlayerSelectionScript playerSelecter = pair.Value;

            //if row is finished, then add it to the rows
            if (newRow)
            {
                rows.Add(row);
                row = new List<GameObject>();
                newRow = false;
            }

            //add selecter to current row, increase counter;
            row.Add(playerSelecter.gameObject);
            rowCounter++;

            //if controller full, add it to the row so we count it as a game object to move
            if (playerSelecter.getPlayer().inputType == InputType.ControllerFull)
            {
                row.Add(joinCanvases[playerSelecter]);
                rowCounter++;
            }


            if (rowCounter >= colCount)
            {
                rowCounter = 0;
                newRow = true;
            }
        }

        //add any half finished rows
        if (row.Count > 0)
            rows.Add(row);

        return rows;
    }

    private void createPlayerSelecter(PlayerInputScheme playerInputScheme)
    {
        GameObject playerSelecter = Instantiate(playerSelecterPrefab, transform.position, Quaternion.identity) as GameObject;
        playerSelecter.transform.parent = transform;

        PlayerSelectionScript playerSelecterScript = playerSelecter.GetComponent<PlayerSelectionScript>();

        playerSelecterScript.setPlayer(playerInputScheme);

        playerSelecters.Add(playerInputScheme.id, playerSelecterScript);
                        
        //if controller full, we add a join instruction for the other side to dictionary
        if (playerInputScheme.inputType == InputType.ControllerFull)
        {
            GameObject joinCanvas = Instantiate(pressToJoinCanvas, transform.position, Quaternion.identity) as GameObject;
            joinCanvas.transform.SetParent(transform, false);
            joinCanvases.Add(playerSelecterScript, joinCanvas);
        }

        updatePlayerNos();

        updatePlayerSelectersPos();
    }

    private void deletePlayerSelecter(PlayerInputScheme playerInputScheme)
    {
        //find the player selector that corresponds to the input's id, and deletor

        //destroy linked join image if a full controller
        if (playerInputScheme.inputType == InputType.ControllerFull)
        {
            Destroy(joinCanvases[playerSelecters[playerInputScheme.id]]);
            joinCanvases.Remove(playerSelecters[playerInputScheme.id]);
        }
        
        Destroy(playerSelecters[playerInputScheme.id].gameObject);
        playerSelecters.Remove(playerInputScheme.id);

        updatePlayerNos();

        updatePlayerSelectersPos();
    }

    private void updatePlayerNos()
    {
        //update player selecters playerNo based on the order it is in the dict
        int i = 1;
        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
        {
            pair.Value.setPlayerNo(i++);
        }
    }

    public void updateTeamNos()
    {
        //if a team game, then apply a rudimentry team difference, half one team, half the other
        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
            pair.Value.setTeam(team);
    }

    public Player[] getReadyPlayers()
    {
        //returns an array of all the players active and ready for a game

        //returns an array of all the players for a round
        List<Player> players = new List<Player>();

        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
        {
            PlayerSelectionScript playerSelectionScript = pair.Value;

            if (playerSelectionScript.getActive() && playerSelectionScript.getReady())
            {
                Player player = new Player();

                player.sprite = playerSelectionScript.getSprite();
                player.color = playerSelectionScript.getColor();
                player.playerNo = playerSelectionScript.getPlayerNo();
                player.teamNo = playerSelectionScript.getTeam();
                player.inputs = playerSelectionScript.getPlayer().inputs;
                players.Add(player);
            }
        }

        return players.ToArray();
    }

    public Player[] getActivePlayers()
    {
        //returns an array of all the players active

        //returns an array of all the players for a round
        List<Player> players = new List<Player>();

        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
        {
            PlayerSelectionScript playerSelectionScript = pair.Value;
            if (playerSelectionScript.getActive())
            {
                Player player = new Player();

                player.sprite = playerSelectionScript.getSprite();
                player.color = playerSelectionScript.getColor();
                player.playerNo = playerSelectionScript.getPlayerNo();
                player.teamNo = playerSelectionScript.getTeam();
                player.inputs = playerSelectionScript.getPlayer().inputs;
                players.Add(player);
            }
        }

        return players.ToArray();
    }

    public PlayerSelectionScript[] getPlayers()
    {
        //debug, but jsut returns players we have created
        List<PlayerSelectionScript> players = new List<PlayerSelectionScript>();

        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
           players.Add(pair.Value);
        return players.ToArray();
    }

    public void setPlayers()
    {
        //debug overload that ready's everyone up
        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
        {
            PlayerSelectionScript playerSelectionScript = pair.Value;
            playerSelectionScript.setActive(true);
            playerSelectionScript.setReady(true);
        }
    }

    public void setScript(bool value)
    {
        //setActive to every selecter under it, as we can't disable this game object
        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
        {
            PlayerSelectionScript playerSelectionScript = pair.Value;
            playerSelectionScript.gameObject.SetActive(value);

            if (joinCanvases.ContainsKey(pair.Value))
                joinCanvases[pair.Value].SetActive(value);
        }
    }

    public bool checkReady()
    {
        //returns true only if all active players are ready, and more than 2 players are ready
        bool ready = true;

        foreach(KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
        {
            PlayerSelectionScript playerSelectionScript = pair.Value;
            if (playerSelectionScript.getActive() && !playerSelectionScript.getReady())
                ready = false;
        }

        Player[] readyPlayers = getReadyPlayers();

        if (readyPlayers.Length < 2)
            ready = false;

        return ready;
    }

    public void setSelectorToGrey(int id, bool status)
    {
        //set a selector to be greyed out and inactive or go back to normal
        playerSelecters[id].setGrey(status);
    }

    public bool getSelectorActive(int id)
    {
        //returns weither or not the selector is active
        return playerSelecters[id].getActive();
    }

    public bool getSelectorReady(int id)
    {
        //returns weither or not the selector is active
        return playerSelecters[id].getReady();
    }

    public bool getTeamMode()
    {
        return team;
    }

    public void setTeamMode(bool val)
    {
        team = val;
        updateTeamNos();
    }

    public PlayerSprite[] getPlayerSprites()
    {
        return pSprites;
    }

    public PlayerColor[] getPlayerColors()
    {
        return pColors;
    }
}
