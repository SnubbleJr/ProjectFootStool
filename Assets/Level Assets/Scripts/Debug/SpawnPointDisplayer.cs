using UnityEngine;
using System.Collections;

[ExecuteInEditMode] 
public class SpawnPointDisplayer : MonoBehaviour
{
    //USE TO SEE HOW CHANGING LEVEL SIZE IN SHELL LEVEL BEHAVIOUR WILL EFFECT SPAWN POINTS
    //THIS CODE WILL NOT BE PRESENT WHEN LEVEL IS IMPORTED
    ShellLevelBehaviour levelBehaviour;

    private float prevSize; 

	// Use this for initialization
	void Awake ()
    {
	    levelBehaviour = GetComponent<ShellLevelBehaviour>();
        prevSize = levelBehaviour.levelSize;
	}
	
	// Update is called once per frame
    void OnRenderObject()
    {
        if (levelBehaviour.levelSize != prevSize)
        {
            prevSize = levelBehaviour.levelSize;
            SpawnPointGenerator.Instance.debugGenerateSpawnPoints(prevSize, true);
        }
	}
}
