using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HillTriggerScript : MonoBehaviour {

    public SFX hillEnter = SFX.HillEnter;
    public SFX hillExit = SFX.HillExit;
    public SFX hillContested = SFX.HillContested;

    //tracks who's currently in the hill, will return who is currently in it on demand

    List<GameObject> playersInHill;

    void OnTriggerEnter2D(Collider2D collider)
    {
        //play sound, depending if if we are contested
        if ((getTeamsInHill().Length > 0) || (getPlayersInHill().Length > 0))
            SFXManagerBehaviour.Instance.playSound(hillContested);
        else
            SFXManagerBehaviour.Instance.playSound(hillEnter);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        SFXManagerBehaviour.Instance.playSound(hillExit);

        playersInHill.Remove(collider.gameObject);
    }

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
