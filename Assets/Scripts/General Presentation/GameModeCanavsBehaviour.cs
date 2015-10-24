using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameModeCanavsBehaviour : MonoBehaviour {
    //script that spawns ui elemetns for player scores, positons them correctly
    //also updates their text when told by a gamemode script

    public GameObject uiElement;

    private PlayerManagerBehaviour playerManager;
    private Camera canvasCamera;
    private Dictionary<int, PlayerScoreBehaviour> playerScores = new Dictionary<int, PlayerScoreBehaviour>();
    private GameObject[] players;

    //Here is a private reference only this class can access
    private static GameModeCanavsBehaviour instance;

    //This is the public reference that other classes will use
    public static GameModeCanavsBehaviour Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<GameModeCanavsBehaviour>();
            return instance;
        }
    }

    void Awake()
    {
        playerManager = GetComponentInParent<PlayerManagerBehaviour>();
        canvasCamera = GetComponentInParent<Canvas>().worldCamera;
    }

    void Update()
    {        
        foreach (KeyValuePair<int, PlayerScoreBehaviour> playerScore in playerScores)
        {
            bool someOneBehind = false;

            RectTransform rectTransform = playerScore.Value.GetComponent<RectTransform>();

            Vector3[] canvasWorldCorners = new Vector3[4];
            Vector3[] mainCameraWorldCorners = new Vector3[4];

            rectTransform.GetWorldCorners(canvasWorldCorners);

            for (int i = 0; i < canvasWorldCorners.Length; i++)
                mainCameraWorldCorners[i] = Camera.main.ViewportToWorldPoint(canvasCamera.WorldToViewportPoint(canvasWorldCorners[i]));

            Rect screenSpaceRect = new Rect(mainCameraWorldCorners[3], mainCameraWorldCorners[1] - mainCameraWorldCorners[3]);

            //see if any of the players are behind the group
            try
            {
                foreach (GameObject player in players)
                    if (screenSpaceRect.Contains(player.transform.position, true))
                        someOneBehind = true;
            }
            catch
            {
                players = playerManager.getPlayers();
            }
            
            if (someOneBehind)
                playerScore.Value.setTransparent();
            else
                playerScore.Value.unsetTransParent();
        }
    }

    private void createUIElement(int index)
    {
        PlayerScoreBehaviour playerScore = Instantiate(uiElement).GetComponent<PlayerScoreBehaviour>();
        playerScore.transform.SetParent(transform, false);
        if (!playerScores.ContainsKey(index))
            playerScores.Add(index, playerScore);
    }

    private void setScore(int index, int score)
    {
        playerScores[index].setScoreText(score.ToString());
    }

    private void setColor (int index, Color color)
    {
        playerScores[index].setColor(color);
    }
    
    private void setTeam(int index, string teamString)
    {
        playerScores[index].setTeamText(teamString);
    }

    private void deleteUIElement(int index)
    {
        if (playerScores.ContainsKey(index))
        {
            Destroy(playerScores[index].gameObject);
            playerScores.Remove(index);
        }    
    }

    public void setPlayer(int playerNo, Color color, string teamPrefix, int teamNo)
    {
        //update players
        players = playerManager.getPlayers();

        createUIElement(playerNo);
        setColor(playerNo, color);
        setTeam(playerNo, teamPrefix + " " + teamNo.ToString());
    }

    public void setPlayerScore(int index, int score)
    {
        if (!playerScores.ContainsKey(index))
            createUIElement(index);
        setScore(index, score);
    }

    public void unsetPlayer(int playerNo)
    {
        deleteUIElement(playerNo);
    }

    public void setPlayerActive(int index)
    {
        playerScores[index].setActive();
    }

    public void setPlayerInactive(int index)
    {
        playerScores[index].setInactive();
    }

    public void setPlayerTransparant(int index)
    {
        playerScores[index].setTransparent();
    }

    public void setInHill(int index)
    {
        if (!playerScores[index].particleSystem.isPlaying)
            playerScores[index].particleSystem.Play();
    }

    public void unsetInHill(int index)
    {
        playerScores[index].particleSystem.Stop();
    }
}
