using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public string versionNumber;

    private PlayerManagerBehaviour playerManager;
    private PlayerFollower playerFollower;

    private float cameraSize;
    private Vector3 cameraPos;
    private bool inGame = false;

    // Use this for initialization
	void Awake () 
    {
        cameraSize = Camera.main.orthographicSize;
        cameraPos = Camera.main.transform.position;
        playerManager = GetComponent<PlayerManagerBehaviour>();
        playerFollower = Camera.main.GetComponent<PlayerFollower>();
	}

    void OnGUI()
    {
        if (!inGame)
        {
            GUI.skin = playerManager.skin;

            GUI.Label(new Rect((Screen.width / 2) - 248, Screen.height / 2 + 2, 500, 50), "<size=30><color=black>PROJECTFOOTSTOOL</color></size><size=15><color=black> v " + versionNumber + "</color></size>");
            GUI.Label(new Rect((Screen.width / 2) - 250, Screen.height / 2, 500, 50), "<size=30>PROJECT<color=yellow>FOOT</color>STOOL</size><size=15><color=yellow> v " + versionNumber + "</color></size>");
        }
    }

	// Update is called once per frame
	void Update () 
    {
        if (!inGame)
        {
            float zoomSpeed = playerFollower.zoomSpeed;

            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, cameraSize, zoomSpeed * Time.deltaTime);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPos, zoomSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Submit"))
                startGame();
        }
	}

    private void startGame()
    {
        inGame = true;
        playerManager.enabled = true;
        playerFollower.enabled = true;
        playerManager.startGame();
        Camera.main.transform.rotation = Quaternion.identity;
    }

    public void resetGame()
    {
        inGame = false;
        playerManager.enabled = false;
        playerFollower.enabled = false;
    }
}
