using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//spawns all the x and y tiles for this slector
//talks to player selection script amamanger

public class PlayerSelectionScript : MonoBehaviour {

    public GameObject backgroundSprite;
    public UITextScript mainText;
    public UITextScript teamText;
    public GameObject controlsCanvas;

    public GameObject playerSelecterTile;
    public SFX colorSound;
    public SFX spriteSound;
    public SFX selectionSound;

    public bool menuRollOver = true;
    public int scrollSpeedMin = 60, scrollSpeedMax = 15; //the number of frames between selecting

    private int currentScrollSpeedMinH = 0, currentScrollSpeedMinV = 0; //slowly increased while a direction is being pushed
    private int currentScrollSpeedH = 0, currentScrollSpeedV = 0; //adds 1 every frame that an input is held
    private bool canScrollH = false, canScrollV = false; //bool used to inhibit sanic levels of scrolling

    private List<PlayerSelectionTileBehaviour> spriteTiles;
    private List<PlayerSelectionTileBehaviour> colorTiles;

    private PlayerSprite[] playerSprites;
    private PlayerColor[] playerColors;

    private ParticleSystem particleSystem;

    private PlayerInputScheme playerInputScheme;
    private int playerNo;
    private int teamNo;
    private bool team;

    private float wiggleH, wiggleV;

    int currentSprite;       //the indexes of the currently selcted ones
    int currentColor;

    private bool active;
    private bool greyedOut = false;
    private bool ready = false;

    private PlayerSelectionScriptManager manager;
    private PlayerManagerBehaviour playerManager;

    private Camera camera;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start ()
    {
        particleSystem = GetComponent<ParticleSystem>();

        playerManager = GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>();

        if (playerManager == null)
        {
            UnityEngine.Debug.LogError("Player manager has not been set!");
            this.enabled = false;
        }

        camera = GameObject.Find("Viewing Camera").GetComponent<Camera>();

        if (camera == null)
        {
            UnityEngine.Debug.LogError("Viewing camera can not be found!");
            this.enabled = false;
        }
        
        grabAndBuildTiles();
	}

    private void grabAndBuildTiles()
    {
        spriteTiles = new List<PlayerSelectionTileBehaviour>();
        colorTiles = new List<PlayerSelectionTileBehaviour>(); 
        
        manager = transform.parent.GetComponent<PlayerSelectionScriptManager>();
        if (manager == null)
        {
            UnityEngine.Debug.LogError("Cannot find player selector manager parent!");
            this.enabled = false;
        }

        spriteRenderer = backgroundSprite.GetComponent<SpriteRenderer>();

        //grab the data from the manger
        playerSprites = manager.getPlayerSprites();
        playerColors = manager.getPlayerColors();

        //spawn some tiles and populate them, make a cross pattern with the len/2 element being the centre

        //x axis
        for (int i = 0; i < playerSprites.Length; i++)
        {
            PlayerColor newColor = new PlayerColor();
            newColor.color = Color.white;
            newColor.setAvailable(true);

            GameObject tile = Instantiate(playerSelecterTile, transform.position, Quaternion.identity) as GameObject;
            spriteTiles.Add(tile.GetComponent<PlayerSelectionTileBehaviour>());

            spriteTiles[i].transform.parent = transform;
            spriteTiles[i].transform.localScale = Vector3.one;
            spriteTiles[i].transform.localPosition = new Vector3((7f / 6f), 0, 0) * (i - (playerSprites.Length / 2) - 1);
            spriteTiles[i].setSprite(playerSprites[i]);
            spriteTiles[i].setColor(newColor);

            //set the sprite tiles to render infornt of color tiles
            tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        //y axis
        for (int i = 0; i < playerColors.Length; i++)
        {
            GameObject tile = Instantiate(playerSelecterTile, transform.position, Quaternion.identity) as GameObject;
            colorTiles.Add(tile.GetComponent<PlayerSelectionTileBehaviour>());

            colorTiles[i].transform.parent = transform;
            colorTiles[i].transform.localScale = Vector3.one;
            colorTiles[i].transform.localPosition = new Vector3(-(11f / 30f), 1.375f, 0) * (i - (playerColors.Length / 2));
            colorTiles[i].setColor(playerColors[i]);
        }

        currentSprite = (playerSprites.Length / 2) + 1;
        currentColor = playerColors.Length / 2;

        //shift wheel by player no so we don't get everyone on the same thing
        for (int i = 0; i < playerNo; i++)
        {
            moveColorWheel(1, false);
            moveSpriteWheel(1, false);
        }

        //disable our selfs, and then wait for input
        setActive(false);

        currentScrollSpeedMinH = scrollSpeedMin;
        currentScrollSpeedMinV = scrollSpeedMin;
    }

    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonDetected;
        InputManagerBehaviour.axisPressed += axisDetected;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonDetected;
        InputManagerBehaviour.axisPressed -= axisDetected;
    }

    void buttonDetected(PlayerInputScheme player, string inputName, float value)
    {
        if (player.id == playerInputScheme.id)
        {
            //ignore jump and flump if they are pressed as in some cases, we actiave and then ready in the same frame

            if (!greyedOut)
            {
                if (inputName == "Cancel")
                {
                    if (ready)
                    {
                        ready = false;
                        unready();
                    }
                    else
                        setActive(false);
                }
                else
                {
                    if (inputName == "Submit" && active && !ready)
                    {
                        ready = true;
                        readyUp();
                    }

                    if (!inputName.Contains("ump"))
                        //activate upon any input,
                        if (!active)
                            setActive(true);
                }
            }
        }
    }

    void axisDetected(PlayerInputScheme player, string inputName, float value)
    {
        if (player.id == playerInputScheme.id)
        {
            switch (inputName)
            {
                case "WiggleHorizontal":
                    wiggleH = value;
                    rotateWindow();
                    break;
                case "WiggleVertical":
                    wiggleV = value;
                    rotateWindow();
                    break;
                case "Horizontal":
                    if (!ready && !greyedOut)
                        axisSelection(inputName, value, true);
                    break;
                case "Vertical":
                    if (!ready && !greyedOut)
                        axisSelection(inputName, value, true);
                    break;
            }
        }
    }

    // Update is called once per frame
	void Update ()
    {
        if (spriteTiles == null || colorTiles == null)
            grabAndBuildTiles();

        if (!active && !greyedOut)
            mainText.setText("<size=25><color=yellow> Player " + playerNo + "\nPress some buttons</color></size>");
        else
            mainText.setText("");

        rotateWindow();

        if (active && ready)
            mainText.setText("<size=50><color=yellow>\r\n\r\nREADY!</color></size>");

        //check if particvle system is playing and play if we need it to
        if (ready && !particleSystem.isPlaying)
            particleSystem.Play();

        if (!greyedOut)
        {
            //set the currently selected tiles to unavailabe
            if (active)
            {
                spriteTiles[currentSprite].getSprite().setAvailable(false);
                colorTiles[currentColor].getColor().setAvailable(false);
            }

            setTileAlpha();

            //the currently selected sprite will get the currently selected color
            spriteTiles[currentSprite].setColor(colorTiles[currentColor].getColor());
        }
	}

    private void axisSelection(string navigateAxis, float input, bool playSound)
    {
        //create artificila dead zone
        if (Math.Abs(input) < 0.5f)
            input = 0;

        //handles the option rollover selection
        if (navigateAxis.Contains("zontal"))
        {
            //if we can scroll
            if (canScrollH && input != 0)
            {
                //parse userinput
                moveSpriteWheel(input, playSound);

                //stop scrolling
                canScrollH = false;

                //while we're scrolling
                //decrease current min by 10%, unitil it's at max
                //this give the overall effect of increasing how frequently canScroll is activated - it speeds up slow at first, then faster
                //done within can scroll so that the whole process is exponential in a way

                if (currentScrollSpeedMinH >= scrollSpeedMax)
                    currentScrollSpeedMinH = (int)(currentScrollSpeedMinH * 0.9);
            }

            if (input == 0)
            {
                //nothing pressed

                //set scroll speed back to min
                currentScrollSpeedH = scrollSpeedMin;

                //set current scroll min to min
                currentScrollSpeedMinH = scrollSpeedMin;
            }

            //increas current Scroll Speed by 1

            //speed checking, if the current speed is over the current min, then set scrollable to true and reset current speed
            //this means that we can move manually as fast as we want

            currentScrollSpeedH++;

            if (currentScrollSpeedH > currentScrollSpeedMinH)
            {
                canScrollH = true;
                currentScrollSpeedH = 0;
            }
        }
        else
        {
            //if we can scroll
            if (canScrollV && input != 0)
            {
                //parse userinput
                moveColorWheel(input, playSound);

                //stop scrolling
                canScrollV = false;

                //while we're scrolling
                //decrease current min by 10%, unitil it's at max
                //this give the overall effect of increasing how frequently canScroll is activated - it speeds up slow at first, then faster
                //done within can scroll so that the whole process is exponential in a way

                if (currentScrollSpeedMinV >= scrollSpeedMax)
                    currentScrollSpeedMinV = (int)(currentScrollSpeedMinV * 0.9);
            }

            if (input == 0)
            {
                //nothing pressed

                //set scroll speed back to min
                currentScrollSpeedV = scrollSpeedMin;

                //set current scroll min to min
                currentScrollSpeedMinV = scrollSpeedMin;
            }

            //increas current Scroll Speed by 1

            //speed checking, if the current speed is over the current min, then set scrollable to true and reset current speed
            //this means that we can move manually as fast as we want

            currentScrollSpeedV++;

            if (currentScrollSpeedV > currentScrollSpeedMinV)
            {
                canScrollV = true;
                currentScrollSpeedV = 0;
            }
        }
    }

    private void readyUp()
    {
        //make all other tiles dissapear
        for (int i = 0; i < spriteTiles.Count; i++)
        {
            if (i != currentSprite)
                spriteTiles[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < colorTiles.Count; i++)
        {
            if (i != currentColor)
                colorTiles[i].gameObject.SetActive(false);
        }

        spriteRenderer.enabled = false;

        //enlarges the selected skin
        spriteTiles[currentSprite].transform.localScale *= 3;
        colorTiles[currentColor].transform.localScale *= 3;
        
        //plays particles behind
        Color color = colorTiles[currentColor].getColor().color;
        color.a = 1;
        particleSystem.startColor = color;
        particleSystem.Play();

        //plays sound
        SFXManagerBehaviour.Instance.playSound(selectionSound);
    }

    private void unready()
    {
        //shrink the selected skin
        spriteTiles[currentSprite].transform.localScale /= 3;
        colorTiles[currentColor].transform.localScale /= 3;
        
        particleSystem.Stop();

        setActive(true);
    }

    private void rotateWindow()
    {
        //takes in the right ananloge stick on the controller and rotates it just like in meele
        transform.localRotation = Quaternion.Euler(25f * wiggleV, 25f * wiggleH, 0);
    }

    private void moveColorWheel(float direction, bool sound)
    {
        //unset old selected
        colorTiles[currentColor].getColor().setAvailable(true);

        if (direction > 0)
        {
            moveColorWheelUp();
            //if this color is already picked, then go again
            if (!colorTiles[currentColor].getColor().getAvailable())
                moveColorWheel(1, false);
        }
        else if (direction < 0)
        {
            moveColorWheelDown();
            if (!colorTiles[currentColor].getColor().getAvailable())
                moveColorWheel(-1, false);
        }

        //play sound
        if (direction != 0 && sound)
            SFXManagerBehaviour.Instance.playSound(colorSound);

        updateTeam();
    }

    private void moveSpriteWheel(float direction, bool sound)
    {
        //unset old selected
        spriteTiles[currentSprite].getSprite().setAvailable(true);

        //set color of old sprite to white, then shift sprites along, and set new current
        PlayerColor newColor = new PlayerColor();
        newColor.color = Color.white;
        newColor.setAvailable(true);
        spriteTiles[currentSprite].setColor(newColor);

        if (direction > 0)
        {
            moveSpriteWheelLeft();
            //if this spreit is already picked, then go again
            if (!spriteTiles[currentSprite].getSprite().getAvailable())
                moveSpriteWheel(1, false);
        }
        else if (direction < 0)
        {
            moveSpriteWheelRight();
            if (!spriteTiles[currentSprite].getSprite().getAvailable())
                moveSpriteWheel(-1, false);
        }

        //play sound
        if (direction != 0 && sound)
            SFXManagerBehaviour.Instance.playSound(spriteSound);
    }

    private void moveColorWheelUp()
    {
        //shift colors along in the queue
        Vector3 minTempPosition = colorTiles[0].transform.position;

        for (int i = 0; i < colorTiles.Count; i++)
        {
            int j = nfmod(i + 1, colorTiles.Count);
            colorTiles[i].transform.position = colorTiles[j].transform.position;
        }

        colorTiles[colorTiles.Count - 1].transform.position = minTempPosition;
        
        currentColor = nfmod(currentColor - 1, colorTiles.Count);
    }

    private void moveColorWheelDown()
    {
        //shift colors along in the queue
        Vector3 maxTempPosition = colorTiles[colorTiles.Count-1].transform.position;

        for (int i = colorTiles.Count - 1; i > 0; i--)
        {
            int j = nfmod(i - 1, colorTiles.Count);
            colorTiles[i].transform.position = colorTiles[j].transform.position;
        }

        colorTiles[0].transform.position = maxTempPosition;

        currentColor = nfmod(currentColor + 1, colorTiles.Count);
    }
    
    private void moveSpriteWheelRight()
    {
        Vector3 minTempPosition = spriteTiles[0].transform.position;

        for (int i = 0; i < spriteTiles.Count; i++)
        {
            int j = nfmod(i + 1, spriteTiles.Count);
            spriteTiles[i].transform.position = spriteTiles[j].transform.position;
        }

        spriteTiles[spriteTiles.Count - 1].transform.position = minTempPosition;

        currentSprite = nfmod(currentSprite - 1, spriteTiles.Count);
    }

    private void moveSpriteWheelLeft()
    {
        Vector3 maxTempPosition = spriteTiles[spriteTiles.Count - 1].transform.position;

        for (int i = spriteTiles.Count - 1; i > 0; i--)
        {
            int j = nfmod(i - 1, spriteTiles.Count);
            spriteTiles[i].transform.position = spriteTiles[j].transform.position;
        }

        spriteTiles[0].transform.position = maxTempPosition;

        currentSprite = nfmod(currentSprite + 1, spriteTiles.Count);
    }

    private void updateTeam()
    {
        if (team)
        {
            teamNo = (int)(currentColor / 19) + 1;
            teamText.setText("<size=25><color=#" + playerColors[currentColor].color.GetHashCode() + "> Team " + teamNo + "</color></size>");
        }
        else
        {
            teamNo = playerNo;
            teamText.setText("");
        }
    }

    private void setTileAlpha()
    {
        //here we alpha out each tile based on how close they are to the current tile

        PlayerColor currentPlayerColor;

        Vector3 scale = spriteTiles[currentSprite].transform.localScale;
        float distance;

        for (int i = 0; i < spriteTiles.Count; i++)
        {
            currentPlayerColor = spriteTiles[i].getColor();

            //calculate and pick the shortest distance to the centre
            //scale it down so we get int values of dist from current
            distance = (spriteTiles[currentSprite].transform.localPosition.x - spriteTiles[i].transform.localPosition.x) / scale.x;

            //distance coverers, different for left and right

            //length on right
            if (distance > 6)
                currentPlayerColor.color.a = 0;
            else
                //here we are setting, make dist abs, as left will have negative dist
                currentPlayerColor.color.a = (1f - (Mathf.Abs(distance) / 6f));

            //shorten the left side
            //if negative, then on left
            if (distance < -2)
                currentPlayerColor.color.a = 0;

            spriteTiles[i].setAlpha(currentPlayerColor.color.a);
        }

        for (int i = 0; i < colorTiles.Count; i++)
        {
            currentPlayerColor = colorTiles[i].getColor();

            //calculate and pick the shortest distance to the centre
            //scale it down so we get int values of dist from current
            distance = (colorTiles[currentColor].transform.localPosition.y - colorTiles[i].transform.localPosition.y) / scale.y;

            if (Mathf.Abs(distance) > 3)
                currentPlayerColor.color.a = 0;
            else
                //here we are setting, make dist abs, as left will have negative dist
                currentPlayerColor.color.a = (1f - (Mathf.Abs(distance) / 8f));

            colorTiles[i].setAlpha(currentPlayerColor.color.a);
        }
    }

    private int nfmod(float a,float b)
    {
        return (int)(a - b * Mathf.Floor(a / b));
    }

    public void setPlayer(PlayerInputScheme player)
    {
        playerInputScheme = player;

        playerNo = player.id;

        //set control graphic (and controller number graphic if applicable)
        controlsCanvas.SendMessage("setControl", player);
    }

    public PlayerInputScheme getPlayer()
    {
        return playerInputScheme;
    }

    public int getPlayerNo()
    {
        return playerNo;
    }

    public void setPlayerNo(int no)
    {
        playerNo = no;
    }

    public void setTeam(bool val)
    {
        team = val;
        updateTeam();
    }

    public int getTeam()
    {
        return teamNo;
    }
    
    public PlayerSprite getSprite()
    {
        return spriteTiles[currentSprite].getSprite();
    }

    public PlayerColor getColor()
    {
        return colorTiles[currentColor].getColor();
    }

    public bool getActive()
    {
        return active;
    }

    public void setActive(bool value)
    {
        if (spriteTiles == null || colorTiles == null)
            grabAndBuildTiles();

        active = value;

        //set each tile in this selector to value
        foreach (PlayerSelectionTileBehaviour pSTB in spriteTiles)
            pSTB.gameObject.SetActive(value);

        foreach (PlayerSelectionTileBehaviour pSTB in colorTiles)
            pSTB.gameObject.SetActive(value);

        //set the tiles we were on to value
        spriteTiles[currentSprite].getSprite().setAvailable(!value);
        colorTiles[currentColor].getColor().setAvailable(!value);

        spriteRenderer.enabled = value;
    }

    public bool getReady()
    {
        return ready;
    }

    public void setReady(bool val)
    {
        ready = val;
        if (ready)
            readyUp();
    }

    public void setGrey(bool value)
    {
        //sets the selector to greyed out and inactive if true
        greyedOut = value;

        if (value)
            spriteRenderer.color = Color.gray;
        else
            spriteRenderer.color = Color.white;
    }
}
