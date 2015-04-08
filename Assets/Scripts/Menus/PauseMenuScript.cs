﻿using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour {

    private bool displayMenu = false;
    private bool quitMatch = false;

    private PlayerManagerBehaviour playerManager;

    private float previousTimeScale;

	// Use this for initialization
    void Start()
    {
        playerManager = GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>();	
	}
	
    void OnGUI()
    {
        if (displayMenu)
        {
            GUI.skin = playerManager.skin;

            GUI.Label(new Rect((Screen.width / 2) - 248, Screen.height / 9 + 2, 500, 50), "<size=30><color=black>PAUSED</color></siZE>");
            GUI.Label(new Rect((Screen.width / 2) - 250, Screen.height / 9, 500, 50), "<size=30><color=yellow>PAUSED</color></size>");

            //upper font
            GUI.Label(new Rect((Screen.width / 2) - 248, Screen.height / 3 + 2, 500, 50), "<size=15><color=black>     A (SPACE)       Y (ENTER)       B (BACKSPACE)</color></size>");
            GUI.Label(new Rect((Screen.width / 2) - 250, Screen.height / 3, 500, 50), "<size=15><color=white>     A (SPACE)       Y (ENTER)       B (BACKSPACE)</color></size>");

            //lower font
            GUI.Label(new Rect((Screen.width / 2) - 248, Screen.height / 2.5f + 2, 500, 50), "<size=20><color=black> QUIT        MENU        BACK</color></size>");
            GUI.Label(new Rect((Screen.width / 2) - 250, Screen.height / 2.5f, 500, 50), "<size=20><color=yellow> QUIT        MENU        BACK</color></size>");
        }
    }

	// Update is called once per frame
    void Update()
    {
        //if escape hit
        if (Input.GetButtonDown("Escape"))
        {
            //toggle menu
            displayMenu = !displayMenu;
        }

        //pause if menu is up
        if (displayMenu)
        {
            //log old time scale if not paused
            if (Time.timeScale > 0)
                previousTimeScale = Time.timeScale;

            //pause game
            Time.timeScale = 0;

            if (Input.GetButtonDown("Cancel"))
                displayMenu = false;

            if (Input.GetButtonDown("Submit"))
            {
#if UNITY_EDITOR || UNITY_WEBPLAYER
                //return to main menu
                Application.LoadLevel(0);  
#else
                Application.Quit();
#endif
            }
            if (Input.GetButtonDown("SwitchMode"))
            {
                quitMatch = true;
                displayMenu = false;
            }
        }
        else
        {
            //unpause
            if (Time.timeScale <= 0)
                Time.timeScale = previousTimeScale;

            if (quitMatch)
            {
                quitMatch = false;
                playerManager.endMatch();
            }
        }
    }
}
