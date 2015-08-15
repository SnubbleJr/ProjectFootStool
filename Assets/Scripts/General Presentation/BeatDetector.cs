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
	void Awake ()
    {
        MusicManagerBehaviour.songStarted += songStarted;
        MusicManagerBehaviour.songStopped += songStopped;

        beatTime = 60 / bpm;

        prevStartupTime = Time.realtimeSinceStartup;
	}
	
    void Update()
    {
        if (started)
        {
            //add the difference between update
            currentTime += Time.unscaledDeltaTime;

            prevStartupTime = Time.realtimeSinceStartup;

            if (currentTime >= beatTime)
            {
                currentTime -= beatTime;
                beatFound();
            }
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
        switch(MusicManagerBehaviour.Instance.getCurrentTrack())
        {
            case MusicTrack.MainMenu:
                bpm = 105;
                break;
            case MusicTrack.PlayerSelectionMenu:
                bpm = 105;
                break;
            case MusicTrack.StockGame:
                bpm = 125;
                break;
            case MusicTrack.KothGame:
                bpm = 125;
                break;
            case MusicTrack.RaceGame:
                bpm = 133;
                break;
        }

        beatTime = 60f / bpm;
        currentTime = 0;
        prevStartupTime = Time.realtimeSinceStartup;
        beatFound();
    }

    private void songStopped()
    {
        started = false;
    }
}
