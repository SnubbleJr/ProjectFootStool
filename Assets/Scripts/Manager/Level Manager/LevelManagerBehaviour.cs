using UnityEngine;
using System.Collections;

public class LevelManagerBehaviour : MonoBehaviour {

    //loads the correct level based on the game mode fe dintoo it

    private GameObject[] levels;

    //Here is a private reference only this class can access
    private static LevelManagerBehaviour instance;

    //This is the public reference that other classes will use
    public static LevelManagerBehaviour Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<LevelManagerBehaviour>();
            return instance;
        }
    }

	// Use this for initialization
	void Awake ()
    {
        findLevels();
	}
	
    public void setLevel(GameMode gameMode)
    {
        foreach(GameObject level in levels)
            if (System.Enum.GetName(typeof(GameMode), gameMode) == level.name)
                level.GetComponent<LevelBehaviour>().setLevel(true);
            else
                level.GetComponent<LevelBehaviour>().setLevel(false);
    }

    public void resetLevels()
    {
        foreach (GameObject level in levels)
            level.GetComponent<LevelBehaviour>().setLevel(false);
    }

    public void findLevels()
    {
        levels = GameObject.FindGameObjectsWithTag("Level");

        //disable all
        foreach (GameObject level in levels)
            level.GetComponent<LevelBehaviour>().setLevel(false);
    }
}
