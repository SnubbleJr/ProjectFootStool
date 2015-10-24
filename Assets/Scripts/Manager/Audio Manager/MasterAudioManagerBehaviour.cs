using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class MasterAudioManagerBehaviour : MonoBehaviour {

    public AudioMixer masterMixer;

    //Here is a private reference only this class can access
    private static MasterAudioManagerBehaviour instance;

    //This is the public reference that other classes will use
    public static MasterAudioManagerBehaviour Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<MasterAudioManagerBehaviour>();
            return instance;
        }
    }

	// Use this for initialization
	void Awake ()
    {
        MasterVolumeSliderElement.volumeChanged += volumeChanged;

        if (masterMixer == null)
            Debug.LogError("Master Mixer not assigned to Mixer Manager!");
	}

    public void volumeChanged(bool save)
    {
        masterMixer.SetFloat("masterVol", (MasterVolumeSliderElement.volume - 1) * 80);
    }

    public void mute()
    {
        masterMixer.SetFloat("masterVol", -80);
    }

    public void unmute()
    {
        volumeChanged(false);
    }

    public AudioMixer getMasterMixer()
    {
        return masterMixer;
    }
}
