using UnityEngine;
using System.Collections;

public class CameraToPlane : MonoBehaviour {

    //simple script taht when gets a request to ripple it's plane, it passes it down

    rippleSharp ripplePlaneScript;

	// Use this for initialization
	void Start () {
        ripplePlaneScript = transform.GetComponentInChildren<rippleSharp>();
	}
	
    public void splashAtPoint(Vector2 coOrds)
    {
        ripplePlaneScript.screenSplash(coOrds);
    }
}
