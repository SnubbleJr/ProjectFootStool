using UnityEngine;
using System.Collections;

public class BackGroundManager : MonoBehaviour {

    private MovingBackground BG;
    private MenuScript menuScript;
    private int previousOption = -1;

	// Use this for initialization
	void Awake ()
    {
        menuScript = GetComponent<MenuScript>();
        BG = GameObject.Find("BG Plane").GetComponent<MovingBackground>();
        BG.setOption(previousOption);
	}

    void OnEnable()
    {
        previousOption = -1;
    }

    void OnDisable()
    {
        previousOption = -1;
    }

	// Update is called once per frame
	void Update ()
    {
        //updated BG on option change
	    if (previousOption != menuScript.getCurrentEntry())
        {
            previousOption = menuScript.getCurrentEntry();
            BG.setOption(previousOption);
        }
	}
}
