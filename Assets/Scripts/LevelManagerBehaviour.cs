using UnityEngine;
using System.Collections;

public class LevelManagerBehaviour : MonoBehaviour {

    //loads the correct level based on the game mode fe dintoo it

    private GameObject[] levels;

	// Use this for initialization
	void Start ()
    {
        levels = GameObject.FindGameObjectsWithTag("Level");

        //disable all
        foreach (GameObject level in levels)
        {
            level.SendMessage("setLevel", false);
        }
	}
	
    public void setLevel(GameMode gameMode)
    {
        foreach(GameObject level in levels)
        {
            if (System.Enum.GetName(typeof(GameMode), gameMode) == level.name)
                level.SendMessage("setLevel", true);
            else
                level.SendMessage("setLevel", false);
        }
    }

    public void resetLevels()
    {   
        foreach (GameObject level in levels)
            level.SendMessage("setLevel", false);
    }
}
