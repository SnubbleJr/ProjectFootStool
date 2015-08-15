using UnityEngine;
using System.Collections;

public class EndGamePanelManager : MonoBehaviour {

    public EndGamePanelDisplayer panelDisplayer;

    void Awake()
    {
        if (panelDisplayer == null)
            Debug.LogError("Panel Displayer Panel not connected to Player Stats Manager!");
    }

    public void lastPanelDisplayed()
    {
        GetComponent<PlayerManagerBehaviour>().winnerDisplayed();
    }

    public void removePanels()
    {
        panelDisplayer.gameObject.SetActive(false);
        GetComponent<PlayerManagerBehaviour>().endGame();
    }

    public void displayPanels()
    {
        panelDisplayer.gameObject.SetActive(true);
        panelDisplayer.buildPanelBoard(this);
    }
}
