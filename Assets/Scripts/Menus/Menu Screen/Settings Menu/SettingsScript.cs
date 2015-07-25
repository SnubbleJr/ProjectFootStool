using UnityEngine;
using System.Collections;

public class SettingsScript : MonoBehaviour {

    public GameObject settingsMenu;

    //enables the settings menu when activated
   
    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        if (settingsMenu != null)
            settingsMenu.SetActive(true);
    }
}
