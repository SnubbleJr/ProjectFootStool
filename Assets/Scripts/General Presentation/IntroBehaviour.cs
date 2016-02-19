using UnityEngine;
using System.Collections;

public class IntroBehaviour : MonoBehaviour {

    public float fadeTimeIn, timeUp, fadeTimeOut;
    public AudioClip sound;

    private bool fadingIn;
    private bool fadingOut;

    private float currentTime;

    private Color currentColor;
    private Color origColor;
    private Color targetColor;

    private float lerpSpeed = 2f;

    private SpriteRenderer spriteRenderer;

    private AudioSource audioSource;

	// Use this for initialization
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sound;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.clear;
        currentColor = spriteRenderer.color;
        
        fadeIn();
    }
	
	// Update is called once per frame
	void Update ()
    {
        currentTime += Time.deltaTime;
        if (fadingIn)
        {
            currentColor = Color.Lerp(origColor, targetColor, (currentTime / fadeTimeIn));
            if (currentTime >= fadeTimeIn)
            {
                currentColor = targetColor;
                fadingIn = false;
                playSound();
                StartCoroutine(waitTime(timeUp));
            }
            spriteRenderer.color = currentColor;
        }
        if (fadingOut)
        {
            currentColor = Color.Lerp(origColor, targetColor, (currentTime / fadeTimeIn));
            if (currentTime >= fadeTimeIn)
            {
                currentColor = targetColor;
                fadingOut = false;
                finished();
            }
            spriteRenderer.color = currentColor;
        }
	}

    private void fadeIn()
    {
        origColor = spriteRenderer.color;
        targetColor = Color.white;
        fadingIn = true;
        currentTime = 0;
    }

    private IEnumerator waitTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        fadeOut();
    }

    private void playSound()
    {
        targetColor = Color.clear;
        audioSource.Play();
    }

    private void fadeOut()
    {
        origColor = spriteRenderer.color;
        targetColor = Color.clear;
        fadingOut = true;
        currentTime = 0;
    }

    private void finished()
    {
        GetComponentInParent<IntroSceneBehaviour>().introFinished();
    }
}
