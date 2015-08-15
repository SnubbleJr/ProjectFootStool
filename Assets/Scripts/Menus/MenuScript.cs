using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuScript : MonoBehaviour {

    public string navigateAxis;
    public bool invert;
    public bool allowEntryFocus = false;
    public SFX upSound = SFX.MenuUp;
    public SFX downSound = SFX.MenuDown;
    public SFX selectSound = SFX.Submit;

    public bool menuRollOver = true;
    public int scrollSpeedMin = 60, scrollSpeedMax = 15; //the number of frames between selecting

    public delegate void MenuScriptDelegate();
    public static event MenuScriptDelegate resetSelect;
    public static event MenuScriptDelegate optionSelected;

    private GameObject[] menuEntries;
    private GameObject[] activeMenuEntries;

    private int selected = 0;
    private int currentScrollSpeedMin = 0; //slowly increased while a direction is being pushed
    private int currentScrollSpeed = 0; //adds 1 every frame that an input is held
    private bool canScroll = false; //bool used to inhibit sanic levels of scrolling
    private bool controllable = true;
    private bool ignoreMouseInput = true;

	// Use this for initialization
	void Awake ()
    {
        getMenuEntries();

        //if no entries, disable
        if (getActiveGO().Length <= 0)
            this.enabled = false;

        //set scroll speed back to min
        currentScrollSpeed = scrollSpeedMin;

        //set current scroll min to min
        currentScrollSpeedMin = scrollSpeedMin;

        //update active GO list
        activeMenuEntries = getActiveGO();

        //set first entry to be selected
        activeMenuEntries[selected].SendMessage("setSelected", true);
    }

    private void getMenuEntries()
    {
        List<GameObject> entries = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("MenuEntry"))
                entries.Add(child.gameObject);
        }

        menuEntries = entries.ToArray();
    }

    void OnEnable()
    {
        InputManagerBehaviour.axisPressed += axisDetected;
        InputManagerBehaviour.buttonPressed += buttonDetected;
    }

    void OnDisable()
    {
        InputManagerBehaviour.axisPressed -= axisDetected;
        InputManagerBehaviour.buttonPressed -= buttonDetected;
    }

    void axisDetected(PlayerInputScheme player, string inputName, float value)
    {
        //if we're not being overidden, and we've detected the input we want
        if (controllable && inputName == navigateAxis && menuEntries != null && value != 0)
        {
            if (value == 10)
                value = 0;

            //handle inputs
            upDownSelection(value);
        }
    }

    void buttonDetected(PlayerInputScheme player, string inputName, float value)
    {
        //tell sliders taht we have started to detect buttons
        SendMessage("finishedAwake", SendMessageOptions.DontRequireReceiver);

        //check for option select - done in here and not in the menu entry for consolidation sake
        //if the option is selected
        if (controllable && inputName == "Submit")
            selectOption();
    }

    GameObject[] getActiveGO()
    {
        if (menuEntries == null)
            getMenuEntries();

        List<GameObject> tmpList = new List<GameObject>();

        //go through menu entries, and add the non greyout ones to activeMenuEntries, so disabled options aren't suedo selected
        foreach (GameObject entry in menuEntries)
        {
            try
            {
                //if entry is not greyed out, then add it
                if (!entry.GetComponent<MenuEntry>().greyedOut)
                    tmpList.Add(entry);
            }
            catch
            {
                //add it if it's another form of menu entry
                tmpList.Add(entry);
            }
        }

        return tmpList.ToArray();
    }

    void upDownSelection(float input)
    {
        //handles the option selection

        //update active GO list
        activeMenuEntries = getActiveGO();
        //if we can scroll
        if (canScroll)
        {
            //check for userinput
            checkUpDownInput(input);

            //while we're scrolling
            //decrease current min by 10%, unitil it's at max
            //this give the overall effect of increasing how frequently canScroll is activated - it speeds up slow at first, then faster
            //done within can scroll so that the whole process is exponential in a way
            if (currentScrollSpeedMin >= scrollSpeedMax)
                currentScrollSpeedMin = (int)(currentScrollSpeedMin * 0.9);
        }

        if (input == 0)
        {
            //nothing pressed

            //set scroll speed back to min
            currentScrollSpeed = scrollSpeedMin;

            //set current scroll min to min
            currentScrollSpeedMin = scrollSpeedMin;
        }
        else
        {
            //ignore mouse untill it moves
            ignoreMouseInput = true;
            Cursor.visible = false;      
        }

        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            //if mouse has moved
            ignoreMouseInput = false;
            Cursor.visible = true;            
        }

        //increas current Scroll Speed by 1
        currentScrollSpeed++;

        //speed checking, if the current speed is over the current min, then set scrollable to true and reset current speed
        //this means that we can move manually as fast as we want
        if (currentScrollSpeed > currentScrollSpeedMin)
        {
            canScroll = true;
            currentScrollSpeed = 0;
        }
    }

    private void selectOption()
    {
        //send the message
        if (optionSelected != null)
        {
            playSound(selectSound);
            optionSelected();
            if (allowEntryFocus)
                greyOutOtherEntries();
        }
    }

    public void entryFinished()
    {
        if (allowEntryFocus)
            unGreyOutOtherEntries();
    }

    private void greyOutOtherEntries()
    {
        for (int i = 0; i < activeMenuEntries.Length; i++)
            if (i != selected)
                activeMenuEntries[i].GetComponent<MenuEntry>().setGreyedOut(true);
    }

    private void unGreyOutOtherEntries()
    {
        for (int i = 0; i < activeMenuEntries.Length; i++)
            activeMenuEntries[i].GetComponent<MenuEntry>().setGreyedOut(false);
    }

    void setEntrySelect()
    {
        //unset the old menu entry
        if (resetSelect != null)
            resetSelect();

        if (activeMenuEntries == null)
            activeMenuEntries = getActiveGO();

        //set new menu entry selected to be highlighted
        activeMenuEntries[selected].SendMessage("setSelected", true);
    }

    void checkUpDownInput(float input)
    {
        //checks for input, stops sanic scrolling and plays appropiate sound

        if (input > 0)
        {
            //up pressed

            //decrease selected by 1, will wrap around if set
            //if we're not at the limit (0), or wrap set
            if ((selected > 0) || menuRollOver)
            {
                //it's all clear
                //decrease selected
                selected = (selected + (activeMenuEntries.Length - 1)) % activeMenuEntries.Length;
                playSound(upSound);
            }
        }

        if (input < 0)
        {
            //down pressed

            //increase selected by 1, will auto wrap around if set
            //if we're not at the limit (length), or wrap set
            if ((selected < (activeMenuEntries.Length - 1)) || menuRollOver)
            {
                //it's all clear
                //increase selected
                selected = (selected + 1) % activeMenuEntries.Length;
                playSound(downSound);
            }
        }

        if (input != 0)
            //something pressed
            setEntrySelect();
               
        //stop scrolling
        canScroll = false;
    }

    void playSound(SFX sound)
    {
        //if the clip exists
        SFXManagerBehaviour.Instance.playSound(sound);
    }

    //here, a menu option can overide us
    public void setControllable(bool val)
    {
        controllable = val;
    }

    public void entryHoveredOver(GameObject entry)
    {
        if (!ignoreMouseInput && controllable)
        {
            //when mouse hovers over option, select it
            int hoveredEntry = -1;
            for (int i = 0; i < activeMenuEntries.Length; i++)
            {
                if (activeMenuEntries[i].Equals(entry))
                    hoveredEntry = i;
            }

            if (hoveredEntry > -1)
            {
                selected = hoveredEntry;
                setEntrySelect();
            }
        }
    }

    public void entryClicked()
    {
        if (controllable)
            selectOption();
    }

    public void setIndex(int index)
    {
        //override so we get get a defualt value set to the menu if desired
        selected = index;
        setEntrySelect();
    }

    public int getIndex(GameObject entry)
    {
        int index = 0;
        for (int i = 0; i < activeMenuEntries.Length; i++)
        {
            if (activeMenuEntries[i].Equals(entry))
                index = i;
        }

        return index;
    }

    public int getEntryLength()
    {
        if (menuEntries == null)
        {
            List<GameObject> entries = new List<GameObject>();
            foreach (Transform child in transform)
            {
                if (child.CompareTag("MenuEntry"))
                    entries.Add(child.gameObject);
            }

            menuEntries = entries.ToArray();
        }
        return menuEntries.Length;
    }

    public int getCurrentEntry()
    {
        return selected;
    }

    public SFX getMenuUpSound()
    {
        return upSound;
    }

    public SFX getMenuDownSound()
    {
        return downSound;
    }

    public SFX getSelectSound()
    {
        return selectSound;
    }
}
