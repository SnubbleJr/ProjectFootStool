using UnityEngine;
using System.Collections;

public class PlayerFollower : MonoBehaviour {

    //make sure both players are with the bounds, zoom in or out to fit (vai the size param)
    //get the x and y values of both players and move camer to that

    public float min;
    public float max;
    public float minSize;
    public float zoomSpeed;
    public float height;

    public bool startedGame = false;          //used as an override fro the beggining of of the game - so we get a zoomout

    public GameObject invertCylinder;

    private bool debug;

    private Transform overide;

    private Rect minRect;
    private Rect maxRect;

    private GameObject[] players;

    private GameObject invertCyl1, invertCyl2;
    private bool spawnCly1 = true;

    private Camera camera;

	// Use this for initialization
	void Awake ()
    {
        camera = GetComponent<Camera>();

        minRect = new Rect((1-min)/2, (1-min)/2, min, min);
        maxRect = new Rect((1-max)/2, (1-max)/2, max, max);

        if (invertCylinder == null)
        {
            UnityEngine.Debug.LogError("No Invert Cylinder given!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (debug)
        {
            drawRect(minRect, Color.cyan);
            drawRect(maxRect, Color.magenta);
        }

        float cameraSize = camera.orthographicSize;

        //if player has one, then zoom in on them
        if (overide)
        {
            float speed = 2;

            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.5f, speed * Time.time);

            Vector3 newPos = overide.position;
            newPos.z = transform.position.z;

            transform.position = Vector3.Slerp(transform.position, newPos, speed * zoomSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(overide.position), 0.25f * Time.deltaTime);

            if(cameraSize > 0)
                camera.orthographicSize = Mathf.Lerp(cameraSize, cameraSize - 1, zoomSpeed * speed * Time.deltaTime);
        }
        else
        {
            bool inMin = true;
            bool inMax = true;

            //check if someone is out of bounds
            foreach (GameObject player in players)
            {
                if (player.GetComponent<PlayerControl>().enabled == true || !startedGame)
                {
                    inMin = minRect.Contains(camera.WorldToViewportPoint(player.transform.position)) && inMin;
                    inMax = maxRect.Contains(camera.WorldToViewportPoint(player.transform.position)) && inMax;
                }
            }

            //if no in max, then zoom out
            if (!inMax)
                camera.orthographicSize = Mathf.Lerp(cameraSize, cameraSize + 1, zoomSpeed * Time.deltaTime);

            //if in min, then zoom in
            if (inMin && cameraSize > minSize)
                camera.orthographicSize = Mathf.Lerp(cameraSize, cameraSize - 1, zoomSpeed * Time.deltaTime);

            //move camera to the midle of both players
            Vector3 midPoint = Vector3.zero;

            int numActivePlayers = 0;

            foreach (GameObject player in players)
            {
                if (player.GetComponent<PlayerControl>().enabled == true || !startedGame)
                {
                    midPoint += player.transform.position;
                    numActivePlayers++;
                }
            }

            midPoint = midPoint / numActivePlayers;

            midPoint.y += height;
            midPoint.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, midPoint, zoomSpeed * 3 * Time.deltaTime);
        }

        Time.fixedDeltaTime = 0.02F * Time.timeScale;
	}

    public void spawnInverter(Transform trans)
    {
        //spawn a invert cylinder, and destroy the old one if it exsists
        if (invertCylinder)
        {
            //we use spawncly1 as a flag to flip between
            if (spawnCly1)
            {
                if (invertCyl1)
                {
                    Destroy(invertCyl1);

                    Destroy(invertCyl2);
                }
                invertCyl1 = Instantiate(invertCylinder, trans.position, Quaternion.identity) as GameObject;
                spawnCly1 = false;
            }
            else
            {
                if (invertCyl2)
                    Destroy(invertCyl2);

                //find if cl12 esixsts, and push i back slightly so we don't overlap
                if (invertCyl1)
                    invertCyl1.transform.position = new Vector3(invertCyl1.transform.position.x, invertCyl1.transform.position.y, invertCyl1.transform.position.z + 1);

                invertCyl2 = Instantiate(invertCylinder, trans.position, Quaternion.identity) as GameObject;
                spawnCly1 = true;
            }
        }
    }

    private void drawRect(Rect rect, Color color)
    {
        Debug.DrawLine(camera.ViewportToWorldPoint(new Vector3(rect.xMin, rect.yMin, 1)), camera.ViewportToWorldPoint(new Vector3(rect.xMax, rect.yMin, 1)), color);
        Debug.DrawLine(camera.ViewportToWorldPoint(new Vector3(rect.xMax, rect.yMin, 1)), camera.ViewportToWorldPoint(new Vector3(rect.xMax, rect.yMax, 1)), color);
        Debug.DrawLine(camera.ViewportToWorldPoint(new Vector3(rect.xMax, rect.yMax, 1)), camera.ViewportToWorldPoint(new Vector3(rect.xMin, rect.yMax, 1)), color);
        Debug.DrawLine(camera.ViewportToWorldPoint(new Vector3(rect.xMin, rect.yMax, 1)), camera.ViewportToWorldPoint(new Vector3(rect.xMin, rect.yMin, 1)), color);
    }

    public void setPlayers(GameObject[] playersGO)
    {
        players = playersGO;
    }

    public void zoomInOnWinner(Transform winner)
    {
        overide = winner;
        spawnInverter(winner);
    }

    public void resetZoom()
    {
        overide = null;
        camera.orthographicSize = 5;
        camera.transform.rotation = Quaternion.identity;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    public void setDebug(bool value)
    {
        debug = value;
    }
}
