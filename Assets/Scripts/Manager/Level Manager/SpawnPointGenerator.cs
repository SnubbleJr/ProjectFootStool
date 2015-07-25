using UnityEngine;
using System.Collections;

public class SpawnPointGenerator : MonoBehaviour {

    //generate the spawn point game objects

	void OnEnable()
    {
        for (int i = 2; i <= 20; i++)
            generatePoints(i);
	}
	
    private void generatePoints(int playerCount)
    {
        //generate container for spawn points
        GameObject spawn = new GameObject(playerCount + "P Spawn");
        spawn.transform.parent = transform;
        spawn.transform.position = Vector3.zero;
        spawn.tag = "SpawnPoints";

        //offsetting similar to player selection
        float width = 12f / (playerCount * 2);
        int j = 1;

        //generate spawn points
        for (int i = 1; i <= playerCount; i++)
        {
            GameObject point = new GameObject("Spawn Point " + i);
            point.transform.parent = spawn.transform;
            //ok so here we are getting the nth quartiles between 0 and 12 (width * j)
            //then offsetting it so that we get it between -6 and 6 (the -6 part)
            //then we are scaling it up so that the highest and lowest bounds are always -6 and 6 (* -(6/(width-6)))
            //we make that part minus, so that we get spawns from left to right
            point.transform.position = new Vector3(((width*j)-6) * -(6/(width-6)), -3.5f, 0);

            j = j + 2;
        }
    }
}
