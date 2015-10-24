using UnityEngine;
using System.Collections;

public class ShellLevelBehaviour : MonoBehaviour
{
    //ANY CODE PUT HERE WILL NOT EXICUTE!
    //BUT ALL PUBLIC VARS DEFINED HERE WILL PASS ON TO THE RELAVANT SCRIPT
    public string author;
    public Sprite thumbNail;
    public GameMode gameModeType;
    public MusicTrack levelTrack;
    public AudioClip customTrackIntro, customTrack;
    public int customTrackBPM;
    public bool customSpawnPoints = false;
    public float levelSize = 1;
    public bool customVisuliserColors = false;
    public Color visuliserColor1, visuliserColor2;
}
