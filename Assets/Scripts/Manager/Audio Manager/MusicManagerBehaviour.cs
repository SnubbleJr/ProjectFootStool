using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class MusicManagerBehaviour : MonoBehaviour {

    public AudioSource menuMusic, playerSelectionMusic, stockMusic, kothMusic, raceMusic, customMusic;

    private AudioMixer masterMixer;
    private MusicTrack currentTrack;
    private AudioSource currentSource;
    
    private float lowPassCutoffMax = 22000;
    private float lowPassCutoffMin = 400;
    private float currentLowPass;

    private float reverbMax = 0;
    private float reverbMin = -10000;
    private float currentReverb ;
        
    private bool paused = false;
    private bool gameOver = false;

    private const float invokeInterval = 0.02f;

    //Here is a private reference only this class can access
    private static MusicManagerBehaviour instance;

    //This is the public reference that other classes will use
    public static MusicManagerBehaviour Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<MusicManagerBehaviour>();
            return instance;
        }
    }

    public delegate void MusicManagerDelegate();
    public static event MusicManagerDelegate songStarted;
    public static event MusicManagerDelegate songStopped;

    private AudioSource layerToAlter;

    private float fadeTime = 0;        //used for the lerp fading
    private bool fadeLayerIn;           //used to singy if we're removing or adding the layer
    private bool beatHit = false;       //used so we start fading on the beat
    
	// Use this for initialization
	void Awake ()
    {
        currentLowPass = lowPassCutoffMax;
        currentReverb = reverbMax;

        BeatDetector.beatDetected += beatDetected;
        MusicVolumeSliderElement.volumeChanged += volumeChanged;
        masterMixer = MasterAudioManagerBehaviour.Instance.getMasterMixer();
	}

    void Update()
    {
        lowPasser();
        reverber();
        alterLayer();
    }

    private void beatDetected(bool onBeat)
    {
        if (onBeat)
            beatHit = true;
    }

    private void lowPasser()
    {
        float speed = 1f;

        //if paused
        if (paused)
            if ((currentLowPass * 0.9f) > lowPassCutoffMin)
                currentLowPass = Mathf.Lerp(currentLowPass, lowPassCutoffMin, speed * Time.unscaledDeltaTime);
            else
                currentLowPass = lowPassCutoffMin;
        else
            if (currentLowPass < (lowPassCutoffMax * 0.9f))
                currentLowPass = Mathf.Lerp(currentLowPass, lowPassCutoffMax, speed * Time.unscaledDeltaTime);
            else
                currentLowPass = lowPassCutoffMax;

        masterMixer.SetFloat("musicLowPassCuttoff", currentLowPass);
    }

    private void reverber()
    {
        float speed = 3f;

        if (gameOver)
            if ((currentReverb * 0.9f) > reverbMin)
                currentReverb = Mathf.Lerp(currentReverb, reverbMin, speed * Time.unscaledDeltaTime);
            else
                currentReverb = reverbMin;
        else
            if (currentReverb < (lowPassCutoffMax * 0.9f))
                currentReverb = Mathf.Lerp(currentReverb, reverbMax, speed * Time.unscaledDeltaTime);
            else
                currentReverb = reverbMax;

        masterMixer.SetFloat("dryLevel", currentReverb);
        masterMixer.SetFloat("roomHF", (reverbMin + reverbMax) - currentReverb);
        masterMixer.SetFloat("room", (reverbMin + reverbMax) - currentReverb);
    }

    private void addLayer(AudioSource layer)
    {
        layerToAlter = layer;
        fadeLayerIn = true;
    }

    private void removeLayer(AudioSource layer)
    {
        layerToAlter = layer;
        fadeLayerIn = false;
    }

    private void alterLayer()
    {
        if (layerToAlter != null)
        {
            if (fadeLayerIn)
            {
                fadeTime += Time.deltaTime;
                //lerp the volume up
                layerToAlter.volume = Mathf.Lerp(0, MusicVolumeSliderElement.volume, fadeTime * 1.5f);

                if (layerToAlter.volume == MusicVolumeSliderElement.volume)
                {
                    layerToAlter = null;
                    beatHit = false;
                    fadeTime = 0;
                }
            }
            else
            {
                fadeTime += Time.deltaTime;
                //lerp the volume down
                layerToAlter.volume = Mathf.Lerp(MusicVolumeSliderElement.volume, 0, fadeTime * 1.5f);

                if (layerToAlter.volume == 0)
                {
                    layerToAlter = null;
                    beatHit = false;
                    fadeTime = 0;
                }
            }
        }
        else
            beatHit = false;        //set beat hit to false, so we wait for the next beat to check
    }
    
    public void playMusic(MusicTrack musicTrack)
    {
        switch (musicTrack)
        {
            case MusicTrack.MainMenu:
                if (menuMusic != null)
                {
                    //if we're already playing the menu music (i.e. we're coming from playerselection)
                    if (menuMusic.isPlaying)
                        //fade out hype layer
                        removeLayer(playerSelectionMusic);
                    else
                    {
                        playMusic(menuMusic);
                        //load up the 2nd layer in the background
                        loadLayer(playerSelectionMusic, playerSelectionMusic);
                    }
                }
                break;
            case MusicTrack.PlayerSelectionMenu:
                if (menuMusic != null)
                {
                    //if we're not coming from the menu
                    if (!menuMusic.isPlaying)
                    {
                        playMusic(menuMusic);
                        //load up the 2nd layer in the background
                        loadLayer(playerSelectionMusic, playerSelectionMusic);
                    }
                    //load up the 2nd layer for fading in
                    addLayer(playerSelectionMusic);
                }
                break;
            case MusicTrack.StockGame:
                if (stockMusic != null)
                    playMusic(stockMusic);
                break;
            case MusicTrack.KothGame:
                if (kothMusic != null)
                    playMusic(kothMusic);
                break;
            case MusicTrack.RaceGame:
                if (raceMusic != null)
                    playMusic(raceMusic);
                break;
            case MusicTrack.Custom:
                if (customMusic != null)
                playMusic(customMusic);
                break;
            default:
                break;
        }

        currentTrack = musicTrack;
    }

    public void playMusic(MusicTrack musicTrack, AudioClip customTrack)
    {
        if (musicTrack == MusicTrack.Custom)
            customMusic.clip = customTrack;

        playMusic(musicTrack);
    }

    private void playMusic(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            stopMusic();
            audioSource.Play();
        }

        currentSource = audioSource;

        if (songStarted != null)
            songStarted();
    }

    private void loadLayer(AudioSource layer, AudioSource audioSource)
    {
        //load up the layer in the background
        layer.Stop();
        layer.volume = 0;
        layer.Play();
    }

    private void volumeChanged()
    {
        masterMixer.SetFloat("musicVol", (MusicVolumeSliderElement.volume - 1) * 80);
    }

    public void stopMusic()
    {
        foreach (Transform child in transform)
            child.GetComponent<AudioSource>().Stop();

        currentSource = null;

        if (songStopped != null)
            songStopped();
    }

    public MusicTrack getCurrentTrack()
    {
        return currentTrack;
    }

    public AudioSource getPlayingMusic()
    {
        return currentSource;
    }

    public void setPaused(bool val)
    {
        paused = val;
    }

    public void setGameOver(bool val)
    {
        gameOver = val;

        //this is so we get an instant jump
        if (gameOver)
        {
            reverbMin = -9000;
            reverbMax = -1000;
        }
        else
        {
            reverbMin = -10000;
            reverbMax = 0;
        }

        currentReverb = reverbMax;
    }
}
