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
    public PlayerSprite sprite;
    public PlayerColor color;
}

public class PlayerSelectionScriptManager : MonoBehaviour 
{
    public PlayerSprite[] playerSprites;
    public PlayerColor[] playerColors;
    public GameObject playerSelecterPrefab;

    private List<PlayerSelectionScript> playerSelecters = new List<PlayerSelectionScript>();

	// Use this for initialization
	void Awake ()
    {
	    //set all things to available
        foreach (PlayerSprite playerSprite in playerSprites)
            playerSprite.setAvailable(true);

        foreach (PlayerColor playerColor in playerColors)
            playerColor.setAvailable(true);

        createPlayerSelecter(1);
        createPlayerSelecter(2);
        createPlayerSelecter(3);
        createPlayerSelecter(4);

        //after spawning, scale down the selecter
        transform.localScale = (transform.localScale * 0.75f);
	}
	
    private void createPlayerSelecter(int playerNo)
    {
        float maginutde = 10;

        float x = 0;
        float y = 0;

        Vector3 pos;

        //if odd, then spawn of the right side of the screen
        if (playerNo % 2 == 1)
            x = 1.2f;
        //else will be on the left

        //if above 2, then spawn below
        if (playerNo > 2)
            y = 0.7f;

        pos = new Vector3(x, y, 0);

        GameObject playerSelecter = Instantiate(playerSelecterPrefab, transform.position + (pos * maginutde), Quaternion.identity) as GameObject;
        playerSelecter.transform.parent = transform;

        PlayerSelectionScript playerSelecterScript = playerSelecter.GetComponent<PlayerSelectionScript>();

        playerSelecterScript.setPlayer(playerNo);
        playerSelecters.Add(playerSelecterScript);
    }
        
    public Player[] getPlayers()
    {
        //returns an array of all the players active for a game

        //returns an array of all the players for a round
        List<Player> players = new List<Player>();

        foreach (PlayerSelectionScript playerSelectionScript in playerSelecters)
        {
            if (playerSelectionScript.getActive() && playerSelectionScript.getReady())
            {
                Player player = new Player();

                player.sprite = playerSelectionScript.getSprite();
                player.color = playerSelectionScript.getColor();
                player.playerNo = playerSelectionScript.getPlayer();

                players.Add(player);
            }
        }

        return players.ToArray();
    }

    public void setScript(bool value)
    {
        //setActive to every selecter under it, as we can't disable this game object
        foreach (PlayerSelectionScript selecter in playerSelecters)
        {
            selecter.gameObject.SetActive(value);
        }
    }

    public bool checkReady()
    {
        //returns true only if all active players are ready, and more than 2 players are ready
        bool ready = true;

        foreach (PlayerSelectionScript playerSelectionScript in playerSelecters)
        {
            if (playerSelectionScript.getActive() && !playerSelectionScript.getReady())
                ready = false;
        }

        Player[] readyPlayers = getPlayers();

        if (readyPlayers.Length < 2)
            ready = false;

        return ready;
    }

    public void setSelectorToGrey(int id, bool status)
    {
        //set a selector to be greyed out and inactive or go back to normal
        playerSelecters[id - 1].setGrey(status);
    }

    public bool getSelectorActive(int id)
    {
        //returns weither or not the selector is active
        return playerSelecters[id - 1].getActive();
    }

    public bool getSelectorReady(int id)
    {
        //returns weither or not the selector is active
        return playerSelecters[id - 1].getReady();
    }
}
