using UnityEngine;
using System.Collections;

public class ConfirmMenuScript : MonoBehaviour {

    private PauseOption currentOption;

    private PauseMenuScript pauseMenuScript;

    void Awake()
    {
        pauseMenuScript = GetComponentInParent<PauseMenuScript>();
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
        //handel controls if in focus
        switch (input)
        {
            case "Cancel":
                exit(PauseOption.cancel);
                break;
            case "Submit":
                exit(currentOption);
                break;
        }
    }

    private void exit(PauseOption option)
    {
        pauseMenuScript.confrimationResult(option);
    }

    public void getConfirmation(PauseOption option)
    {
        currentOption = option;

        //display correct confirmation text
        string text = "";
        switch (option)
        {
            case PauseOption.exitMatch:
                text = "End Match?";
                break;
            case PauseOption.exitGame:
                text = "Quit Game?";
                break;
        }
        GetComponentInChildren<UITextScript>().setText(text);
    }
}
