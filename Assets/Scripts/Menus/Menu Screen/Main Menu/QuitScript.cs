using UnityEngine;
using System.Collections;

public class QuitScript : MonoBehaviour {

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_WEBGL
        Application.LoadLevel(0);
#else
                Application.Quit();
#endif
    }
}
