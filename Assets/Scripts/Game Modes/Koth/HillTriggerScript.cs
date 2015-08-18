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
        if ((getTeamsInHill().Count > 0) || (getPlayersInHill().Count > 0))
            SFXManagerBehaviour.Instance.playSound(hillContested);
        else
            SFXManagerBehaviour.Instance.playSound(hillEnter);

        if (!playersInHill.Contains(collider.gameObject))
            playersInHill.Add(collider.gameObject);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        SFXManagerBehaviour.Instance.playSound(hillExit);

        playersInHill.Remove(collider.gameObject);
    }
    
    void OnEnable()
    {
        playersInHill = new List<GameObject>();
    }

    void OnDisable()
    {
        playersInHill = new List<GameObject>();
    }

    public List<GameObject> getPlayersInHill()
    {
        return playersInHill;
    }

    public List<GameObject> getTeamsInHill()
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

        return teamsInHill;
    }

    public void removePlayerFromIn(PlayerControl player)
    {
        if (playersInHill.Contains(player.gameObject))
        {
            playersInHill.Remove(player.gameObject);
            SFXManagerBehaviour.Instance.playSound(hillExit);
        }
    }
}
