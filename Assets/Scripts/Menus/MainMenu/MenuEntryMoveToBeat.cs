using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MoveToBeat))]
public class MenuEntryMoveToBeat : MonoBehaviour {

    //moves selected entries, looks like they're pulsing due to their constant lerp

    private MenuEntry menuEntry;
    private MoveToBeat moveToBeat;
    private bool prevSelectVal;

	// Use this for initialization
	void Start () 
    {
        menuEntry = GetComponent<MenuEntry>();
        moveToBeat = GetComponent<MoveToBeat>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //only move if selected
        //update mover with our current status
        if (prevSelectVal != menuEntry.getSelected())
        {
            prevSelectVal = menuEntry.getSelected();
            moveToBeat.setMove(prevSelectVal);
        }
	}
}
