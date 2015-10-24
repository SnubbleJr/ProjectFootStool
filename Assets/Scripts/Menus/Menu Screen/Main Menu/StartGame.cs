using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

    public GameMode gameMode;

    private MainMenuScript mainMenu;

    void Start()
    {
        mainMenu = GameObject.Find("Menu Manager").GetComponent<MainMenuScript>();
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        if (mainMenu == null)
            mainMenu = GameObject.Find("Menu Manager").GetComponent<MainMenuScript>();

        mainMenu.setGameMode(gameMode);
    }
}
