using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HillTriggerScript : MonoBehaviour {

    public SFX hillEnter = SFX.HillEnter;
    public SFX hillExit = SFX.HillExit;
    public SFX hillContested = SFX.HillContested;

    //tracks who's currently in the hill, will return who is currently in it on demand

    private List<GameObject> playersInHill;

    private SpriteRenderer spriteRenderer;

    private Color targetHillColor;
    private Color defaultHillColor = Color.white;
    private Color contestedHillColor = Color.grey;

    private float lerpSpeed = 5f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //play sound, depending if if we are contested
        if ((getTeamsInHill().Count > 0) || (getPlayersInHill().Count > 0))
        {
            SFXManagerBehaviour.Instance.playSound(hillContested);

            targetHillColor = contestedHillColor;
        }
        else
        {
            SFXManagerBehaviour.Instance.playSound(hillEnter);

            targetHillColor = collider.GetComponent<PlayerControl>().getColor();
        }

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

    void Update()
    {
        if (getPlayersInHill().Count <= 0)
            targetHillColor = defaultHillColor;
        else if (getPlayersInHill().Count == 1)
            targetHillColor = playersInHill[0].GetComponent<PlayerControl>().getColor();

        spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetHillColor, Time.deltaTime * lerpSpeed);
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
