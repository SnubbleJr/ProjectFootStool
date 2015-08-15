using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PositionLerper))]

public class PulseInwardsScript : MonoBehaviour {

    //moves to set position, and will manage movemeent spript (which will make it move back to where it was, thus pulse)

    private PositionLerper movementScript;

    private Vector3 oldPos;

	// Use this for initialization
	void Awake ()
    {
        movementScript = GetComponent<PositionLerper    >();
        setMovement(false);
	}

    void Update()
    {
        if (transform.position == oldPos)
            setMovement(false);
    }

    public void setMovement(bool val)
    {
        movementScript.enabled = val;
    }

    public void move(Vector3 newPos, float magnitude)
    {
        setMovement(false);
        setMovement(true);
        oldPos = transform.position;
        transform.position += (newPos * magnitude);
    }

    public void pulse()
    {
        move(new Vector3(1, -1, 0), 50);
    }
}
