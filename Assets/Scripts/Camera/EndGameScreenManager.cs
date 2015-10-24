using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndGameScreenManager : MonoBehaviour {

    public RenderTexture[] RTs;

    private Camera mainCamera;
    private const int numberOfRTs = 3;
    private const int frameDelay = 3;   //number of frames that we will hold for
    private int currentFrame = 0;       //count of frames we have held for
    private const int frameDelayBeforeDisable = 12;
    private int currentFrameBeforeDisable = 0;
    private Camera camera;
    private bool goingToDisplay = false;
    private bool displaying = false;

    //Here is a private reference only this class can access
    private static EndGameScreenManager instance;

    //This is the public reference that other classes will use
    public static EndGameScreenManager Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<EndGameScreenManager>();
            return instance;
        }
    }

	// Use this for initialization
	void Awake ()
    {
        mainCamera = GetComponent<Camera>();
        camera = makeCamera();
        setUpCamera(camera);

        List<RenderTexture> temp = new List<RenderTexture>();
        for (int i = 0; i < numberOfRTs; i++)
        {
            RenderTexture RT = new RenderTexture((int)(camera.pixelWidth * camera.rect.width), (int)(camera.pixelHeight * camera.rect.height), 16);
            RT.name = "End Game RT " + i;
            temp.Add(RT);
        }

        RTs = temp.ToArray();

        camera.targetTexture = RTs[numberOfRTs - 1];
	}
	
    private Camera makeCamera()
    {
        GameObject cameraGO = new GameObject("End Game Camera", typeof(Camera), typeof(EndGameScreenBehaviour));
        cameraGO.transform.SetParent(transform, false);
        return cameraGO.GetComponent<Camera>();
    }

    private void setUpCamera(Camera camera)
    {
        camera.orthographic = true;
        camera.rect = new Rect(0,0,3,3);
        camera.backgroundColor = Color.black;
        camera.cullingMask = ~(1 << LayerMask.NameToLayer("TransparentFX"));
    }

    private void updateCameraSize(Camera camera)
    {
        camera.orthographicSize = mainCamera.orthographicSize;
        camera.transform.localPosition = Vector3.down * (mainCamera.orthographicSize / 2);
    }

    //have on one camera
    //but many RT's
    //have them in a loop

    void OnPreRender()
    {
        if (!displaying)
        {
            //add a frame delay so we get a nice kill for final image
            if (goingToDisplay)
            {
                if (++currentFrameBeforeDisable >= frameDelayBeforeDisable)
                    displaying = true;
            }
            else
            {
                //we don't do this if we are going to display - we want to delay the last frame

                //if we have held the frame for long enough
                if (++currentFrame >= frameDelay)
                {
                    currentFrame = 0;
                    //we reorder the RTs, so we will always be setting the target to the same number (but a different RT) and then shuffle it up
                    if (!goingToDisplay)
                    {
                        reorderRTs();
                        camera.targetTexture = RTs[numberOfRTs - 1];
                    }
                }
            }
        }
        else
        {
            camera.enabled = false;
        }

    }    

    void Update()
    {
        updateCameraSize(camera);
    }
    
    private void reorderRTs()
    {
        //shuffle everthing up
        //we don't need to wory about the last rt, as that is about to get written into

        RenderTexture temp = RTs[0];

        for (int i = 0; i < numberOfRTs - 1; i++)
            RTs[i] = RTs[i + 1];

        RTs[numberOfRTs - 1] = temp;
    }

    public RenderTexture getRT(int id)
    {
        goingToDisplay = true;
        camera.GetComponent<EndGameScreenBehaviour>().set(true);
        currentFrameBeforeDisable = 0;
        return RTs[id];
    }

    public int getRTNoCount()
    {
        return numberOfRTs;
    }

    public void resetRTs()
    {
        camera.enabled = true;
        goingToDisplay = false;
        displaying = false;
        camera.GetComponent<EndGameScreenBehaviour>().set(false);
        currentFrameBeforeDisable = 0;
    }
}
