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
    public PlayerInputScheme playerInputScheme;
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
    private PlayerColor[] activepColors;

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
            ps.setAvailable(true);
            ps.sprite = sprite;
            pss.Add(ps);
        }
        pSprites = pss.ToArray();

        List<PlayerColor> pcs = new List<PlayerColor>();
        foreach(Color color in colors)
        {
            PlayerColor pc = new PlayerColor();
            pc.setAvailable(true);
            pc.color = color;
            pcs.Add(pc);
        }
        pColors = pcs.ToArray();
        
        //create player selectors for current inputs
        createPlayerSelecters();

        setTeamMode(false);
	}
	
    void OnEnable()
    {
        InputManagerBehaviour.playerAdded += createPlayerSelecter;
        InputManagerBehaviour.playerRemoved += deletePlayerSelecter;
        InputManagerBehaviour.buttonPressed += buttonDetected;
        InputManagerBehaviour.axisPressed += axisDetected;
    }

    void OnDisable()
    {
        InputManagerBehaviour.playerAdded -= createPlayerSelecter;
        InputManagerBehaviour.playerRemoved -= deletePlayerSelecter;
        InputManagerBehaviour.buttonPressed -= buttonDetected;
        InputManagerBehaviour.axisPressed -= axisDetected;
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
            if (playerSelecter.getPlayerInputScheme().inputType == InputType.ControllerFull)
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
        updateTeamColors();
        updateTeamNos();

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

    public void buttonDetected(PlayerInputScheme player, string inputName, float value)
    {
        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
            if (player.id == pair.Value.getPlayerInputSchemeId())
                pair.Value.buttonDetected(player, inputName, value);
    }

    public void axisDetected(PlayerInputScheme player, string inputName, float value)
    {
        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
            if (player.id == pair.Value.getPlayerInputSchemeId())
                pair.Value.axisDetected(player, inputName, value);
    }

    private void updatePlayerNos()
    {
        //update player selecters playerNo based on the order it is in the dict
        int i = 1;
        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
            pair.Value.setPlayerNo(i++);
    }

    public void updateTeamNos()
    {
        //if a team game, then apply a rudimentry team difference, half one team, half the other
        foreach (KeyValuePair<int, PlayerSelectionScript> pair in playerSelecters)
            pair.Value.setTeam(team);
    }

    private void updateTeamColors()
    {
        //changes the amount of colors avaiable pased on the current team value
        if (team)
        {
            //update pColors availbility with whatever has been happening while active was short
            if (activepColors != null)
                for (int i = 0; i > activepColors.Length; i++)
                    pColors[9 * (i + 1)] = activepColors[i];

            activepColors = pColors;
        }
        else
            activepColors = makeShortColors();
    }

    private PlayerColor[] makeShortColors()
    {
        PlayerColor[] colors = new PlayerColor[20];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = pColors[Mathf.RoundToInt(9.5f * i)];

        return colors;
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
                player.playerInputScheme = playerSelectionScript.getPlayerInputScheme();
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
                player.playerInputScheme = playerSelectionScript.getPlayerInputScheme();
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
        updateTeamColors();
        updateTeamNos();
    }

    public void returnPlayerSprite(int index)
    {
        pSprites[index].setAvailable(true);
    }

    public void returnPlayerColor(int index)
    {
        activepColors[index].setAvailable(true);
    }

    public int getNextPlayerSprite(int index, int direction)
    {
        //gets next aviaoble and sets it

        pSprites[index].setAvailable(true);
        
        int newIndex = tryGetNextSprite(index, direction);

        pSprites[newIndex].setAvailable(false);

        return newIndex;
    }
    
    public int tryGetNextSprite(int index, int direction)
    {
        return tryGetNextSprite(index, direction, false);
    }

    public int tryGetNextSprite(int index, int direction, bool ignoreAvailablity)
    {
        //just returns the next available

        int newIndex = index + direction;

        if (newIndex >= pSprites.Length)
            newIndex -= pSprites.Length;
        if (newIndex < 0)
            newIndex += pSprites.Length;

        if (ignoreAvailablity)
            return newIndex;

        if (pSprites[newIndex].getAvailable())
            return newIndex;
        else
            //if direction is 0, then we are probing to see if the current index is available, return -1 if it isn't
            if (direction == 0)
                return -1;
            else
                return tryGetNextSprite(newIndex, direction);
    }

    public int getNextPlayerColor(int index, int direction)
    {
        //gets next aviaoble and sets it

        activepColors[index].setAvailable(true);
                
        int newIndex = tryGetNextColor(index, direction);

        activepColors[newIndex].setAvailable(false);

        return newIndex;
    }

    public int tryGetNextColor(int index, int direction)
    {
        return tryGetNextColor(index, direction, false);
    }

    public int tryGetNextColor(int index, int direction, bool ignoreAvailablity)
    {
        //just returns the next available

        int newIndex = index + direction;

        if (newIndex >= activepColors.Length)
            newIndex -= activepColors.Length;
        if (newIndex < 0)
            newIndex += activepColors.Length;

        if (ignoreAvailablity)
            return newIndex;

        if (activepColors[newIndex].getAvailable())
            return newIndex;
        else
            //if direction is 0, then we are probing to see if the current index is available, return -1 if it isn't
            if (direction == 0)
                return -1;
            else
                return tryGetNextColor(newIndex, direction);
    }

    public PlayerSprite[] getPlayerSprites()
    {
        return pSprites;
    }

    public PlayerColor[] getPlayerColors()
    {
        return activepColors;
    }

    public PlayerColor getTeamColor(int index)
    {
        //returns the corresponding overal team color
        return makeShortColors()[((int)(index / 19))*2];
    }
}
