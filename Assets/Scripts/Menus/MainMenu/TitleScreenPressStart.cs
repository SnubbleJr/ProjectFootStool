using UnityEngine;
using System.Collections;

public class TitleScreenPressStart : MonoBehaviour {

    private bool selected = false;
    
    //get the reset select evenr from Menu Script
    void OnEnable()
    {
        MenuScript.optionSelected += selectionCheck;
        MenuScript.resetSelect += resetSelected;
        selected = false;
    }

    void OnDisable()
    {
        MenuScript.optionSelected -= selectionCheck;
        MenuScript.resetSelect -= resetSelected;
        selected = false;
    }
        
    void selectionCheck()
    {
        //if we are selected
        if (selected)
        {
            //send a message to a script in the object to get moving
            this.SendMessage("goTime");
        }
    }

    //resetselected is just a speical case of setselected
    void resetSelected()
    {
        setSelected(false);
    }

    //sets the selectiong marker for the entry
    public void setSelected(bool val)
    {
        selected = val;
    }
}
