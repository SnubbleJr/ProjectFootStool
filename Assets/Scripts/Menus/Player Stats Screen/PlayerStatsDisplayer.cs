using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStatsDisplayer : MonoBehaviour {

    public GameObject statBoardHeaderLong, statBoardHeaderShort;
    public GameObject statBoardPanelLong, statBoardPanelShortHi, statBoardPanelShortLo;

    private PlayerStatsManagerBehaviour statsManager;

    private GameObject headerBoard;
    private PlayerStatsPanel[] statBoards;

    private PlayerStats[] playerStats;
    private PlayerControl[] players;

    void OnEnable()
    {
        PanelSlider.arrivedAtDestination += arrivedAtPos;
        PanelSlider.exited += exited;
    }

    void OnDisable()
    {
        PanelSlider.arrivedAtDestination -= arrivedAtPos;
        PanelSlider.exited -= exited;
    }

    public void buildStatBoard(PlayerStats[] stats, PlayerControl[] playerControls, PlayerStatsManagerBehaviour manager)
    {
        statsManager = manager;

        GameObject statBoard;
        
        bool shortBoard;

        if (playerControls.Length > 10)
            shortBoard = true;
        else
            shortBoard = false;

        //build the header
        if (shortBoard)
            headerBoard = Instantiate(statBoardHeaderShort, statBoardHeaderShort.transform.position, Quaternion.identity) as GameObject;
        else
            headerBoard = Instantiate(statBoardHeaderLong, statBoardHeaderShort.transform.position, Quaternion.identity) as GameObject;

        headerBoard.transform.SetParent(transform, false);

        //fancy maths to stack up the board's destination in such a way that they com in hot
        PanelSlider headerSlider = headerBoard.GetComponent<PanelSlider>();
        headerSlider.setDestination(headerBoard.transform.localPosition);

        //fancy maths to stack up the boards in such a way that they com in hot
        headerBoard.transform.localPosition += Vector3.left * 75 * 10;

        playerStats = stats;
        players = playerControls;

        List<PlayerStatsPanel> boards = new List<PlayerStatsPanel>();

        int winner = manager.getWinner();

        //builds ui for each player
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (shortBoard)
                if (i < 10)
                    statBoard = statBoardPanelShortHi;
                else
                    statBoard = statBoardPanelShortLo;
            else
                statBoard = statBoardPanelLong;

            GameObject board = Instantiate(statBoard, statBoard.transform.position, statBoard.transform.rotation) as GameObject;
            board.transform.SetParent(transform, false);

            //fancy maths to stack up the board's destination in such a way that they com in hot
            PanelSlider panelSlider = board.GetComponent<PanelSlider>();
            panelSlider.setDestination(board.transform.localPosition + Vector3.right * 58 * (i % 10));

            //fancy maths to stack up the boards in such a way that they com in hot
            board.transform.localPosition += Vector3.right * 75 * (i+1 % 10) * 20;

            PlayerStatsPanel boardStats = board.GetComponent<PlayerStatsPanel>();
            boards.Add(boardStats);

            int bulliedInt = findPlayerStats(playerStats, playerStats[i].getBullied());
            int bullyInt = findPlayerStats(playerStats, playerStats[i].getBully());

            if (bulliedInt < 0)
                bulliedInt = i;

            if (bullyInt < 0)
                bullyInt = i;

            bool won = winner == (i + 1);

            boardStats.setStats(players[i], playerStats[i], won, players[bulliedInt], players[bullyInt]);
        }

        statBoards = boards.ToArray();

        animatePanels();
    }

    private int findPlayerStats(PlayerStats[] playerStats, int id)
    {
        for (int i = 0; i < playerStats.Length; i++)
            if (playerStats[i].id == id)
                return i;
        return -1;
    }

    private void animatePanels()
    {
        PanelSlider headerSlider = headerBoard.GetComponent<PanelSlider>();
        headerSlider.moveToDestination();

        for (int i = 0; i < statBoards.Length; i++)
        {
            statBoards[i].moveToDestination();
        }
    }

    public void sendOffBoard()
    {
        PanelSlider headerSlider = headerBoard.GetComponent<PanelSlider>();
        headerSlider.depart();
        headerSlider.playLeavingSound();

        foreach (PlayerStatsPanel board in statBoards)
        {
            board.clearStats();
            board.sendOff();
        }
    }

    private void arrivedAtPos(PanelSlider panel)
    {
        panel.playArrivedSound();
    }
    
    private void exited(PanelSlider panel)
    {
        //if the header has moved from the screen
        if (panel == headerBoard.GetComponent<PanelSlider>())
        {
            //remove our stats
            removeStatBoard();

            //tell the manager it's time to mmove on
            statsManager.removeStats();
        }
    }

    public void updateStatBoardState(PlayerState[] playerStates)
    {
        //else, change the state for each player
        for (int i = 0; i < statBoards.Length; i++)
            statBoards[i].setReady(playerStates[i].ready);
    }

    private void removeStatBoard()
    {
        Destroy(headerBoard);
        foreach (PlayerStatsPanel board in statBoards)
            Destroy(board.gameObject);
    }
}
