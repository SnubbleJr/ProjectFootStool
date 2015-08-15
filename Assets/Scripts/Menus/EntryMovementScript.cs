using UnityEngine;
using System.Collections;

public class EntryMovementScript : MonoBehaviour {

    public float time = 30f;

    private Vector3 pos;
    
	// Use this for initialization
	void OnEnable ()
    {
        pos = transform.localPosition;
        transform.localPosition = new Vector3(-1000, pos.y, 0);
	}

    void OnDisable ()
    {
        transform.localPosition = pos;
    }
	
	// Update is called once per frame
	void Update () 
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, pos, time * Time.deltaTime);
	}
}
