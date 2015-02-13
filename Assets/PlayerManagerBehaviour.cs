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

    private int countDown = 5;

	// Use this for initialization
    void Start()
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

        p1Control.setPlayerNo(1, 2);
        p2Control.setPlayerNo(2, 1);

        p1Control.enabled = false;
        p2Control.enabled = false;

        InvokeRepeating("countdown", 0, 1f);
    }

    private void countdown()
    {
        if (countDown > 1)
            countDown--;
        else
        {
            countDown = 0;
            CancelInvoke("coundown");
            p1Control.enabled = true;
            p2Control.enabled = true;
        }
    }

    void OnGUI()
    {
        GUI.skin = skin;

        if (countDown != 0)
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2,  30, 30), countDown.ToString());

        if (p1Control && p2Control)
        {
            GUI.Label(new Rect(Screen.width - 60, 50 / 10, 50, 30), p2Control.getScore().ToString());
            GUI.Label(new Rect(10, 50 / 10, 50, 30), p1Control.getScore().ToString());
        }
    }

    public void playerLost(int playerNo)
    {
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
        
        Invoke("respawn", 1f);

        int winner = checkForWinner();

        if (winner != 0)
            endOfGame(winner);
    }

    private void respawn()
    {
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

    private void endOfGame(int winner)
    {
        print("Player " + winner + " has won!");

        //play sounds
    }
}
