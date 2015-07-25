using UnityEngine;
using System.Collections;

public class GameModeSelecterOption : MonoBehaviour {
    //sets the text for the values for this option

    //we have to make a distintion for the left most value, otherwise we get justification offsetting problems
    private GameModeSelecterValue priviousValue, currentValues;
    private UITextScript uiTextScript;

    // Use this for initialization
    void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name.Contains("1"))
                priviousValue = child.GetComponent<GameModeSelecterValue>();
            if (child.gameObject.name.Contains("2"))
                currentValues = child.GetComponent<GameModeSelecterValue>();
        }

        uiTextScript = GetComponent<UITextScript>();
    }

    public void setOptionText(string text)
    {
        try
        {
            uiTextScript.enterText(text);
        }
        catch
        {
        }
    }

    public void setValueText(string privousValueText)
    {
        try
        {
            //one string is for privous value
            priviousValue.setValueText(privousValueText);
        }
        catch
        {
        }
    }

    public void setValueText(string currentValueText, string nextValueText)
    {
        try
        {
            //two strings are for current value
            currentValues.setValueText(currentValueText + " " + nextValueText);
        }
        catch
        {
        }
    }
}
