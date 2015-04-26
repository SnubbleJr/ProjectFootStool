using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenText : MonoBehaviour {

    public string versionNumber;

    private Text guiText;

	// Use this for initialization
	void Start ()
    {
        guiText = GetComponent<Text>();

        guiText.text += "<size=15><color=yellow> v " + versionNumber + "</color></size>";    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
