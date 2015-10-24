using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SFXPair
{
    public SFX name;
    public GameObject clip;

    public SFXPair(SFX sfx, GameObject go)
    {
        name = sfx;
        clip = go;
    }
}

//to add a new sfx, add it to the enum
//create a GO with a audio source
//create a new group in mixer
//increase SFXPairs to accomidate this
//hoook everything up

//for auto populating SFXPairs
/*
[ExecuteInEditMode]
public class SFXManagerBehaviour : MonoBehaviour {

    public AudioMixer masterMixer;
    public List<SFXDictionary> SFXPairs = new List<SFXDictionary>();

	// Use this for initialization
	void Start () {
        foreach (SFX sfx in Enum.GetValues(typeof(SFX)))
        {
            SFXDictionary dic = new SFXDictionary();
            dic.name = sfx;
            SFXPairs.Add(dic);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
 */

public class SFXManagerBehaviour : MonoBehaviour
{
    public List<SFXPair> SFXPairs = new List<SFXPair>();

    private AudioMixer masterMixer;

    private Dictionary<SFXPair, GameObject> loopingInstances = new Dictionary<SFXPair, GameObject>(); //using sfxpair as a way of storinng each instance of a loop

    //Here is a private reference only this class can access
    private static SFXManagerBehaviour instance;

    //This is the public reference that other classes will use
    public static SFXManagerBehaviour Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<SFXManagerBehaviour>();
            return instance;
        }
    }

    void Awake()
    {
        SFXVolumeSliderElement.volumeChanged += volumeChanged;
        masterMixer = MasterAudioManagerBehaviour.Instance.getMasterMixer();
    }

    public void volumeChanged(bool save)
    {
        masterMixer.SetFloat("sfxVol", (SFXVolumeSliderElement.volume - 1) * 80);
    }
         
    private void playSFX(SFXPair sfx)
    {
        playSFX(sfx, false);
    }

    private GameObject playSFX(SFXPair sfx, bool loop)
    {
        if (sfx.name == SFX.None)
            return null;        

        //spawn gameobject, returns it if a method wants to keep track of it
        GameObject sfxInstance = Instantiate(sfx.clip) as GameObject;
        sfxInstance.transform.SetParent(transform, false);
        sfxInstance.GetComponent<AudioSource>().loop = loop;
        return sfxInstance;
    }

    private SFXPair getSFXPair(SFX sfx)
    {
        foreach (SFXPair pair in SFXPairs)
            if (pair.name == sfx)
                return(pair);
        return null;
    }

    public void playSound(SFX sfx)
    {
        playSFX(getSFXPair(sfx));
    }

    public void loopSound(SFXPair pair)
    {
        bool contains = false;

        foreach (KeyValuePair<SFXPair, GameObject> loopingInstance in loopingInstances)
            if ((loopingInstance.Key.name.Equals(pair.name) && loopingInstance.Key.clip.Equals(pair.clip)))
                contains = true;

        //if we don't have an instance running
        if (!contains)
        {
            GameObject newLoopingInstance = playSFX(getSFXPair(pair.name), true);
            if (newLoopingInstance != null)
                //keep track of tthis loopin game object
                loopingInstances.Add(pair, newLoopingInstance);
        }
    }

    public void stopLoop(SFXPair pair)
    {
        foreach (KeyValuePair<SFXPair, GameObject> loopingInstance in loopingInstances)
            if ((loopingInstance.Key.name.Equals(pair.name) && loopingInstance.Key.clip.Equals(pair.clip)))
            {
                Destroy(loopingInstance.Value);
                loopingInstances.Remove(loopingInstance.Key);
                break;
            }
    }

    public void stopAllLoops()
    {
        foreach (KeyValuePair<SFXPair, GameObject> loopingInstance in loopingInstances)
            Destroy(loopingInstance.Value);

        loopingInstances.Clear();
    }
}
