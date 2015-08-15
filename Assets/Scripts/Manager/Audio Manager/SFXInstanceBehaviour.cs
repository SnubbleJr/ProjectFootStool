using UnityEngine;
using System.Collections;

public class SFXInstanceBehaviour : MonoBehaviour {

    private AudioSource source;

	// Use this for initialization
	void Awake ()
    {
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!source.isPlaying)
            Destroy(this.gameObject);
	}
}
