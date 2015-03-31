using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGeneratorBehaviour : MonoBehaviour {

    //generates platforms at intervals

    public GameObject platform;

    private List<GameObject> platforms = new List<GameObject>();
    
	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void spawnPlatforms(int startAtLevel, int levelAmount)
    {
        //spawns platforms for levelAmount amount of levels
        for (int i = startAtLevel; i < (startAtLevel + levelAmount + 1); i += 3)
            spawnPlatformsOnLevel(i);
    }

    private void spawnPlatformsOnLevel(float height)
    {
        //spawns several platforms for a given level
        int rand = Random.Range(2, 5);

        for (int i = 0; i < rand; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-8f, 8f), height);
            GameObject plat = Instantiate(platform, pos, transform.rotation) as GameObject;
            plat.transform.parent = transform;
            platforms.Add(plat);
        }
    }

    public void resetScript(int platformChunkAmount)
    {
        foreach (GameObject plat in platforms)
            Destroy(plat);

        platforms.Clear();

        spawnPlatforms(0, platformChunkAmount);
    }
}
