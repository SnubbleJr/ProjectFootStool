using UnityEngine;
using System.Collections;

public class MusicComponent : MonoBehaviour {

    private MusicTrack musicTrack;
    private AudioClip customTrack;

    private MusicManagerBehaviour musicManager;

    void Awake()
    {
        musicManager = MusicManagerBehaviour.Instance;
    }

    public void playMusic()
    {
        if (musicTrack == MusicTrack.Custom && customTrack != null)
            musicManager.playMusic(musicTrack, customTrack);
        else
            musicManager.playMusic(musicTrack);
    }

    public void playMusic(MusicTrack track)
    {
        setMusic(track);
        playMusic();
    }

    public void stopMusic()
    {
        try
        {
            musicManager.stopMusic();
        }
        catch
        {
        }
    }

    public void setMusic(MusicTrack track)
    {
        musicTrack = track;
    }

    public void setMusic(MusicTrack track, AudioClip custom)
    {
        musicTrack = track;
        if (custom != null)
            customTrack = custom;
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
