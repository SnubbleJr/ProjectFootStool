using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGeneratorBehaviour : MonoBehaviour {

    //generates platforms at intervals

    public GameObject preferedPlatform;
    public GameObject[] specialPlatforms;

    private List<GameObject> platforms = new List<GameObject>();
    
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
            GameObject platformPrefab;
            //random platform
            int index = (int)Random.Range(0, specialPlatforms.Length * 2);
            //weight toward perfered pplat
            if (index >= specialPlatforms.Length)
                platformPrefab = preferedPlatform;
            else
                platformPrefab = specialPlatforms[index];

            Vector2 pos = new Vector2(Random.Range(-8f, 8f), height);
            CustomLevelParser.Instance.parseObject(platformPrefab);
            GameObject plat = CustomLevelParser.Instance.getParsedObject();
            plat.transform.position = pos;
            plat.transform.parent = transform;
            plat.SetActive(true);
            setChildren(plat.transform, true);

            plat.BroadcastMessage("startFade", SendMessageOptions.DontRequireReceiver);

            platforms.Add(plat);
        }
    }

    private void setChildren(Transform trans, bool value)
    {
        foreach (Transform child in trans)
        {
            child.gameObject.SetActive(value);
            setChildren(child, value);
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
