using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]

public class UITextScript : MonoBehaviour {

    private Text text;

	// Use this for initialization
	void Awake ()
    {
        text = GetComponent<Text>();
	}
	
    public void setText(string str)
    {
        if (!text.text.Equals(str))
            text.text = str;
    }

    public void setColor(Color color)
    {
        if (color != text.color)
            text.color = color;
    }

    public Color getColor()
    {
        if (text == null)
            text = GetComponent<Text>();

        return text.color;
    }
}
