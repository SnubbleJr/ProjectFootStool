using UnityEngine;
using System.Collections;

public class MusicVolumeSliderElement : MonoBehaviour {

    public static float volume = 1f;

    public delegate void MusicVolumeSliderDelegate(bool save);
    public static event MusicVolumeSliderDelegate volumeChanged;

    private MenuScript menuScript;

    void Awake()
    {
        menuScript = transform.parent.GetComponent<MenuScript>();
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        //set the volume to our value, based on our distance :3
        volume = 1 - (float) menuScript.getIndex(this.gameObject) / (float) (menuScript.getEntryLength());
        transform.parent.SendMessage("finished");

        if (volumeChanged != null)
            volumeChanged(true);
    }

    //set volume if hovered over
    public void setSelected(bool selected)
    {
        if (!menuScript)
            menuScript = transform.parent.GetComponent<MenuScript>();

        if (selected)
        {
            volume = 1 - (float)menuScript.getIndex(this.gameObject) / (float)(menuScript.getEntryLength());

            if (volumeChanged != null)
                volumeChanged(false);
        }
    }
}
