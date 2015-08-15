using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {

    //forces music manager to start playing music
    public MusicTrack track;

    void OnEnable()
    {
        AssetBundleLoaderBehaviour.musicLoaded += playMusic;
    }

    void OnDisable()
    {
        AssetBundleLoaderBehaviour.musicLoaded -= playMusic;
    }

	// Use this for initialization
	void playMusic () 
    {
        MusicManagerBehaviour.Instance.playMusic(track);   
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
