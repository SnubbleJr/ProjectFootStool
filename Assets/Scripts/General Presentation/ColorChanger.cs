using UnityEngine;
using System.Collections;

public class ColorChanger : MonoBehaviour {

    //when given the signal, just darkens everything, for when someone wins

    public Color changeColor;

    private Color color;
    private bool go = false;

    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();

        color = renderer.material.color;
    }

	// Update is called once per frame
	void Update ()
    {
        if (go)
            renderer.material.color = Color.Lerp(renderer.material.color, changeColor, Time.deltaTime);

        if ((renderer.material.color.r + color.r) < (1 - 0.06f))
            go = false;
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
