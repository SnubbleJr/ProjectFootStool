using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {

    //when given the signal, just darkens everything, for when someone wins

    public Color changeColor;

    private Color color;
    private bool go = false;
	
    void Start()
    {
        color = renderer.material.color;
    }

	// Update is called once per frame
	void Update () 
    {
        if (go)
            renderer.material.color = Color.Lerp(renderer.material.color, changeColor, Time.deltaTime);
	}

    public void startFade()
    {
        go = true;
    }

    public void setFade(bool value)
    {
        go = value;

        if (!go)
            renderer.material.color = color;
    }
}
