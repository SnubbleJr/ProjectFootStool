using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UITextScript))]

public class TipBehaviour : MonoBehaviour {

    //slowly moves to the left, gets a tip for tip bar
    //is told when to reset by tip end

    private TipBarBehaviour tipBar;
    private GameObject tipEnd;
    private UITextScript uiTextScript;

    private float speed = 10;

    private Vector3 startPos;
    private float screenWdith;

    private bool entered = false;
    private bool exited = false;

	void Awake ()
    {
        startPos = transform.localPosition;

        tipBar = TipBarBehaviour.Instance;
	    //create tip end
        tipEnd = transform.GetChild(0).gameObject;
        uiTextScript = GetComponent<UITextScript>();

        resetTip();
    }

    public void resetTip()
    {
        entered = false;
        exited = false;
        
        setPos();
        setTipText();
	}
	
    private void setPos()
    {
        screenWdith = TipBarBehaviour.Instance.transform.GetComponentInParent<Canvas>().pixelRect.width;
        transform.localPosition = startPos;
    }

    private void setTipText()
    {
        //add space at beggining to space out tip, and also so we don't see it spawn on screen
        uiTextScript.setText("     " + tipBar.getTip());
    }

	// Update is called once per frame
	void Update ()
    {
        transform.position += (screenWdith * speed) * Vector3.left * Time.deltaTime;
        if (tipEnd)
        {
            if (tipEnd.transform.position.x < screenWdith && !entered)
            {
                entered = true;
                TipBarBehaviour.Instance.tipEntered(this);
            }
            if (tipEnd.transform.position.x < -10 && !exited)
            {
                exited = true;
                TipBarBehaviour.Instance.tipExited(this);
            }
        }
	}

    public float getSpeed()
    {
        return speed;
    }

    public void setSpeed(float var)
    {
        speed = var;
    }
}
