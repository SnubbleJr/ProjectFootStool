using UnityEngine;
using System.Collections;

public class SFXVolumeSliderElement : MonoBehaviour {

    public static float volume = 1f;

    public delegate void SFXSliderDelegate(bool save);
    public static event SFXSliderDelegate volumeChanged;

    private MenuScript menuScript;
    private SFX SFXSound;

    void Awake()
    {
        menuScript = transform.parent.GetComponent<MenuScript>();
        SFXSound = menuScript.getMenuUpSound();
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        //set the volume to our value, based on our distance :3
        volume = 1 - (float)menuScript.getIndex(this.gameObject) / (float)(menuScript.getEntryLength());
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
            //set the volume to our value, based on our distance :3
            volume = 1 - (float)menuScript.getIndex(this.gameObject) / (float)(menuScript.getEntryLength());

            SFXManagerBehaviour.Instance.playSound(SFXSound);

            if (volumeChanged != null)
                volumeChanged(false);
        }
    }
}
