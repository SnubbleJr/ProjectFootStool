using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HillTriggerScript : MonoBehaviour {
    
    //tracks who's currently in the hill, will return who is currently in it on demand

    List<GameObject> playersInHill;
    
    //add if not already in
    void OnTriggerStay2D(Collider2D other)
    {
        if (!playersInHill.Contains(other.gameObject))
            playersInHill.Add(other.gameObject);
    }

    void OnEnable()
    {
        playersInHill = new List<GameObject>();
    }

    void OnDisable()
    {
        playersInHill = new List<GameObject>();
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

    public GameObject[] getTeamsInHill()
    {
        List<GameObject> teamsInHill = new List<GameObject>();
        int lastTeam = 0;

        foreach (GameObject player in playersInHill)
        {
            int playerTeam = player.GetComponent<PlayerControl>().getTeamNo();

            if (playerTeam != lastTeam)
            {
                lastTeam = playerTeam;
                teamsInHill.Add(player);
            }
        }

        return teamsInHill.ToArray();
    }
}
