﻿using UnityEngine;
using System.Collections;

public class MusicComponent : MonoBehaviour {

    public MusicTrack musicTrack;
    
    private MusicManagerBehaviour musicManager;

	// Use this for initialization
	void Awake ()
    {
        musicManager = GameObject.Find("Music Manager").GetComponent<MusicManagerBehaviour>();
	}

    public void playMusic()
    {
        if (musicManager != null)
            musicManager.playMusic(musicTrack);
    }

    public void stopMusic()
    {
        if (musicManager != null)
            musicManager.stopMusic();
    }

    public void setMusic(MusicTrack track)
    {
        musicTrack = track;
    }

    public void setMusic(GameMode gameMode)
    {
        switch(gameMode)
        {
            case GameMode.Stock:
                musicTrack = MusicTrack.StockGame;
                break;
            case GameMode.Koth:
                musicTrack = MusicTrack.KothGame;
                break;
            case GameMode.Race:
                musicTrack = MusicTrack.RaceGame;
                break;
            default:
                break;
        }
    }
}
