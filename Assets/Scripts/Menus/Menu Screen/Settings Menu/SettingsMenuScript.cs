using UnityEngine;
using System.Collections;

public class SettingsMenuScript : MonoBehaviour {

    public GameObject mainMenu;

    //disabales the main menu when'ts enabled, and then renables the main menu and disables itself when we step out

    void OnEnable()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);
    }

    void OnDisable()
    {
        //close up shop
        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
    }
}
