using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class stuffSpawner : MonoBehaviour {

    public GameObject thing;
    public int count;

    int oldCount;

    //test bit here
    List<GameObject> playerSelecters;

    void Start()
    {
    }

    private void buildPlayerSelecters()
    {
        playerSelecters = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject aThing = Instantiate(thing, transform.position, Quaternion.identity) as GameObject;
            aThing.transform.parent = transform;
            playerSelecters.Add(aThing);
        }
    }

    private void updateSelectors()
    {
        float xOffset;
        float baseXOffset = 0.05f;
        float maxXOffset = 0.15f;
        float deltaXOffset = 0;

        float yOffset;
        float baseYOffset = 0.95f;
        float maxYOffset = 0.05f;
        float deltaYOffset = 0;

        List<List<GameObject>> rows = generateRows();

        if (rows.Count > 1)
        {
            //calculate the how much each row needs to be xOffset and yOffset by
            deltaXOffset = (maxXOffset - baseXOffset) / (rows.Count - 1);
            deltaYOffset = (baseYOffset - maxYOffset) / (rows.Count - 1);
        }

        for (int i = 0; i < rows.Count; i++)
        {
            //this is basically a foreach loop on each colum, but we want to know it's index

            List<GameObject> row = rows[i];

            //NOTE yOffset is from top down

            //for yOffset, it's
            //95% - (delta% * row number)

            yOffset = baseYOffset - (deltaYOffset * i);

            for (int j = 0; j < rows[i].Count; j++)
            {
                //same as above but for each game object in the row

                //if even, then on right side, else left
                //NOTE: highest xOffset for each row is at the start
                //ALSO NOTE: base and max offset refers to the offset of the colum

                //delta% being the deference in xOffset betweenn each row

                //left side
                //5%(base xOffset) + 0-10%(15% total being max)
                //15% - (delta% * col number)

                //right side
                //85%(base xOffset) - 0-10%(95% total being max)
                //95% - (delta% * col number)
                
                GameObject gO = row[j];

                //check if even or odd
                if (j % 2 == 0)
                    xOffset = maxXOffset;
                else
                    xOffset = 1 - baseXOffset;

                xOffset -= deltaXOffset * i;

                //apply new position as a scale of screen width and height
                gO.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * xOffset, Screen.height * yOffset, Camera.main.nearClipPlane));
                gO.transform.position *= (1f - (1f / (rows.Count+1)));
                print((1f - (1f / (rows.Count*3f))));
                
                float scale = 2f / (rows.Count+1);

                if (!float.IsInfinity(scale))
                    gO.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }

    private List<List<GameObject>> generateRows()
    {
        //build the rows
        List<List<GameObject>> rows = new List<List<GameObject>>();
        List<GameObject> row = new List<GameObject>();

        int colCount = 2;

        int rowCounter = 0;         //used to deal with each row
        bool newRow = false;        //indicates when we start a new row

        //foreach (PlayerSelecter playerSelecter in playerSelecters)
        foreach (GameObject playerSelecter in playerSelecters)
        {
            //if row is finished, then add it to the rows
            if (newRow)
            {
                rows.Add(row);
                row = new List<GameObject>();
                newRow = false;
            }

            //add selecter to current row, increase counter;
            //row.Add(playerSelecter.GameObject);
            row.Add(playerSelecter);
            rowCounter++;

            //if controller full, or we have filled the row move on

            //if (playerSelector.getPlayer().inputType != InputType.ControllerFull || rowCounter >= colCount)
            if (rowCounter >= colCount)
            {
                rowCounter = 0;
                newRow = true;
            }
        }

        //add any half finished rows
        if (row.Count > 0)
            rows.Add(row);

        return rows;
    }

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Keyboard 1 Right"))
            count++;
        if (Input.GetButtonDown("Keyboard 1 Left"))
            count--;

        if (count != oldCount)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            buildPlayerSelecters();
            updateSelectors();
        }
        oldCount = count;
	}
}
