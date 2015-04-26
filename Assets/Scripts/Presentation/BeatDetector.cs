using UnityEngine;
using System.Collections;

public class BeatDetector : MonoBehaviour {

    public delegate void BeatDetectorDelegate(bool offBeat);
    public static event BeatDetectorDelegate beatDetected;

    private float bpm = 105;
    private float beatTime;
    private float currentTime;
    private bool started = false;
    private bool onTheBeat = false;
    private float prevStartupTime = 0;

	// Use this for initialization
	void Start ()
    {
        MusicManagerBehaviour.songStarted += songStarted;
        MusicManagerBehaviour.songStopped += songStopped;

        beatTime = 60 / bpm;

        prevStartupTime = Time.realtimeSinceStartup;
	}
	
    void Update()
    {
        //add the difference between update
        currentTime += (Time.realtimeSinceStartup - prevStartupTime);

        prevStartupTime = Time.realtimeSinceStartup;

        if (currentTime >= beatTime)
        {
            currentTime -= beatTime;
            beatFound();
        }
    }

    private void beatFound()
    {
        if (beatDetected != null)
        {
            onTheBeat = !onTheBeat;
            beatDetected(onTheBeat);
        }
    }

    private void songStarted()
    {
        started = true;
        currentTime = 0;
    }

    private void songStopped()
    {
        started = false;
    }
}
