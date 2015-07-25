using UnityEngine;
using System.Collections;

public class VolumeSliderElement : MonoBehaviour {

    public static float volume = 1f;

    public delegate void VolumeSliderDelegate();
    public static event VolumeSliderDelegate volumeChanged;

    private MenuScript menuScript;

    void Start()
    {
        menuScript = transform.parent.GetComponent<MenuScript>();
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        //set the volume to our value, based on our distance :3
        volume = (float) menuScript.getIndex(this.gameObject) / (float) (menuScript.getEntryLength() - 1);
        transform.parent.SendMessage("finished");

        if (volumeChanged != null)
            volumeChanged();
    }

    //set volume if hovered over
    public void setSelected(bool selected)
    {
        if (selected)
            volume = (float)menuScript.getIndex(this.gameObject) / (float)(menuScript.getEntryLength() - 1);

        if (volumeChanged != null)
            volumeChanged();
    }
}
