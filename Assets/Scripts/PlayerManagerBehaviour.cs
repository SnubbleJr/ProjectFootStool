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

    private PlayerFollower cameraPlayerFollower;

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

        fadingColorObjects = GameObject.FindGameObjectsWithTag("FadingColor");
    }

    public void PMBstartGame()
    {
        GameObject[] changingColorObjects = GameObject.FindGameObjectsWithTag("ChangeableColor");

        //find camera
        cameraPlayerFollower = Camera.main.GetComponent<PlayerFollower>();
        cameraPlayerFollower.setDebug(debug);

        spawnPlayers();

        if (Network.connections.Length == 0)
            cameraPlayerFollower.enabled = true;

        //reseting values for when we start again
        Time.timeScale = 1;

        countDown = countDownStart;

        InvokeRepeating("countdown", 0, 1f);

        foreach (GameObject obj in changingColorObjects)
        {
            obj.SendMessage("startFade");
        }

    }

    public void spawnPlayers()
    {
        Color cameaBG = Camera.main.backgroundColor;
        Color inverseCameraBg = new Color(1.0f - cameaBG.r, 1.0f - cameaBG.g, 1.0f - cameaBG.b);

        //spawn 2 players and set them up
        if (Network.connections.Length == 0)
        {
            p1 = Network.Instantiate(player, spawn1.position, Quaternion.identity, 0) as GameObject;
            p1Control = p1.GetComponent<PlayerControl>();
            p1Control.setDebug(debug);
            p1Control.setPlayer(1, 2, inverseCameraBg);
            p1Control.enabled = false;

            cameraPlayerFollower.setPlayer(p1.transform, 1);
        }
        else
        {
            p2 = Network.Instantiate(player, spawn2.position, Quaternion.identity, 0) as GameObject;
            p2Control = p2.GetComponent<PlayerControl>();
            p2Control.setDebug(debug);
            p2Control.setPlayer(2, 1, inverseCameraBg);
            p2Control.enabled = false;

            cameraPlayerFollower.setPlayer(p2.transform, 2);
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
            if (p1 != null)
                p1Control.enabled = true;
            if (p2 != null)
                p2Control.enabled = true;
        }
    }

    void OnGUI()
    {
        GUI.skin = skin;

        if (countDown > 0)
        {
            GUI.Label(new Rect((Screen.width / 2) - 23, Screen.height / 2 + 2, 50, 30), "<color=black>" + countDown.ToString() + "</color>");
            GUI.Label(new Rect((Screen.width / 2) - 25, Screen.height / 2, 50, 30), "<color=white>" + countDown.ToString() + "</color>");
        }

        if (countDown == 0)
        {
            GUI.Label(new Rect((Screen.width / 2) - 48, Screen.height / 2 + 2, 100, 30), "<color=black>GO!</color>");
            GUI.Label(new Rect((Screen.width / 2) - 50, Screen.height / 2, 100, 30), "<color=yellow>GO!</color>");
        }

        if (p1Control && p2Control)
        {
            GUI.Label(new Rect(Screen.width - 58, 52 / 10, 50, 30), "<color=black>" + p2Control.getScore().ToString() + "</color>");
            GUI.Label(new Rect(Screen.width - 60, 50 / 10, 50, 30), "<color=white>" + p2Control.getScore().ToString() + "</color>");

            GUI.Label(new Rect(8, 52 / 10, 50, 30), "<color=black>" + p1Control.getScore().ToString() + "</color>");
            GUI.Label(new Rect(10, 50 / 10, 50, 30), "<color=white>" + p1Control.getScore().ToString() + "</color>");
        }

        if (winner != 0)
        {
            GUI.Label(new Rect((Screen.width / 2) - 248, Screen.height / 2 + 2, 500, 50), "<color=black>Player " + winner + " has won!</color>");
            GUI.Label(new Rect((Screen.width / 2) - 250, Screen.height / 2, 500, 50), "<color=white>Player " + winner + " has won!</color>");
        }
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
            cameraPlayerFollower.zoomInOnWinner(p2.transform);
            p2Control.increaseScore();
        }
        else
        {
            cameraPlayerFollower.zoomInOnWinner(p1.transform);
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

        cameraPlayerFollower.resetZoom();

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
