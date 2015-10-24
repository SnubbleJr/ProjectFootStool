using UnityEngine;
using System.Collections;

public class CameraToPlane : MonoBehaviour {

    //simple script taht when gets a request to ripple it's plane, it passes it down

    rippleSharp ripplePlaneScript;

    //Here is a private reference only this class can access
    private static CameraToPlane instance;

    //This is the public reference that other classes will use
    public static CameraToPlane Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<CameraToPlane>();
            return instance;
        }
    }

	// Use this for initialization
	void Start () {
        ripplePlaneScript = transform.GetComponentInChildren<rippleSharp>();
	}
	
    public void splashAtPoint(Vector2 coOrds)
    {
        ripplePlaneScript.screenSplash(coOrds);
    }
}
