using UnityEngine;
using System.Collections;

public enum GameMode
{
    Stock,
    Koth,
    Race,
};

public interface IGameMode
{  
    Transform playerHit(PlayerControl playerControl);

    int checkForWinner();

    void setPlayers(GameObject[] p);

    void setPlayerControls(PlayerControl[] pc);

    void setColors(PlayerColor[] col);

    void setScore(int score);
    
    void restartRound();
}
