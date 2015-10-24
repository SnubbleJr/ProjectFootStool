using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
            if (levelBehaviour.customSpawnPoints)
            {
                List<GameObject> children = findChildWithTag(transform, "SpawnPoints");
                
                SpawnPointGenerator.Instance.debugSetSpawnPoints(children, true);
            }
            else
                SpawnPointGenerator.Instance.debugGenerateSpawnPoints(prevSize, true);
        }
	}

    private List<GameObject> findChildWithTag(Transform trans, string tag)
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in trans)
            if (child.CompareTag(tag))
                children.Add(child.gameObject);
            else
                children = findChildWithTag(child, tag);

        return children;
    }
}
