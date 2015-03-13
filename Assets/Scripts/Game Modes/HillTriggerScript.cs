using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HillTriggerScript : MonoBehaviour {
    
    //tracks who's currently in the hill, will return who is currently in it on demand

    List<GameObject> playersInHill;

    void Start()
    {
        playersInHill = new List<GameObject>();
    }

    //add if not already in
    void OnTriggerStay2D(Collider2D other)
    {
        if (!playersInHill.Contains(other.gameObject))
            playersInHill.Add(other.gameObject);
    }

    void FixedUpdate()
    {
        //clear it out every frame
        playersInHill.Clear();
    }
    
    public GameObject[] getPlayersInHill()
    {
        return playersInHill.ToArray();
    }
}
