using UnityEngine;
using System.Collections;

public class PulseToBeat : MonoBehaviour {

    public float speed = 30f;
    public Vector3 direction = Vector3.up;
    public float magnitude = 3;
    public bool playOnBeat = true;
    private Vector3 pos, dest;

	// Use this for initialization
	void Awake ()
    {
        pos = transform.position;
        dest = pos + (direction * magnitude);
	}

    void OnEnable()
    {
        BeatDetector.beatDetected += beatDectected;
    }

    void OnDisable()
    {
        BeatDetector.beatDetected -= beatDectected;
    }
     
    private void beatDectected(bool onTheBeat)
    {
        if (onTheBeat == playOnBeat)
            transform.position = dest;
    }

	// Update is called once per frame
	void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, pos, speed * Time.deltaTime);
	}
}
