using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
            x = 1.4f;
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

	// Update is called once per frame
	void Update () {
	
	}

    public PlayerSprite[] getSprites()
    {
        List<PlayerSprite> sprites = new List<PlayerSprite>();

        foreach (PlayerSelectionScript playerSelecter in playerSelecters)
            sprites.Add(playerSelecter.getSprite());
        
        return sprites.ToArray();
    }

    public PlayerColor[] getColors()
    {
        List<PlayerColor> colors = new List<PlayerColor>();

        foreach (PlayerSelectionScript playerSelecter in playerSelecters)
        colors.Add(playerSelecter.getColor());

        return colors.ToArray();
    }

    public void setScript(bool value)
    {
        //setSctive to every selecter under it, as we can't disable this game object
        foreach (PlayerSelectionScript selecter in playerSelecters)
        {
            selecter.gameObject.SetActive(value);
        }
    }
}
