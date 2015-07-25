using UnityEngine;
using System.Collections;

public enum PauseOption {cancel, exitMatch, exitGame};

public class PauseMenuScript : MonoBehaviour {

    public GameObject pauseCanvas;
    public GameObject confirmCanvas;

    private bool active = false;
    private bool inFocus = true;

    private PlayerManagerBehaviour playerManager;

    private float previousTimeScale;

	// Use this for initialization
    void Start()
    {
        playerManager = GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>();	
	}

    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonPressed;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonPressed;
    }

    private void buttonPressed(PlayerInputScheme player, string input, float value)
    {
        //if escape hit
        if (input == "Pause")
            //toggle menu
            active = !active;

        if (active)
        {
            pause();

            //handel controls if in focus
            if (inFocus)
            {
                switch (input)
                {
                    case "Cancel":
                        active = false;
                        break;
                    case "ChangeMode":
                        askConfirmation(PauseOption.exitMatch);
                        break;
                    case "Submit":
                        askConfirmation(PauseOption.exitGame);
                        break;
                }
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
    }

    private void unpause()
    {
        //hide menu
        pauseCanvas.SetActive(false);

        //hide confirm
        confirmCanvas.SetActive(false);
        
        active = false;
        inFocus = true;

        Time.timeScale = previousTimeScale;
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
        playerManager.endMatch();
    }

    private void quitGame()
    {
#if UNITY_EDITOR || SwitchMode
        unpause();
        Application.LoadLevel(0);
#else
                Application.Quit();
#endif
    }
}
