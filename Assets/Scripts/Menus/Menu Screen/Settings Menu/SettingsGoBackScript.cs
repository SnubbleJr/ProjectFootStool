using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MenuEntry))]

public class SettingsGoBackScript : MonoBehaviour {

    public GameObject settingsMenu;

    private MenuEntry menuEntry;

    //simple script that just asks for the users go head, then closes the menu

    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonDetected;
        menuEntry = GetComponent<MenuEntry>();
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonDetected;
    }

    void buttonDetected(PlayerInputScheme player, string inputName, float value)
    {
        //quit out if no one is in selecter
        if (inputName == "Cancel")
            if (menuEntry)
            {
                if (!menuEntry.getGreyedOut())
                    SendMessage("goTime");
            }
            else
                SendMessage("goTime");
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        if (settingsMenu)
        {
            settingsMenu.GetComponent<MenuScript>().entryFinished();
            settingsMenu.SetActive(false);
        }
    }
}
