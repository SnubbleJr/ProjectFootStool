using UnityEngine;
using System.Collections;

public class CameraColorLerper : MonoBehaviour {

    public Color color1;
    public Color color2;
    public float speed;

    private bool go = true;
    private Camera camera;

    void Awake()
    {
        camera = GetComponent<Camera>();
    }

	// Update is called once per frame
    void Update()
    {
        if (go)
            camera.backgroundColor = Color.Lerp(color1, color2, Mathf.PingPong(Time.time * speed, 1));	
	}

    public void setFade(bool value)
    {
        //deactivates when we want the the BG to fade
        go = !value;
    }
}
