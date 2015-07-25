using UnityEngine;
using System.Collections;

public class HillPulseScript : MonoBehaviour {

    //lerps the alpha back and forth

    public float alphaThreshold = 0.5f;
        
    private float newAlpha;
    private Color color;

    private Renderer renderer;

	// Use this for initialization
	void Start ()
    {
        renderer = GetComponent<Renderer>();
        color = renderer.material.color;
	}
	
	// Update is called once per frame
	void Update ()
    {
        newAlpha = Mathf.PingPong(Time.time, alphaThreshold);
        color.a = newAlpha;
        renderer.material.color = color;
	}
}
