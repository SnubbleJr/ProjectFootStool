using UnityEngine;
using System.Collections;

public class SliderMenuEntry : MonoBehaviour {

    //workd ontop of menu entry, disables updown of menu script for our own needs

    public bool selected = false;

    void Start()
    {
        //set all the children to be a slider menu entry so it doesn't move
        foreach (Transform child in transform)
            child.GetComponent<MenuEntry>().setSliderEntry(true);
    }
    	
    //called with invoke, so eveything works in order
    void activate()
    {
        selected = true;

        //activate menu of our own
        this.GetComponent<MenuScript>().enabled = true;

        //send a message to the parent menu system that we're in controll now, and don't act apon button presses 
        transform.parent.gameObject.SendMessage("setControllable", false);

        //disable menu entry script
        this.GetComponent<MenuEntry>().enabled = false;
    }

    //called with invoke, so eveything works in order
    void deactivate()
    {
        //set selected
        selected = false;

        //renable our menu entry script
        this.GetComponent<MenuEntry>().enabled = true;

        //give control back to the menu after we're done
        transform.parent.gameObject.SendMessage("setControllable", true);

        //deactivate menu of our own
        this.GetComponent<MenuScript>().enabled = false;
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        //set selected   

        //exit on input (if selected!)
        Invoke("activate", 0);
    }

    public void finished()
    {
        Invoke("deactivate", 0);
    }
}
