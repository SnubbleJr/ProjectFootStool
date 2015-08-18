using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpawnPointGenerator : MonoBehaviour {

    //generate the spawn point game objects

    public Sprite debugSprite;

    //Here is a private reference only this class can access
    private static SpawnPointGenerator instance;

    //This is the public reference that other classes will use
    public static SpawnPointGenerator Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<SpawnPointGenerator>();
            return instance;
        }
    }

	public void generateSpawnPoints(float spawnAeraSize, bool showSpawnPoints)
    {
        destroyPoints();

        for (int i = 2; i <= 20; i++)
            generatePoints(i, spawnAeraSize, showSpawnPoints);
	}

    public void debugGenerateSpawnPoints(float spawnAeraSize, bool showSpawnPoints)
    {
        debugDestroyPoints();

        for (int i = 2; i <= 20; i++)
            generatePoints(i, spawnAeraSize, showSpawnPoints);
    }
	
    private void generatePoints(int playerCount, float size, bool debug)
    {
        //generate container for spawn points
        GameObject spawn = new GameObject(playerCount + "P Spawn");
        spawn.transform.parent = transform;
        spawn.transform.position = Vector3.zero;
        spawn.tag = "SpawnPoints";

        //offsetting similar to player selection
        const float magicNo = 12;
        float width = magicNo / (playerCount * 2);
        int j = 1;

        Color debugColor = Color.white;
        if (debug)
            debugColor = new Color(Random.Range(0, playerCount) / 20f, Random.Range(0, playerCount) / 20f, Random.Range(0, playerCount) / 20f);

        //generate spawn points
        for (int i = 1; i <= playerCount; i++)
        {
            GameObject point = new GameObject("Spawn Point " + i);
            point.transform.parent = spawn.transform;
            //ok so here we are getting the nth quartiles between 0 and 12 (width * j)
            //then offsetting it so that we get it between -6 and 6 (the -6 part)
            //then we are scaling it up so that the highest and lowest bounds are always -6 and 6 (* -(6/(width-6)))
            //we make that part minus, so that we get spawns from left to right
            point.transform.position = new Vector3(((width * j) - (magicNo / 2)) * -((magicNo / 2) / (width - (magicNo / 2))) * size, -(magicNo / 3), 0);

            if (debug)
            {
                point.AddComponent<SpriteRenderer>().sprite = debugSprite;
                point.GetComponent<SpriteRenderer>().color = debugColor;
            }

            j = j + 2;
        }
    }

    public GameObject[] getSpawnPointHolders()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (Transform child in transform)
            list.Add(child.gameObject);
        return list.ToArray();
    }

    public void destroyPoints()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (Transform child in transform)
            list.Add(child.gameObject);

        transform.DetachChildren();

        foreach (GameObject child in list)
            Destroy(child);
    }

    public void debugDestroyPoints()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (Transform child in transform)
            list.Add(child.gameObject);

        transform.DetachChildren();

        foreach (GameObject child in list)
            DestroyImmediate(child);
    }
}
