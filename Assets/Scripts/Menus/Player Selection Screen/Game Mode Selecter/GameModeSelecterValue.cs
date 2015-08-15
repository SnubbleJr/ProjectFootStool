using UnityEngine;
using System.Collections;

public class GameModeSelecterValue : MonoBehaviour {
    //mediator between the parent option and uitextsctipt
    private UITextScript uiTextScript;

	// Use this for initialization
	void Awake ()
    {
        uiTextScript = GetComponent<UITextScript>();
	}

    public void setValueText(string text)
    {
        uiTextScript.setText(text);
    }
}
