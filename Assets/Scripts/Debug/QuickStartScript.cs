using UnityEngine;
using System.Collections;

public class QuickStartScript : MonoBehaviour {

    //create some random players in playerSelectionMenu (getReadyPlayers)
    //set game mode (GetGameMode, getStockCount and getTeamMode)
    //and then start the game

    public GameObject menuManager;
    public GameObject playerSelectionManager;
    public GameObject menuScreen;
    public AudioClip trackIntro;
    public AudioClip trackToPlay;
    public int BPM;

    public GameMode gameMode;
    public int stockCount;

    private MainMenuScript mainMenuScript;
    private PlayerSelectionMenuScript playerSelectionMenuScript;

    private bool started = false;

    void Awake()
    {
        mainMenuScript = menuManager.GetComponent<MainMenuScript>();
        mainMenuScript.setGameMode(gameMode);
        playerSelectionMenuScript = menuManager.GetComponent<PlayerSelectionMenuScript>();
        playerSelectionMenuScript.enabled = true;
        playerSelectionMenuScript.setScript(true);
        menuScreen.SetActive(false);
    }

    void Update()
    {
        if (!started)
            if (playerSelectionMenuScript.getPlayers().Length > 0)
                startGame();
    }

    void startGame()
    {
        started = true;

        playerSelectionMenuScript.setPlayers();
        playerSelectionMenuScript.setStockCount(stockCount);
        mainMenuScript.playersChosen();

        if (trackToPlay)
            MusicManagerBehaviour.Instance.playMusic(MusicTrack.Custom, trackIntro, trackToPlay, BPM);
    }
}
