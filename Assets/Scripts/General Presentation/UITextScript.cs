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
        text.text = str;
    }

    public void setColor(Color color)
    {
        text.color = color;
    }
}
