using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

    public GameMode gameMode;

    private MainMenu mainMenu;

    void Start()
    {
        mainMenu = GameObject.Find("Menu Manager").GetComponent<MainMenu>();
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        mainMenu.setGameMode(gameMode);
    }
}
