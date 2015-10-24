using UnityEngine;
using System.Collections;

public class MusicComponent : MonoBehaviour {

    private MusicTrack musicTrack;
    private AudioClip customIntro;
    private AudioClip customTrack;

    private MusicManagerBehaviour musicManager;

    private int customTrackBPM;

    void Awake()
    {
        musicManager = MusicManagerBehaviour.Instance;
    }

    public void playMusic()
    {
        if (musicTrack == MusicTrack.Custom && customTrack != null)
            musicManager.playMusic(musicTrack, customIntro, customTrack, customTrackBPM);
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

    public void setMusic(MusicTrack track, AudioClip intro, AudioClip custom, int customBPM)
    {
        musicTrack = track;
        customTrackBPM = customBPM;
        if (custom != null)
            customTrack = custom;
        if (intro != null)
            customIntro = intro;
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
