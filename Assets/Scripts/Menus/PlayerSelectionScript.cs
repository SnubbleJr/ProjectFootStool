using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//spawns all the x and y tiles for this slector
//talks to player selection script amamanger

public class PlayerSelectionScript : MonoBehaviour {

    public GameObject playerSelecterTile;
    public AudioClip colorSound;
    public AudioClip spriteSound;
    public AudioClip selectionSound;

    private List<PlayerSelectionTileBehaviour> spriteTiles = new List<PlayerSelectionTileBehaviour>();
    private List<PlayerSelectionTileBehaviour> colorTiles = new List<PlayerSelectionTileBehaviour>();

    private PlayerSprite[] playerSprites;
    private PlayerColor[] playerColors;

    private ParticleSystem particleSystem;

    private int playerNo;
    private string horizontalAxis = "Horizontal";
    private string verticallAxis = "Vertical";
    private string secondHorizontalAxis = "RHorizontal";
    private string secondVerticallAxis = "RVertical";
    private string submitKey = "Submit";
    private string cancelKey = "Cancel";

    private bool hitRight = false;      //used so moving the horizontal axis only moves one tile
    private bool hitLeft = false;
    private bool hitUp = false;
    private bool hitDown = false;

    int currentSprite;       //the indexes of the currently selcted ones
    int currentColor;

    private bool active;
    private bool greyedOut = false;
    private bool ready = false;

    private PlayerSelectionScriptManager manager;
    private PlayerManagerBehaviour playerManager;

    private Camera camera;

    private SpriteRenderer spriteRenderer;

    private AudioSource audio;

	// Use this for initialization
	void Start ()
    {
        manager = transform.parent.GetComponent<PlayerSelectionScriptManager>();

        particleSystem = GetComponent<ParticleSystem>();

        audio = GetComponent<AudioSource>();

        if (manager == null)
        {
            UnityEngine.Debug.LogError("Cannot find player selector manager parent!");
            this.enabled = false;
        }

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

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        //grab the data from the manger
        playerSprites = manager.playerSprites;
        playerColors = manager.playerColors;
        
        //spawn some tiles and populate them, make a cross pattern with the len/2 element being the centre
                
        //x axis
        for (int i = 0; i < playerSprites.Length; i++)
        {
            PlayerColor newColor = new PlayerColor();
            newColor.color = Color.white;
            newColor.setAvailable(true);

            GameObject tile = Instantiate(playerSelecterTile, (transform.position + (Vector3.right * (i - (playerSprites.Length / 2)-1))), Quaternion.identity) as GameObject;
            spriteTiles.Add(tile.GetComponent < PlayerSelectionTileBehaviour>());

            spriteTiles[i].transform.parent = transform;
            spriteTiles[i].setSprite(playerSprites[i]);
            spriteTiles[i].setColor(newColor);

            //set the sprite tiles to render infornt of color tiles
            tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        //y axis
        for (int i = 0; i < playerColors.Length; i++)
        {
            GameObject tile = Instantiate(playerSelecterTile, (transform.position + (Vector3.up * (i - (playerColors.Length / 2)))), Quaternion.identity) as GameObject;
            colorTiles.Add(tile.GetComponent < PlayerSelectionTileBehaviour>());

            colorTiles[i].transform.parent = transform;
            colorTiles[i].setColor(playerColors[i]);
        }

        currentSprite = (playerSprites.Length / 2)+1;
        currentColor = playerColors.Length / 2;

        //shift wheel by player no so we don't get everyone on the same thing
        for (int i = 0; i < playerNo; i++)
        {
            moveColorWheel(1);
            moveSpriteWheel(1);
        }

        //disable our selfs, and then wait for input
        setActive(false);
	}
    	
    void OnGUI()
    {
        GUI.skin = playerManager.skin;

        Vector3 screenPoint = camera.WorldToScreenPoint(transform.position);

        int height = camera.pixelHeight;

        if (!active && !greyedOut)
        {            
            GUI.Label(new Rect(screenPoint.x -98, height - screenPoint.y -52, 300, 70), "<size=25><color=black> Player " + playerNo + "\nPress some buttons</color></size>");
            GUI.Label(new Rect(screenPoint.x -100, height - screenPoint.y -50, 300, 70), "<size=25><color=yellow> Player " + playerNo + "\nPress some buttons</color></size>");
        }

        if (active && ready)
        {
            GUI.Label(new Rect(screenPoint.x -149, height - screenPoint.y + 27, 300, 30), "<size=30><color=black>READY!</color></size>");
            GUI.Label(new Rect(screenPoint.x -150, height - screenPoint.y + 25, 300, 30), "<size=30><color=yellow>READY!</color></size>");
        }
    }

	// Update is called once per frame
	void Update ()
    {
        //check if particvle system is playing and play if we need it to
        if (ready && !particleSystem.isPlaying)
            particleSystem.Play();

        if (!greyedOut)
        {
            activationAndReadyInputHandler();

            inputCheck();

            //the currently selected sprite will get the currently selected color
            spriteTiles[currentSprite].setColor(colorTiles[currentColor].getColor());

            //set the currently selected tiles to unavailabe
            if (active)
            {
                spriteTiles[currentSprite].getSprite().setAvailable(false);
                colorTiles[currentColor].getColor().setAvailable(false);
            }

            setTileAlpha();

            rotateWindow();
        }
	}
    
    private void inputCheck()
    {
        if (!ready && active)
        {
            float v = Input.GetAxisRaw(verticallAxis);

            if (v < 0)
            {
                if (!hitUp)
                {
                    hitUp = true;
                    moveColorWheel(1);
                }
            }
            else
                hitUp = false;

            if (v > 0)
            {
                if (!hitDown)
                {
                    hitDown = true;
                    moveColorWheel(-1);
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
                    moveSpriteWheel(1);
                }
            }
            else
                hitLeft = false;

            if (h > 0)
            {
                if (!hitRight)
                {
                    hitRight = true;
                    moveSpriteWheel(-1);
                }
            }
            else
                hitRight = false;
        }
    }

    private void activationAndReadyInputHandler()
    {
        if (Input.GetButtonDown(submitKey) && active)
        {
            if (!ready)
            {
                ready = true;
                readyUp();
            }
        }

        //activate upon any input,
        if (!active && ((Input.GetAxisRaw(verticallAxis) != 0) || (Input.GetAxisRaw(horizontalAxis) != 0) || (Input.GetButton(submitKey))))
            setActive(true);

        if (Input.GetButtonDown(cancelKey))
        {
            if (ready)
            {
                ready = false;
                unready();
            }
            else
                setActive(false);
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
        audio.PlayOneShot(selectionSound, 1f);
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
        float h = Input.GetAxis(secondHorizontalAxis);
        float v = Input.GetAxis(secondVerticallAxis);

        transform.localRotation = Quaternion.Euler(25f * v, 25f * h, 0);
    }

    private void moveColorWheel(int direction)
    {
        //unset old selected
        colorTiles[currentColor].getColor().setAvailable(true);

        if (direction > 0)
        {
            moveColorWheelUp();
            //if this spreit is already picked, then go again
            if (!colorTiles[currentColor].getColor().getAvailable())
                moveColorWheel(1);
        }
        else
        {
            moveColorWheelDown();
            if (!colorTiles[currentColor].getColor().getAvailable())
                moveColorWheel(-1);
        }

        //play sound
        audio.PlayOneShot(colorSound, 0.3f);
    }

    private void moveSpriteWheel(int direction)
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
                moveSpriteWheel(1);
        }
        else
        {
            moveSpriteWheelRight();
            if (!spriteTiles[currentSprite].getSprite().getAvailable())
                moveSpriteWheel(-1);
        }

        //play sound
        audio.PlayOneShot(spriteSound, 0.3f);
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

    private void setTileAlpha()
    {
        //here we alpha out each tile based on how close they are to the current tile

        PlayerColor currentPlayerColor;

        for (int i = 0; i < spriteTiles.Count; i++)
        {
            currentPlayerColor = spriteTiles[i].getColor();

            //calculate and pick the shortest distance to the centre

            float distance1 = Mathf.Abs(i - currentSprite - spriteTiles.Count);
            float distance2 = Mathf.Abs(i - currentSprite + spriteTiles.Count);
            float distance3 = Mathf.Abs(i - currentSprite);

            float distance = 0;

            if (distance1 < distance2)
                distance = distance1;
            else
                distance = distance2;

            if (distance > distance3)
                distance = distance3;

            if (distance > 5)
                currentPlayerColor.color.a = 0;
            else
                currentPlayerColor.color.a = (1f - (distance / 5f));

            spriteTiles[i].setAlpha(currentPlayerColor.color.a);
        }

        for (int i = 0; i < colorTiles.Count; i++)
        {
            currentPlayerColor = colorTiles[i].getColor();

            //calculate and pick the shortest distance to the centre

            float distance1 = Mathf.Abs(i - currentColor - colorTiles.Count);
            float distance2 = Mathf.Abs(i - currentColor + colorTiles.Count);
            float distance3 = Mathf.Abs(i - currentColor);

            float distance = 0;

            if (distance1 < distance2)
                distance = distance1;
            else
                distance = distance2;

            if (distance > distance3)
                distance = distance3;

            if (distance > 3)
                currentPlayerColor.color.a = 0;
            else
                currentPlayerColor.color.a = (1f - (distance / 3f));

            colorTiles[i].setAlpha(currentPlayerColor.color.a);
        }
    }

    private int nfmod(float a,float b)
    {
        return (int)(a - b * Mathf.Floor(a / b));
    }

    public void setPlayer(int playerNumber)
    {
        playerNo = playerNumber;

        //set up the inputs for this player
        horizontalAxis = "P" + playerNumber + horizontalAxis;
        verticallAxis = "P" + playerNumber + verticallAxis;
        secondHorizontalAxis = "P" + playerNumber + secondHorizontalAxis;
        secondVerticallAxis = "P" + playerNumber + secondVerticallAxis;
        submitKey = "P" + playerNumber + submitKey;
        cancelKey = "P" + playerNumber + cancelKey;
    }

    public int getPlayer()
    {
        return playerNo;
    }
    
    public PlayerSprite getSprite()
    {
        return spriteTiles[currentSprite].getSprite();
    }

    public PlayerColor getColor()
    {
        return colorTiles[currentColor].getColor();
    }

    public void setActive(bool value)
    {
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

    public bool getActive()
    {
        return active;
    }

    public bool getReady()
    {
        return ready;
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
