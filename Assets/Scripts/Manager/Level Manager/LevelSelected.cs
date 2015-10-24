using UnityEngine;
using System.Collections;

public class LevelSelected : MonoBehaviour {

    private LevelMenuEntryManager entryManager;

    void Awake()
    {
        entryManager = GetComponentInParent<LevelMenuEntryManager>();
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        if (entryManager == null)
            entryManager = GetComponentInParent<LevelMenuEntryManager>();

        entryManager.levelSelected();
    }
}
