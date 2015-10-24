using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//spawns all the x and y tiles for this slector
//talks to player selection script amamanger

public class PlayerSelectionScript : MonoBehaviour {

    public GameObject backgroundSprite;
    public GameObject greenOption;
    public GameObject redOption;
    public UITextScript mainText;
    public UITextScript teamText;
    public GameObject controlsCanvas;

    public GameObject playerSelecterTile;
    public SFX colorSound;
    public SFX spriteSound;
    public SFX selectionSound;

    public bool menuRollOver = true;
    public int scrollSpeedMin = 60, scrollSpeedMax = 5; //the number of frames between selecting

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

    private PulseInwardsScript pulseScript;
    private const float pulseMag = 0.3f;

	// Use this for initialization
	void Start ()
    {
        particleSystem = GetComponent<ParticleSystem>();

        pulseScript = GetComponent<PulseInwardsScript>();

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

    void OnEnable()
    {
        particleSystem = GetComponent<ParticleSystem>();
        //check if particles should play
        if (ready && !particleSystem.isPlaying)
            particleSystem.Play();
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
        for (int i = -5; i < 3; i++)
        {
            PlayerColor newColor = new PlayerColor();
            newColor.color = Color.white;
            newColor.setAvailable(true);

            GameObject tile = Instantiate(playerSelecterTile, transform.position, Quaternion.identity) as GameObject;
            PlayerSelectionTileBehaviour tileBehaviour = tile.GetComponent<PlayerSelectionTileBehaviour>();
            spriteTiles.Add(tileBehaviour);

            tileBehaviour.transform.parent = transform;
            tileBehaviour.transform.localScale = Vector3.one;
            tileBehaviour.transform.localPosition = new Vector3((7f / 6f), 0, 0) * i;
            tileBehaviour.setSprite(playerSprites[0]);
            tileBehaviour.setColor(newColor);

            //set the sprite tiles to render infornt of color tiles
            tile.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }

        //y axis
        for (int i = -2; i < 3; i++)
        {
            GameObject tile = Instantiate(playerSelecterTile, transform.position, Quaternion.identity) as GameObject;
            PlayerSelectionTileBehaviour tileBehaviour = tile.GetComponent<PlayerSelectionTileBehaviour>();
            colorTiles.Add(tileBehaviour);

            tileBehaviour.transform.parent = transform;
            tileBehaviour.transform.localScale = Vector3.one;
            tileBehaviour.transform.localPosition = new Vector3(-(11f / 30f), 1.375f, 0) * i;
            tileBehaviour.setColor(playerColors[0]);
        }

        currentSprite = 5;
        currentColor = 2;

        //shift wheel by player no so we don't get everyone on the same thing
        for (int i = 0; i < playerNo; i++)
            moveColorWheel(1, false);

        for (int i = 0; i < playerNo * 3; i++)
            moveSpriteWheel(1, false);

        //disable our selfs, and then wait for input
        setActive(false);

        currentScrollSpeedMinH = scrollSpeedMin;
        currentScrollSpeedMinV = scrollSpeedMin;
    }

    public void buttonDetected(PlayerInputScheme player, string inputName, float value)
    {
        if (player.id == playerInputScheme.id)
        {
            //ignore jump and flump if they are pressed as in some cases, we actiave and then ready in the same frame

            if (!greyedOut)
            {
                if (inputName == player.inputs[PlayerInput.CancelInput].shortName)
                {
                    if (ready)
                        unready();
                    else
                        setActive(false);
                }
                else
                {
                    if (inputName == player.inputs[PlayerInput.SubmitInput].shortName && active && !ready)
                        readyUp();

                    if (!inputName.Contains("ump"))
                        //activate upon any input,
                        if (!active)
                            setActive(true);
                }
            }
        }
    }

    public void axisDetected(PlayerInputScheme player, string inputName, float value)
    {
        if (player.id == playerInputScheme.id && active)
        {
            if (player.inputType == InputType.ControllerFull)
            {
                if (inputName == player.inputs[PlayerInput.WiggleHorizontalInput].shortName.ToString())
                {
                    wiggleH = value;
                    rotateWindow();
                }
                if (inputName == player.inputs[PlayerInput.WiggleVerticalInput].shortName.ToString())
                {
                    wiggleV = value;
                    rotateWindow();
                }
            }
            if (inputName == player.inputs[PlayerInput.HorizontalInput].shortName.ToString())
            {
                if (!ready && !greyedOut)
                    StartCoroutine(axisSelection(inputName, value, true));
            }
            if (inputName == player.inputs[PlayerInput.VerticalInput].shortName.ToString())
            {
                if (!ready && !greyedOut)
                    StartCoroutine(axisSelection(inputName, value, true));
            }
        }
    }

    private IEnumerator axisSelection(string navigateAxis, float input, bool playSound)
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
                moveSpriteWheel(-input, playSound);

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

        yield return null;

        if (spriteTiles == null || colorTiles == null)
            grabAndBuildTiles();

        yield return null;
        
        //the currently selected sprite will get the currently selected color
        spriteTiles[5].setColor(colorTiles[2].getColor());
    }

    private void readyUp()
    {
        ready = true;

        //make all other tiles dissapear
        for (int i = 0; i < spriteTiles.Count; i++)
            if (i != 5)
                spriteTiles[i].gameObject.SetActive(false);

        for (int i = 0; i < colorTiles.Count; i++)
            if (i != 2)
                colorTiles[i].gameObject.SetActive(false);

        spriteRenderer.enabled = false;
        greenOption.SetActive(false);
        redOption.SetActive(false);
        controlsCanvas.SetActive(false);

        //enlarges the selected skin
        spriteTiles[5].transform.localScale *= 3;
        colorTiles[2].transform.localScale *= 3;
        
        //plays particles behind
        Color color = colorTiles[2].getColor().color;
        color.a = 1;
        particleSystem.startColor = color;
        particleSystem.loop = true;
        particleSystem.Play();

        //plays sound
        SFXManagerBehaviour.Instance.playSound(selectionSound);

        pulseScript.move(new Vector3(1, -1, -1), pulseMag);

        updateTeam();

        mainText.setText("<size=50><color=yellow>\r\n\r\nREADY!</color></size>");
    }

    private void unready()
    {
        ready = false;

        //shrink the selected skin
        spriteTiles[5].transform.localScale /= 3;
        colorTiles[2].transform.localScale /= 3;
        
        particleSystem.Stop();

        updateTeam();

        setActive(true);
    }

    private void rotateWindow()
    {
        //takes in the right ananloge stick on the controller and rotates it just like in meele
        transform.localRotation = Quaternion.Euler(25f * wiggleV, 25f * wiggleH, 0);
    }

    private void moveColorWheel(float direction, bool sound)
    {
        currentColor = manager.getNextPlayerColor(currentColor, (int)Mathf.Sign(direction));

        //play sound
        if (direction != 0 && sound)
            SFXManagerBehaviour.Instance.playSound(colorSound);

        //move selecter right so it lerps back
        if (pulseScript == null)
            pulseScript = GetComponent<PulseInwardsScript>();

        pulseScript.move(Vector3.up * direction, pulseMag * (currentScrollSpeedMinV / (float)scrollSpeedMin));

        updateTeam();
    }

    private void moveSpriteWheel(float direction, bool sound)
    {
        currentSprite = manager.getNextPlayerSprite(currentSprite, (int)Mathf.Sign(direction));
        
        //play sound
        if (direction != 0 && sound)
            SFXManagerBehaviour.Instance.playSound(spriteSound);

        //move selecter right so it lerps back
        pulseScript.move(Vector3.right * direction, pulseMag * (currentScrollSpeedMinH / (float)scrollSpeedMin));

        updateSpriteTiles();
    }

    private void updateColorTiles(bool teamChanged)
    {
        if (manager == null)
            return;
        
        playerColors = manager.getPlayerColors();

        //alting of the current color if teams have been change
        if (teamChanged)
        {
            if (team)
                currentColor = Mathf.RoundToInt(currentColor * 9.5f);
            else
                currentColor = Mathf.RoundToInt(currentColor / 9.5f);
            
            //free up the currentcolor, as it might be some abritery color that doesn't map to the short list
            manager.returnPlayerColor(currentColor);

            //seach for a new color that we can use
            if (active)
                setActive(true);
        }

        for (int i = -2; i < 3; i++)
        {
            int index = manager.tryGetNextColor(currentColor, i, true);
            colorTiles[i + 2].setColor(playerColors[index]);
        }
    }

    private void updateSpriteTiles()
    {
        for (int i = -5; i < 3; i++)
        {
            int index = manager.tryGetNextSprite(currentSprite, i, true);
            spriteTiles[i + 5].setSprite(playerSprites[index]);
        }
    }

    private void updateTeam()
    {
        updateTeam(false);
    }

    private void updateTeam(bool teamChanged)
    {
        if (teamChanged && active && ready)
            unready();

        if (team && active && !ready)
        {
            teamNo = (int)(currentColor / 19) + 1;
            teamText.setText("<size=25>Team " + teamNo + "</size>");
            teamText.setColor(manager.getTeamColor(currentColor).color);
        }
        else
        {
            teamNo = playerNo;
            teamText.setText("");
        }
        
        //update color position (and amount of colors if team has changed)
        updateColorTiles(teamChanged);
    }

    public void setPlayer(PlayerInputScheme player)
    {
        playerInputScheme = player;

        playerNo = player.id;

        //set control graphic (and controller number graphic if applicable)
        controlsCanvas.SendMessage("setControl", player);
    }

    public PlayerInputScheme getPlayerInputScheme()
    {
        return playerInputScheme;
    }

    public int getPlayerNo()
    {
        return playerNo;
    }

    public int getPlayerInputSchemeId()
    {
        if (playerInputScheme != null)
            return playerInputScheme.id;
        else
            return 0;
    }

    public void setPlayerNo(int no)
    {
        playerNo = no;
    }

    public void setTeam(bool val)
    {
        bool teamChanged = false;

        if (team != val)
            teamChanged = true;

        team = val;
        updateTeam(teamChanged);
    }

    public int getTeam()
    {
        return teamNo;
    }
    
    public PlayerSprite getSprite()
    {
        return spriteTiles[5].getSprite();
    }

    public PlayerColor getColor()
    {
        return colorTiles[2].getColor();
    }

    public bool getActive()
    {
        return active;
    }

    public void setActive(bool value)
    {
        if (spriteTiles == null || colorTiles == null)
            return;

        active = value;

        //set each tile in this selector to value
        foreach (PlayerSelectionTileBehaviour pSTB in spriteTiles)
            pSTB.gameObject.SetActive(value);

        foreach (PlayerSelectionTileBehaviour pSTB in colorTiles)
            pSTB.gameObject.SetActive(value);

        if (value)
        {
            //check to see if we can still use the currently selected
            int newSprite = manager.tryGetNextSprite(currentSprite, 0);
            if (newSprite == -1)
            {
                //manual moving of the tiles with unsetting anything
                currentSprite = manager.tryGetNextSprite(currentSprite, 1);
                moveSpriteWheel(0, false);
            }
            int newColor = manager.tryGetNextColor(currentColor, 0);
            if (newColor == -1)
            {
                currentColor = manager.tryGetNextColor(currentColor, 1);
                moveColorWheel(0, false);
            }
        }
        else
        {
            //set the tiles we were on to value
            manager.returnPlayerSprite(currentSprite);
            manager.returnPlayerColor(currentColor);
        }

        spriteRenderer.enabled = value;
        greenOption.SetActive(value);
        redOption.SetActive(value);
        controlsCanvas.SetActive(true);
        
        if (!active && !greyedOut)
            mainText.setText("<size=25><color=yellow>Player " + playerNo + "\nPress some buttons</color></size>");
        else
            mainText.setText(" ");

        updateTeam();
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
