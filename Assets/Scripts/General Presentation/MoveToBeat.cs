using UnityEngine;
using System.Collections;

public class MoveToBeat : MonoBehaviour {

    public Vector3 direction = Vector3.right;
    public float magnitude = 50;
    public bool playOnBeat = true;
    private Vector3 pos;
    private bool canMove = false;

    void Awake()
    {
        pos = transform.position;
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
        if (canMove)
            if (onTheBeat == playOnBeat)
                transform.position = transform.position + (direction * magnitude);
    }

    public void setMove(bool val)
    {
        canMove = val;
    }
}
