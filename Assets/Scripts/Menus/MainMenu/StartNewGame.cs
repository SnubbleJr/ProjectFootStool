using UnityEngine;
using System.Collections;

public class StartNewGame : MonoBehaviour {

    public string levelName;

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        //disable menu entry script
        this.GetComponent<MenuEntry>().enabled = false;

        //load level and fade
        GameObject.FindGameObjectWithTag("Fader").GetComponent<SceneFadeInOut>().changeScene(levelName);
    }
}
