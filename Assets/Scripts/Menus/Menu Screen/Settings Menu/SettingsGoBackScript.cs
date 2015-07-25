using UnityEngine;
using System.Collections;

public class SettingsGoBackScript : MonoBehaviour {

    public GameObject settingsMenu;

    //simple script that just asks for the users go head, then closes the menu

    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonDetected;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonDetected;
    }

    void buttonDetected(PlayerInputScheme player, string inputName, float value)
    {
        //quit out if no one is in selecter
        if (inputName == "Cancel")
            SendMessage("goTime");
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        if (settingsMenu != null)
            settingsMenu.SetActive(false);
    }
}
