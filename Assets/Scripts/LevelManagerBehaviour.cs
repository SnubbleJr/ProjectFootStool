using UnityEngine;
using System.Collections;

public class LevelManagerBehaviour : MonoBehaviour {

    //loads the correct level based on the game mode fe dintoo it

    private GameObject[] levels;

	// Use this for initialization
	void Start ()
    {
        levels = GameObject.FindGameObjectsWithTag("Level");
	}
	
    public void setLevel(GameMode gameMode)
    {
        foreach(GameObject level in levels)
        {
            if (System.Enum.GetName(typeof(GameMode), gameMode) == level.name)
                level.SetActive(true);
            else
                level.SetActive(false);
        }
    }
}
