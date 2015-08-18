using UnityEngine;
using System.Collections;

public class PanelSlider : MonoBehaviour {

    public delegate void PanelSliderDelegate(PanelSlider instantce);
    public static event PanelSliderDelegate arrivedAtDestination;
    public static event PanelSliderDelegate exited;

    public SFX arrivingSound;
    public SFX arrivedSound;
    public SFX leavingSound;
    public SFX leftSound;

    private Vector2 destination, start;

    private bool toDest = false;
    private bool toExit = false;

	private float speed = 5000.00F;

    public void setSpeed(float sp)
    {
        speed = sp;
    }

    public void setTime(float time)
    {
        start = transform.localPosition;
        setSpeed(Vector2.Distance(start, destination) / time);
    }

    public void moveToDestination()
    {
        //lerp  from start to dest
        start = transform.localPosition;
        toDest = true;
    }

    public void depart()
    {
        destination = -start;
        toExit = true;
    }

    void Update()
    {
        if (toDest)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, destination, speed * Time.deltaTime);

            if ((Vector2)transform.localPosition == destination)
            {
                //inform of arrival
                toDest = false;
                if (arrivedAtDestination != null)
                {
                    arrivedAtDestination(this);
                }
            }
        }
        if (toExit)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, destination, speed * Time.deltaTime);

            RectTransform rectTrans = transform.parent.GetComponent<RectTransform>();
            if (!rectTrans.rect.Contains(transform.localPosition * 0.9f))
            {
                //inform of arrival
                if (exited != null)
                    exited(this);
            }
        }
    }

    public void backToStart()
    {
        transform.localPosition = start;
    }

    public void setDestination(Vector2 dest)
    {
        destination = dest;
    }

    public void playArrivedSound()
    {
        SFXManagerBehaviour.Instance.playSound(arrivedSound);
    }

    public void playArringSound()
    {
        SFXManagerBehaviour.Instance.playSound(arrivingSound);
    }

    public void playLeavingSound()
    {
        SFXManagerBehaviour.Instance.playSound(leavingSound);
    }

    public void playLeftSound()
    {
        SFXManagerBehaviour.Instance.playSound(leftSound);
    }

    public void playSound(SFX sound)
    {
        SFXManagerBehaviour.Instance.playSound(sound);
    }
}
