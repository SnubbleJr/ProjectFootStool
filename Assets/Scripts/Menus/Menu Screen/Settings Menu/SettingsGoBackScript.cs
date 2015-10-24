using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MenuEntry))]

public class SettingsGoBackScript : MonoBehaviour {

    public GameObject menuToDisable;
    public GameObject menuToEnable; 

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
        if (inputName == player.inputs[PlayerInput.CancelInput].shortName.ToString())
            if (menuEntry)
            {
                if (!menuEntry.getGreyedOut())
                    goTime();
            }
            else
                goTime();
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        menuToDisable.SetActive(false);
        menuToEnable.SetActive(true);
        menuToDisable.GetComponent<MenuScript>().entryFinished();
    }
}
