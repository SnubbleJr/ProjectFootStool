using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManagerBehaviour : MonoBehaviour {

    //loads the correct level based on the game mode fe dintoo it

    private GameObject menu;

    private GameObject[] levels;
    private GameObject selectedLevel;

    private List<GameObject> listedLevels = new List<GameObject>();

    private MainMenuScript mainMenu;

    private bool active = false;

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
        menu = transform.FindChild("Canvas").gameObject;

        mainMenu = GameObject.Find("Menu Manager").GetComponent<MainMenuScript>();

        findLevels();
	}
	
    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonDetected;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonDetected;
    }

    private void buttonDetected(PlayerInputScheme player, string input, float value)
    {
        if (active)
        {
            if (input == player.inputs[PlayerInput.CancelInput].shortName)
            {
                //return back to player menu
                mainMenu.levelChosen(-1);
                closeMenu();
            }
        }
    }

    public void openMenu(GameMode selectedGameMode)
    {
        active = true;
        menu.SetActive(active);

        //displays set for current game mode
        listedLevels.Clear();

        foreach (GameObject level in levels)
            if (level.GetComponent<LevelBehaviour>().gameModeType == selectedGameMode)
                listedLevels.Add(level);

        //list selection and wait on choice
        listSelection();
    }

    public void closeMenu()
    {
        active = false;
        GetComponentInChildren<LevelMenuEntryManager>().clearLevels();
        menu.SetActive(active);
    }

    private void listSelection()
    {
        if (listedLevels.Count == 1)
        {
            selectedLevel = listedLevels[0];
            mainMenu.levelChosen(1);
        }
        else
            //list all the levels
            GetComponentInChildren<LevelMenuEntryManager>().listLevels(listedLevels);
    }

    private void selectionMade()
    {
        //get currently selected level
        mainMenu.levelChosen(1);
    }

    public void startLevel()
    {
        resetLevels();
        selectedLevel.GetComponent<LevelBehaviour>().setLevel(true);
    }

    public void resetLevels()
    {
        foreach (GameObject level in levels)
            level.GetComponent<LevelBehaviour>().setLevel(false);
    }

    public void findLevels()
    {
        levels = GameObject.FindGameObjectsWithTag("Level");

        if (levels.Length != 0)
            //disable all
            foreach (GameObject level in levels)
                level.GetComponent<LevelBehaviour>().setLevel(false);
    }

    public void levelSelected(int index)
    {
        selectedLevel = listedLevels[index];
        selectionMade();
    }
}
