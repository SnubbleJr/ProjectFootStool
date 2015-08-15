using UnityEngine;
using System.Collections;

public class EndGameScreenBehaviour : MonoBehaviour
{
    //doesn't do a lot
    private PlayerFollower playerFollower;
    private Camera camera;

    private const float zoomSpeed = 3f;

    private bool active = false;
    private Transform oldTrans;

    // Use this for initialization
    void Awake()
    {
        playerFollower =GetComponentInParent<PlayerFollower>();
        camera = GetComponent<Camera>();
        oldTrans = transform;
    }

    // Update is called once per frame
    void Update()
    {/*
        if (active)
        {
            float cameraSize = camera.orthographicSize;

            if (playerFollower != null)
            {
                float speed = 2;

                Vector3 midPoint = playerFollower.getMidPoint();
                Vector3 newPos = midPoint;
                newPos.z = transform.position.z;

                //transform.position = Vector3.Lerp(transform.position, newPos, zoomSpeed * Time.deltaTime);
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(midPoint), 0.25f * Time.deltaTime);

                //if (cameraSize > 0)
                    //camera.orthographicSize = Mathf.Lerp(cameraSize, cameraSize - 1, zoomSpeed * speed * Time.deltaTime);
            }
            else
                playerFollower = GetComponentInParent<PlayerFollower>();
        }*/
    }

    public void set(bool value)
    {
        active = value;

        if (active)
        {
            camera.backgroundColor = Color.white;
        }
        else
        {
            //reset
            camera.backgroundColor = Color.black;
        }
    }
}
