using UnityEngine;
using System.Collections;

public class CountdownBehaviour : MonoBehaviour {

    public SFX countDownSound = SFX.Countdown;
    public SFX countDownGoSound = SFX.Go;

    private int countDownStart = 4;
    private int countDownTime;
    private bool countingDown = false;

    private PlayerManagerBehaviour playerManager;
    private UITextScript uiText;

    private Camera canvasCamera;

	// Use this for initialization
    void Awake()
    {
        canvasCamera = GetComponentInParent<Canvas>().worldCamera;
        playerManager = GetComponentInParent<PlayerManagerBehaviour>();
        uiText = GetComponent<UITextScript>();
    }
	
    void OnEnable()
    {
        BeatDetector.beatDetected += countDown;
    }

    void OnDisable()
    {
        BeatDetector.beatDetected -= countDown;
    }
    
    public void startCountdown()
    {
        countDownTime = countDownStart;
        moveText();
        uiText.setText(countDownTime.ToString());
        uiText.setColor(Color.white);
        countingDown = true;
    }
    
    public void stopCountdown()
    {
        //disable countdown incase it was still showing
        countDownTime = -2;
        countingDown = false;
    }

    private void countDown(bool onBeat)
    {
        if (countingDown)
        {
            //countdown to -1, have to pad out due to double beat on end on intro start of main
            if (countDownTime > -1)
            {
                SFXManagerBehaviour.Instance.playSound(countDownSound);
                countDownTime--;
                uiText.setText(countDownTime.ToString());
            }
            else
            {
                stopCountdown();
                uiText.setText("");
            }

            //checked at 0, so players are active on go
            if (countDownTime <= 0 && countDownTime >= -1)
            {
                SFXManagerBehaviour.Instance.playSound(countDownGoSound);
                uiText.setColor(Color.yellow);
                uiText.setText("GO!");
                playerManager.countDownFinished();
            }

            moveText();
        }
    }

    private void moveText()
    {
        //moves the text to a different side of the screen based on what number are on
        
        switch (countDownTime)
        {
            case (4):
                transform.position = canvasCamera.ScreenToWorldPoint(new Vector3(canvasCamera.pixelWidth / 2, canvasCamera.pixelHeight, 10));
                break;
            case (3):
                transform.position = canvasCamera.ScreenToWorldPoint(new Vector3(canvasCamera.pixelWidth / 2, 0, 10));
                break;
            case (2):
                transform.position = canvasCamera.ScreenToWorldPoint(new Vector3(0, canvasCamera.pixelHeight / 2, 10));
                break;
            case (1):
                transform.position = canvasCamera.ScreenToWorldPoint(new Vector3(canvasCamera.pixelWidth, canvasCamera.pixelHeight / 2, 10));
                break;
            case (0):
                transform.position = canvasCamera.ScreenToWorldPoint(new Vector3(canvasCamera.pixelWidth / 2, canvasCamera.pixelHeight / 2, 10));
                break;
            default:
                break;
        }
    }
}
