using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMenuEntryManager : MonoBehaviour {

    //attached to the level slection menu, spawns menu entries up to a limit

    public GameObject levelMenuEntry;

    private LevelManagerBehaviour levelManager;

    private const int maxEntryCount = 7;

    private int entriesAbove = 0;
    private int entriesBelow = 0;

    private int previousOption = -1;

    private List<GameObject> levels = new List<GameObject>();
    private List<GameObject> displayedLevels = new List<GameObject>();
    private MenuScript menuScript;

	// Use this for initialization
	void Awake () 
    {
        levelManager = GetComponentInParent<LevelManagerBehaviour>();
        menuScript = GetComponent<MenuScript>();
	}
	
    public void listLevels(List<GameObject> listedLevels)
    {
        levels = new List<GameObject>(listedLevels);

        foreach (GameObject level in levels)
            if (displayedLevels.Count < maxEntryCount)
                createEntry(level);

        entriesBelow = levels.Count - (displayedLevels.Count + entriesAbove);

        menuScript.enabled = true;
        menuScript.searchForMenuEntries();

        padEntryText();
    }

    public void clearLevels()
    {
        foreach (GameObject level in displayedLevels)
            Destroy(level);

        levels.Clear();
        displayedLevels.Clear();
    }
    
    private void createEntry(GameObject level)
    {
        createEntry(level, displayedLevels.Count);
    }

    private void createEntry(GameObject level, int insertionIndex)
    {
        LevelBehaviour levelBehaviour = level.GetComponent<LevelBehaviour>();

        GameObject entry = Instantiate(levelMenuEntry);
        entry.transform.SetParent(transform, false);
        entry.GetComponent<RectTransform>().SetSiblingIndex(insertionIndex);

        LevelBehaviour entryBehaviour = entry.AddComponent<LevelBehaviour>();

        entry.GetComponent<UnityEngine.UI.Text>().text = level.name;

        entryBehaviour.author = levelBehaviour.author;
        entryBehaviour.thumbNail = levelBehaviour.thumbNail;

        displayedLevels.Insert(insertionIndex, entry);
    }

	// Update is called once per frame
	void Update ()
    {
        if (previousOption != menuScript.getCurrentEntry())
        {
            previousOption = menuScript.getCurrentEntry();
            menuBoundCheck();
        }
	}

    private void menuBoundCheck()
    {
        //if at the bottom of the list
        if (previousOption >= (maxEntryCount - 1))
            //if another entry we can display
            if(entriesBelow > 0)
            {
                entriesBelow--;
                entriesAbove++;

                //destroy top entry
                DestroyImmediate(displayedLevels[0]);
                displayedLevels.RemoveAt(0);
                
                //add next entry at end
                createEntry(levels[displayedLevels.Count + entriesAbove]);

                //rescan for entries
                previousOption--;
                menuScript.setIndex(previousOption);
                menuScript.searchForMenuEntries();
            }
        //else nothing

        //if we are close to the edge of the menu
        if (previousOption <= 0)
            //if another entry we can display
            if (entriesAbove > 0)
            {
                entriesBelow++;
                entriesAbove--;

                //destroy bottom entry
                DestroyImmediate(displayedLevels[displayedLevels.Count - 1]);
                displayedLevels.RemoveAt(displayedLevels.Count - 1);

                //add next entry at begginging
                createEntry(levels[entriesAbove], 0);

                //rescan for entries
                previousOption++;
                menuScript.setIndex(previousOption);
                menuScript.searchForMenuEntries();
            }

        padEntryText();
    }

    private void padEntryText()
    {
        for (int i = 0; i < displayedLevels.Count; i++)
        {
            string name = displayedLevels[i].GetComponent<UnityEngine.UI.Text>().text;
            name = name.Trim();

            for (int j = displayedLevels.Count - 1; j > i; j--)
                name = " " + name;

            displayedLevels[i].GetComponent<UnityEngine.UI.Text>().text = name;
        }
    }

    public void levelSelected()
    {
        levelManager.levelSelected(entriesAbove + menuScript.getCurrentEntry());
    }
}
