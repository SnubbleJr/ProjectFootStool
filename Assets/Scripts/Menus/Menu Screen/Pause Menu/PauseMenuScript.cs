using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PauseOption {cancel, exitMatch, exitGame};

public class PauseMenuScript : MonoBehaviour {

    public GameObject pauseCanvas;
    public GameObject confirmCanvas;
    public GameObject dcInfoText;

    private bool active = false;
    private bool inFocus = true;
    private bool lockedInPause = false;     //for when we have a DC'd player

    private PlayerManagerBehaviour playerManager;

    private float previousTimeScale;

    private List<PlayerInputScheme> removedPlayerInputSchemes = new List<PlayerInputScheme>();

    void Awake()
    {
        previousTimeScale = Time.timeScale;
    }

	// Use this for initialization
    void Start()
    {
        playerManager = GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>();	
	}

    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonPressed;
        InputManagerBehaviour.playerAdded += playerAdded;
        InputManagerBehaviour.playerRemoved += playerRemoved;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonPressed;
        InputManagerBehaviour.playerAdded -= playerAdded;
        InputManagerBehaviour.playerRemoved -= playerRemoved;
    }

    private void playerAdded(PlayerInputScheme scheme)
    {
        List<PlayerInputScheme> temp = new List<PlayerInputScheme>(removedPlayerInputSchemes);

        foreach (PlayerInputScheme playerInputScheme in temp)
            if (playerInputScheme.id == scheme.id)
                removedPlayerInputSchemes.Remove(playerInputScheme);

        playerDissconnectionCheck();
    }

    private void playerRemoved(PlayerInputScheme scheme)
    {
        removedPlayerInputSchemes.Add(scheme);
        playerDissconnectionCheck();
    }

    private void playerDissconnectionCheck()
    {
        //force pause is someone DC'd
        string str = printDisconnectedPlayers();
        bool playerDCd = (!str.Equals(""));

        if (playerDCd)
        {
            active = true;
            pause();
        }

        lockedInPause = playerDCd;
        dcInfoText.GetComponent<UITextScript>().setText(str);
    }

    private string printDisconnectedPlayers()
    {
        //returns a string containing all of the players and the controls they belong to
        string str = "";
        
        foreach (PlayerInputScheme playerInputScheme in removedPlayerInputSchemes)
            if (hasBeenRemoved(playerInputScheme.id))
                str += "Player " + playerInputScheme.id + " [Controller " + playerInputScheme.controller + "] ";

        if (!str.Equals(""))
            str = "Please reconnect " + str;

        return str;
    }

    private bool hasBeenRemoved(int id)
    {
        foreach (PlayerInputScheme playerInputScheme in removedPlayerInputSchemes)
            if (playerInputScheme.id == id)
                return true;
        return false;
    }

    private void buttonPressed(PlayerInputScheme player, string input, float value)
    {
        //if escape hit
        if (input == player.inputs[PlayerInput.PauseInput].shortName)
            //if already active
            if (!lockedInPause)
                //toggle menu
                active = !active;

        if (active)
        {
            //handel controls (and show pausse screen) if in focus
            if (inFocus)
            {
                pause();

                if (input == player.inputs[PlayerInput.CancelInput].shortName && !lockedInPause)
                    active = false;

                if (input == player.inputs[PlayerInput.ChangeModeInput].shortName)
                    askConfirmation(PauseOption.exitMatch);

                if (input == player.inputs[PlayerInput.SubmitInput].shortName)
                    askConfirmation(PauseOption.exitGame);
            }
        }

        if (!active)
            //close menus
            unpause();
    }

    private void pause()
    {
        //log old time scale if not paused
        if (Time.timeScale > 0)
            previousTimeScale = Time.timeScale;

        //pause game
        Time.timeScale = 0;

        //show menu
        pauseCanvas.SetActive(true);

        //informm music manager
        MusicManagerBehaviour.Instance.setPaused(true);
    }

    private void unpause()
    {
        //hide menu
        pauseCanvas.SetActive(false);

        //hide confirm
        confirmCanvas.SetActive(false);
        
        active = false;
        inFocus = true;
        lockedInPause = false;

        Time.timeScale = previousTimeScale;

        //informm music manager
        MusicManagerBehaviour.Instance.setPaused(false);
    }

    private void askConfirmation(PauseOption option)
    {
        //hide menu
        pauseCanvas.SetActive(false);

        //show confirm
        confirmCanvas.SetActive(true);

        inFocus = false;

        //wait for confirmationMenu to return to us
        GetComponentInChildren<ConfirmMenuScript>().getConfirmation(option);
    }

    public void confrimationResult(PauseOption option)
    {
        //hide confirm
        confirmCanvas.SetActive(false);

        inFocus = true;

        switch (option)
        {
            case PauseOption.cancel:
                //go back, back to square one, show menu
                pauseCanvas.SetActive(true);
                break;
            case PauseOption.exitMatch:
                endmatch();
                break;
            case PauseOption.exitGame:
                quitGame();
                break;
        }
    }

    private void endmatch()
    {
        unpause();
        playerManager.StopMatch();
    }

    private void quitGame()
    {
        unpause();
#if UNITY_EDITOR || SwitchMode || UNITY_WEBPLAYER || UNITY_WEBGL
        Application.LoadLevel(0);
#else
        Application.Quit();
#endif
    }
}
