using UnityEngine;
using System.Collections;

public enum Setting
{
    MasterVol,
    MusicVol,
    SFXVol
}

public class SliderMenuEntry : MonoBehaviour
{

    //workd ontop of menu entry, disables updown of menu script for our own needs

    public bool selected = false;
    public Setting setting;

    void Awake()
    {
        //tell our menuscript we nee to set selecteded to the setting
        int selectedEntry = getSettingIndex();
        
        GetComponent<MenuScript>().setIndex(selectedEntry);

        for (int i = 0; i < transform.GetChildCount(); i++)
        {
            Transform child = transform.GetChild(i);
            //set all the children to be a slider menu entry so it doesn't move - does nothing atm
            child.GetComponent<MenuEntry>().setSliderEntry(true);
            //disable them to start with so they retain their selectoin value
            child.GetComponent<MenuEntry>().enabled = false;
        }
    }

    private int getSettingIndex()
    {
        //return entry index of set setting
        float volume = 1;

        switch (setting)
        {
            case Setting.MasterVol:
                volume = MasterVolumeSliderElement.volume;
                break;
            case Setting.MusicVol:
                volume = MusicVolumeSliderElement.volume;
                break;
            case Setting.SFXVol:
                volume = SFXVolumeSliderElement.volume;
                break;
        }

        float entryLength = GetComponent<MenuScript>().getEntryLength();
        float index = (1 - volume) * entryLength;
        return (int)index;
    }

    void Update()
    {
        foreach (Transform child in transform)
            //grey out children if we are greyd out
            child.GetComponent<MenuEntry>().setGreyedOut(this.GetComponent<MenuEntry>().getGreyedOut());
    }

    //called with invoke, so eveything works in order
    void activate()
    {
        selected = true;

        //activate menu of our own
        this.GetComponent<MenuScript>().enabled = true;

        //send a message to the parent menu system that we're in controll now, and don't act apon button presses 
        transform.parent.GetComponent<MenuScript>().setControllable(false);

        //disable menu entry script
        this.GetComponent<MenuEntry>().enabled = false;

        foreach (Transform child in transform)
            child.GetComponent<MenuEntry>().enabled = true;
    }

    //called with invoke, so eveything works in order
    void deactivate()
    {
        //set selected
        selected = false;

        //renable our menu entry script
        this.GetComponent<MenuEntry>().enabled = true;

        //give control back to the menu after we're done
        transform.parent.GetComponent<MenuScript>().entryFinished();
        transform.parent.GetComponent<MenuScript>().setControllable(true);

        //deactivate menu of our own
        this.GetComponent<MenuScript>().enabled = false;

        foreach (Transform child in transform)
            //disable them so they retain their selectoin value
            child.GetComponent<MenuEntry>().enabled = false;
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        //set selected   

        //exit on input (if selected!)
        activate();
    }

    public void finished()
    {
        deactivate();
    }
}
