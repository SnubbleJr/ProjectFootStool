using UnityEngine;
using System.Collections;

public class LevelMoverScript : MonoBehaviour {

    //slowly moves level up
    //increases in a sin curve

    private float speed = Mathf.PI;

    private Vector3 originalPos;
    private Vector3 dest;

	// Use this for initialization
	void Start ()
    {
        originalPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        speed += 0.001f;
        
        if (speed > (2 * Mathf.PI))
            speed = (2 * Mathf.PI);
        
        dest = transform.position;
        dest.y += 1;
        transform.position = Vector3.Lerp(transform.position, dest, (10 * (1 + Mathf.Cos(speed))) * Time.deltaTime);   
	}

    public void resetScript()
    {
        transform.position = originalPos;
        speed = Mathf.PI;
    }
}
