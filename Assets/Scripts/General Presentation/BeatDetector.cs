using UnityEngine;
using System.Collections;

public class BeatDetector : MonoBehaviour {

    public delegate void BeatDetectorDelegate(bool offBeat);
    public static event BeatDetectorDelegate beatDetected;

    private int bpm = 105;
    private float beatTime;
    private double currentTime;
    private bool started = false;
    private bool onTheBeat = false;
    private bool paused = false;

	// Use this for initialization
	void Awake ()
    {
        MusicManagerBehaviour.songStarted += songStarted;
        MusicManagerBehaviour.songPaused += songPaused;
        MusicManagerBehaviour.songUnpaused += songUnpaused;
        MusicManagerBehaviour.songStopped += songStopped;

        beatTime = 60 / bpm;

	}
	
    void Update()
    {
        if (started && !paused)
        {
            //add the difference between update
            currentTime += Time.unscaledDeltaTime;

            if (currentTime >= beatTime)
            {
                currentTime -= beatTime;
                beatFound();
            }
        }
    }

    private void beatFound()
    {
        onTheBeat = !onTheBeat;
        if (beatDetected != null)
            beatDetected(onTheBeat);
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
                bpm = 124;
                break;
            case MusicTrack.KothGame:
                bpm = 124;
                break;
            case MusicTrack.RaceGame:
                bpm = 133;
                break;
            case MusicTrack.Custom:
                bpm = MusicManagerBehaviour.Instance.getCustomBPM();
                break;
        }

        onTheBeat = true;
        beatTime = 60f / bpm;
        currentTime = 0;
        beatFound();
    }

    private void songPaused()
    {
        paused = true;
    }

    private void songUnpaused()
    {
        paused = false;
    }

    private void songStopped()
    {
        started = false;
        currentTime = 0;
    }

    public int getBPM()
    {
        return bpm;
    }

    public float timeTillNextBeat()
    {
        return (float)(beatTime - currentTime);
    }
}
