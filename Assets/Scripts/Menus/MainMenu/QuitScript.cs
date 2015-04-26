using UnityEngine;
using System.Collections;

public class QuitScript : MonoBehaviour {

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        print("QUIT!");
        Application.Quit();
    }
}
