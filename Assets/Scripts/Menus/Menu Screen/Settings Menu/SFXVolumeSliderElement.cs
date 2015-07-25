using UnityEngine;
using System.Collections;

public class SFXVolumeSliderElement : MonoBehaviour {

    public static float volume = 1f;
    
    private MenuScript menuScript;
    private AudioClip SFXSound;

    void Start()
    {
        menuScript = transform.parent.GetComponent<MenuScript>();
        SFXSound = menuScript.getSelectSound();
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        //jump out
        transform.parent.SendMessage("finished");
    }

    //set volume if hovered over
    public void setSelected(bool selected)
    {
        if (selected)
        {
            //set the volume to our value, based on our distance :3
            volume = (float)menuScript.getIndex(this.gameObject) / (float)(menuScript.getEntryLength() - 1);

            //if the clip exists
            if (SFXSound != null)
            {
                AudioSource.PlayClipAtPoint(SFXSound, Camera.main.transform.position, volume);
            }
        }
    }
}
