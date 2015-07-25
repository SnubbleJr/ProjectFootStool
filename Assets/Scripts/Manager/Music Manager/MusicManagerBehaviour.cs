using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]

public class MusicManagerBehaviour : MonoBehaviour {

    public AudioClip menuMusic, playerSelectionMusic, stockMusic, kothMusic, raceMusic;

    public delegate void MusicManagerDelegate();
    public static event MusicManagerDelegate songStarted;
    public static event MusicManagerDelegate songStopped;

    private AudioSource baseAudioLayer, secondaryAudioLayer;
    private AudioSource layerToAlter;

    private float fadeTime = 0;        //used for the lerp fading
    private bool fadeLayerIn;           //used to singy if we're removing or adding the layer
    private bool beatHit = false;       //used so we start fading on the beat

	// Use this for initialization
	void Start ()
    {
        baseAudioLayer = GetComponent<AudioSource>();
        secondaryAudioLayer = this.gameObject.AddComponent<AudioSource>();
        secondaryAudioLayer.loop = true;
        secondaryAudioLayer.playOnAwake = false;
        secondaryAudioLayer.volume = 0;
        BeatDetector.beatDetected += beatDetected;
        VolumeSliderElement.volumeChanged += volumeChanged;
	}
	
    void Update()
    {
        alterLayer();
    }

    private void beatDetected(bool onBeat)
    {
        if (onBeat)
            beatHit = true;
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
                layerToAlter.volume = Mathf.Lerp(0, VolumeSliderElement.volume, fadeTime * 1.5f);

                if (layerToAlter.volume == VolumeSliderElement.volume)
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
                layerToAlter.volume = Mathf.Lerp(VolumeSliderElement.volume, 0, fadeTime * 1.5f);

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
                //if we're already playing the menu music (i.e. we're coming from playerselection)
                if (baseAudioLayer.clip.Equals(menuMusic) && baseAudioLayer.isPlaying)
                    //fade out hype layer
                    removeLayer(secondaryAudioLayer);
                else
                {
                    playMusic(menuMusic);
                    //load up the 2nd layer in the background
                    loadLayer(secondaryAudioLayer, playerSelectionMusic);
                }
                break;
            case MusicTrack.PlayerSelectionMenu:
                //if we're not coming from the menu
                if (!baseAudioLayer.clip.Equals(menuMusic))
                {
                    playMusic(menuMusic);
                    //load up the 2nd layer in the background
                    loadLayer(secondaryAudioLayer, playerSelectionMusic);
                }
                //load up the 2nd layer for fading in
                addLayer(secondaryAudioLayer);
                break;
            case MusicTrack.StockGame:
                playMusic(stockMusic);
                break;
            case MusicTrack.KothGame:
                playMusic(kothMusic);
                break;
            case MusicTrack.RaceGame:
                playMusic(raceMusic);
                break;
            default:
                break;
        }

        baseAudioLayer.loop = true;
    }

    private void playMusic(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            stopMusic();
            baseAudioLayer.clip = audioClip;
            baseAudioLayer.Play();
        }

        if (songStarted != null)
            songStarted();
    }

    private void loadLayer(AudioSource layer, AudioClip clip)
    {
        //load up the layer in the background
        layer.Stop();
        layer.clip = clip;
        layer.volume = 0;
        layer.Play();
    }

    private void volumeChanged()
    {
        baseAudioLayer.volume = VolumeSliderElement.volume;
    }

    public void stopMusic()
    {
        if (baseAudioLayer == null)
            baseAudioLayer = GetComponent<AudioSource>();

        if (secondaryAudioLayer == null)
            secondaryAudioLayer = GetComponent<AudioSource>();	
        
        baseAudioLayer.Stop();
        secondaryAudioLayer.Stop();

        if (songStopped != null)
            songStopped();
    }
}
