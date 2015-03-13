using UnityEngine;
using System.Collections;

public class PlayerSelectionMenuScript : MonoBehaviour {

    public GameObject playerSelectionManager;

    private PlayerManagerBehaviour playerManager;
    private PlayerSelectionScriptManager playerSelecterManager;
    private int stockCount;
    private string stockString = "4";

    private int playerCount;
    private string playerString = "4";

	// Use this for initialization
	void Awake ()
    {
        playerManager = GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>();

        if (playerManager == null)
        {
            UnityEngine.Debug.LogError("Player manager has not been set!");
            this.enabled = false;
        }
	}
	
    void OnGUI()
    {
        GUI.skin = playerManager.skin;

        GUI.Label(new Rect(2, 2, 150, 70), "<size=30><color=black>Players: </color></size>");
        GUI.Label(new Rect(0, 0, 150, 70), "<size=30><color=yellow>Players: </color></size>");

        playerString = GUI.TextField(new Rect(175, 30, 30, 30), playerString);
        int.TryParse(playerString, out playerCount);

        GUI.Label(new Rect(152, 2, 400, 70), "<size=30><color=black>Stocks: </color></size>");
        GUI.Label(new Rect(150, 0, 400, 70), "<size=30><color=yellow>Stocks: </color></size>");

        stockString = GUI.TextField(new Rect(450, 30, 30, 30), stockString);
        int.TryParse(stockString, out stockCount);
    }

	// Update is called once per frame
	void Update () {
	
	}
    
    public PlayerSprite[] getSprites()
    {
        return playerSelecterManager.getSprites();
    }

    public PlayerColor[] getColors()
    {
        return playerSelecterManager.getColors();
    }

    public int getStockAmount()
    {
        return stockCount;
    }

    public int getPlayerCount()
    {
        if (playerCount < 2)
            playerCount = 2;

        if (playerCount > 4)
            playerCount = 4;

        return playerCount;
    }

    public void setScript(bool value)
    {
        playerSelectionManager.SetActive(true);

        if (playerSelecterManager == null)
            playerSelecterManager = playerSelectionManager.GetComponent<PlayerSelectionScriptManager>();
        playerSelecterManager.setScript(value);
    }
}
