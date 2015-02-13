using UnityEngine;
using System.Collections;

public class PlayerManagerBehaviour : MonoBehaviour {

    public bool debug;
    public GameObject player;
    public int firstTo = 5;
    public GUISkin skin;

    private Transform spawn1;
    private Transform spawn2;

    private GameObject p1;
    private GameObject p2;

    private PlayerControl p1Control;
    private PlayerControl p2Control;

    private PlayerFollower camera;

    private GameObject[] changingColorObjects;
    private GameObject[] fadingColorObjects;

    private int winner;
    private int countDownStart = 5;
    private int countDown;

	// Use this for initialization
    void Awake()
    {
        spawn1 = transform.FindChild("Spawn Point 1");
        spawn2 = transform.FindChild("Spawn Point 2");

        if (spawn1 == null || spawn2 == null)
        {
            UnityEngine.Debug.LogError("Cannot find spawn points under player manager!");
            this.enabled = false;
        }

        if (player == null)
        {
            UnityEngine.Debug.LogError("No player prefab given to the player manager!");
            this.enabled = false;
        }

        changingColorObjects = GameObject.FindGameObjectsWithTag("ChangeableColor");
        fadingColorObjects = GameObject.FindGameObjectsWithTag("FadingColor");
    }

    public void startGame()
    {
        //spawn 2 players and set them up
        p1 = Instantiate(player, spawn1.position, Quaternion.identity) as GameObject;
        p2 = Instantiate(player, spawn2.position, Quaternion.identity) as GameObject;

        //find camera and give it the players
        camera = Camera.main.GetComponent<PlayerFollower>();
        camera.setPlayers(p1.transform, p2.transform);

        p1Control = p1.GetComponent<PlayerControl>();
        p2Control = p2.GetComponent<PlayerControl>();

        camera.setDebug(debug);
        p1Control.setDebug(debug);
        p2Control.setDebug(debug);

        p1Control.setPlayer(1, 2, Color.red);
        p2Control.setPlayer(2, 1, Color.blue);
        
        p1Control.enabled = false;
        p2Control.enabled = false;

        //reseting values for when we start again
        Time.timeScale = 1;

        countDown = countDownStart;

        InvokeRepeating("countdown", 0, 1f);

        print(changingColorObjects.Length);
        foreach (GameObject obj in changingColorObjects)
        {
            obj.SendMessage("startFade");
        }

    }

    private void countdown()
    {
        //countdown to 0
        if (countDown > 0)
            countDown--;
        else
        {
            //wehen hit 0, set to -1 so go on screen disapears
            countDown = -1;
            CancelInvoke("countdown");
        }

        //checked at 0, so players are active on go
        if (countDown == 0)
        {
            p1Control.enabled = true;
            p2Control.enabled = true;
        }
    }

    void OnGUI()
    {
        GUI.skin = skin;

        if (countDown > 0)
            GUI.Label(new Rect((Screen.width / 2) - 25, Screen.height / 2, 50, 30), countDown.ToString());

        if (countDown == 0)
            GUI.Label(new Rect((Screen.width / 2) - 50, Screen.height / 2, 100, 30), "<color=yellow>GO!</color>");

        if (p1Control && p2Control)
        {
            GUI.Label(new Rect(Screen.width - 60, 50 / 10, 50, 30), p2Control.getScore().ToString());
            GUI.Label(new Rect(10, 50 / 10, 50, 30), p1Control.getScore().ToString());
        }

        if (winner != 0)            
            GUI.Label(new Rect((Screen.width / 2) - 250, Screen.height / 2,  500, 50), "Player " + winner + " has won!");
    }

    public void playerLost(int playerNo)
    {
        foreach (GameObject obj in fadingColorObjects)
        {
            obj.SendMessage("setFade", true);
        }

        Camera.main.gameObject.SendMessage("setFade", true);

        //increase score and check for winner
        print("Player " + playerNo + " was hit!");

        //if player 1 lost
        if (playerNo == p1Control.getPlayerNo())
        {
            camera.zoomInOnWinner(p2.transform);
            p2Control.increaseScore();
        }
        else
        {
            camera.zoomInOnWinner(p1.transform);
            p1Control.increaseScore();
        }
        

        winner = checkForWinner();

        if (winner != 0)
            endOfGame();
        else
            Invoke("respawn", 0.8f);
    }

    private void respawn()
    {
        Camera.main.gameObject.SendMessage("setFade", false);

        foreach (GameObject obj in fadingColorObjects)
        {
            obj.SendMessage("setFade", false);
        }

        camera.resetZoom();

        p1.transform.position = spawn1.position;
        p2.transform.position = spawn2.position;

        p1Control.enabled = true;
        p2Control.enabled = true;

        p1Control.respawn();
        p2Control.respawn();
    }

    private int checkForWinner()
    {
        if (p1Control.getScore() >= firstTo)
            return p1Control.getPlayerNo();

        if (p2Control.getScore() >= firstTo)
            return p2Control.getPlayerNo();

        return 0;
    }

    private void endOfGame()
    {
        //play sounds

        p1Control.enabled = false;
        p2Control.enabled = false;

        Invoke("reloadLevel", 1f);
    }

    private void reloadLevel()
    {
        Destroy(p1);
        Destroy(p2);
        winner = 0;
        GetComponent<MainMenu>().resetGame();
    }
}
