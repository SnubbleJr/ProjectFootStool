using UnityEngine;
using System.Collections;

public class CameraColorChanger : MonoBehaviour {

    public Color changeColor;

    private Color color;
    private bool go = false;

    void Start()
    {
        color = camera.backgroundColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (go)
            camera.backgroundColor = Color.Lerp(camera.backgroundColor, changeColor, Time.deltaTime);
    }

    public void startFade()
    {
        go = true;
    }

    public void setFade(bool value)
    {
        go = value;

        if (!go)
            camera.backgroundColor = color;
    }
}
