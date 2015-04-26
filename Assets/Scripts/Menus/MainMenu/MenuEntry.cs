using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuEntry : MonoBehaviour {

    public Color highlightedColor = Color.red;
    public bool greyedOut = false;

    public delegate void MenuEntryDelegate(bool selected);
    public static event MenuEntryDelegate selectDelegate;
    public static event MenuEntryDelegate selectDelegateGUI;

    private bool sliderEntry = false;
    private bool selected = false;

    private Text guiText;
    private Color defaultColor;

    void Start()
    {
        guiText = GetComponent<Text>();
        defaultColor = guiText.color;
    }

    //get the reset select evenr from Menu Script
    void OnEnable()
    {
        MenuScript.optionSelected += selectionCheck;
        MenuScript.resetSelect += resetSelected;
    }

    void OnDisable()
    {
        MenuScript.optionSelected -= selectionCheck;
        MenuScript.resetSelect -= resetSelected;
    }

	// Update is called once per frame
	void Update () {

        //dirst, see if applicable, else it's greyed out
        if (!greyedOut)
        {
            //check if selected
            if (selected)
            {
                //selected code
                guiText.color = highlightedColor;
            }
            else
            {
                //unselected code
                guiText.color = defaultColor;
            }
        }
        else
        {
            guiText.color = Color.grey;
        }
    }
    
    void OnGUI()
    {
        if (selected && selectDelegateGUI != null)
        {
            selectDelegateGUI(true);
        }
    }

    void selectionCheck()
    {
        //if we are selected
        if (selected)
        {
            //send a message to a script in the object to get moving
            this.SendMessage("goTime", SendMessageOptions.DontRequireReceiver);
        }
    }

    //resetselected is just a speical case of setselected
    void resetSelected()
    {
        setSelected(false);
    }

    public bool getSelected()
    {
        return selected;
    }

    public void setSelected(bool val)
    {
        selected = val;
    }

    public void setSliderEntry(bool entry)
    {
        sliderEntry = entry;
    }
}
