using UnityEngine;
using System.Collections;

public class BrightnessSliderElement : MonoBehaviour {

    public static float brightness = 1f;
    
    private MenuScript menuScript;

    void Start()
    {
        menuScript = transform.parent.GetComponent<MenuScript>();
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        print("brightness was: " + brightness);
        //set the volume to our value, based on our distance :3
        brightness = (float)menuScript.getIndex(this.gameObject) / (float)(menuScript.getEntryLength() - 1);
        print("brightness is: " + brightness);
        transform.parent.SendMessage("finished");
    }
}
