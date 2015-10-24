using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicVisualiserBehaviour : MonoBehaviour {

    public GameObject visualiserBar;
    public int minQuantity;
    public int maxQuantity;

    public float spacingPercentage = 0.2f;

    [Range(0f, 1f)] public float oppacityScale;

    private Color color1;
    private Color color2;

    private bool customColors = false;

    private List<SpriteRenderer> bars = new List<SpriteRenderer>();
    private int quantity;
    private int prevQauntity;

    private float barHeight;
    private float barHeightPerUnit;
    private float barScale;

    private float lerpSpeed = 30;

    private Camera mainCamera;
    private float mainNearClipPlane;
    private float mainWidth;
    private float mainHeight;

    private AudioSource currentTrack;
    private float samplesPerBar;
    private float[] spectrum = new float[1024];

    //Here is a private reference only this class can access
    private static MusicVisualiserBehaviour instance;

    //This is the public reference that other classes will use
    public static MusicVisualiserBehaviour Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<MusicVisualiserBehaviour>();
            return instance;
        }
    }

	// Use this for initialization
	void Awake ()
    {
        mainCamera = Camera.main;
        mainNearClipPlane = mainCamera.nearClipPlane;
        mainWidth = mainCamera.pixelWidth;
        mainHeight = mainCamera.pixelHeight;
        barHeight = visualiserBar.GetComponent<SpriteRenderer>().sprite.rect.height;
        barHeightPerUnit = barHeight / visualiserBar.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
    }

    void OnEnable()
    {
        BeatDetector.beatDetected += beatDetected;
        MusicManagerBehaviour.songStarted += setupTrack;
        MusicManagerBehaviour.songStopped += disableTrack;
    }

    void OnDisable()
    {
        BeatDetector.beatDetected -= beatDetected;
        MusicManagerBehaviour.songStarted -= setupTrack;
        MusicManagerBehaviour.songStopped -= disableTrack;
    }

    private void addBars(int amount)
    {
        if (amount > 0)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject bar = Instantiate(visualiserBar, getBarPos(bars.Count), Quaternion.identity) as GameObject;
                bar.transform.SetParent(transform, false);
                bars.Add(bar.GetComponent<SpriteRenderer>());
            }
        }
    }

    private void removeBars(int amount)
    {
        if (amount > 0)
        {
            for (int i = bars.Count - amount; i < bars.Count; )
            {
                Destroy(bars[i]);
                bars.RemoveAt(i);
            }
        }
    }

    private void scaleBars()
    {
        float totalSpacing = (spacingPercentage * (bars.Count - 1)) + 1;    //+ 1 so we hve space at top and bottom
        barScale = (mainHeight / (bars.Count + totalSpacing)) * barHeightPerUnit;

        foreach (SpriteRenderer bar in bars)
            bar.transform.localScale = Vector3.Lerp(bar.transform.localScale, new Vector3(0.5f, barScale, 1), Time.deltaTime * lerpSpeed);
    }

    private void positionBars()
    {
        for (int i = 0; i < bars.Count; i++)
        {
            Vector3 barPos = getBarPos(i);
            if (Vector3.Distance(bars[i].transform.position, barPos) < 0.1f)
                bars[i].transform.position = barPos;
            else
                bars[i].transform.position = Vector3.Lerp(bars[i].transform.position, barPos, Time.deltaTime * lerpSpeed);
        }
    }

    private Vector3 getBarPos(int i)
    {
        float baseSpace = barScale / barHeightPerUnit;       //the pixelehight of the current bar size
        float nearClipPlane = mainNearClipPlane*2;
        float x0 = Screen.width / 2;

        return mainCamera.ScreenToWorldPoint(new Vector3(mainWidth / 2, mainHeight - (((baseSpace / 2) * (((i + 1) * 2) - 1)) + ((i + 1) * (baseSpace * spacingPercentage))), nearClipPlane));
    }

    private void beatDetected(bool offBeat)
    {
        if (!offBeat)
            updateQuantity();
    }

    private void updateQuantity()
    {
        mainWidth = mainCamera.pixelWidth;
        mainHeight = mainCamera.pixelHeight;

        quantity = (((int)Mathf.PingPong(Time.time*4, maxQuantity - minQuantity)) + minQuantity);
        //quantity = maxQuantity;

	    if (quantity > prevQauntity)
            addBars(quantity - prevQauntity);
        if (quantity < prevQauntity)
            removeBars(prevQauntity - quantity);

        prevQauntity = quantity;
    }

    private void colorBars()
    {
        for (int i = 0; i < bars.Count; i++)
            bars[i].color = Color.Lerp(color1, color2, (float)i / (float)bars.Count);
    }

	// Update is called once per frame
	void Update ()
    {
        spacingPercentage = 20f / mainCamera.orthographicSize;

        scaleBars();
        positionBars();
        colorBars();

        /*
         * 
         * /\   building and maintaing the structre of the bars
         * 
         * \/   moving the bars to the music
         * 
         * */
        
        if (currentTrack == null)
            setupTrack();

        stretchBarsToMusic();
    }
    
    private void setupTrack()
    {
        currentTrack = MusicManagerBehaviour.Instance.getPlayingMusic();
    }

    private void disableTrack()
    {
        currentTrack = null;
    }

    private void stretchBarsToMusic()
    {
        if (currentTrack == null)
            return;

        currentTrack.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        if (bars.Count > 0)
            //find out how many samples are represented by a bar
            samplesPerBar = spectrum.Length / bars.Count;

        for (int i = 0; i < bars.Count; i++)
        {
            Vector3 scale = bars[i].transform.localScale;

            int from = (int)(samplesPerBar * i);
            int to = (int)(samplesPerBar * (i + 1));

            scale.x = (Mathf.Log(to-from) / Mathf.Log(averageSamples(spectrum, from, to)));
            bars[i].transform.localScale = scale;
        }
    }

    private float averageSamples(float[] samples, int from, int to)
    {
        //average out each ampe group for a bar and scale it
        float average = 0;

        try
        {
            for (int i = from; i <= to; i++)
                average += samples[i];
        }
        catch
        {
        }

        return (average / (to - from));
    }

    public Color getColor1()
    {
        return color1;
    }

    public void setColor1(Color color)
    {
        color1 = color;
        color1.a *= oppacityScale;
    }

    public Color getColor2()
    {
        return color2;
    }

    public void setColor2(Color color)
    {
        color2 = color;
        color2.a *= oppacityScale;
    }

    public bool getCustomColors()
    {
        return customColors;
    }

    public void setCustomColors(bool val)
    {
        customColors = val;
    }
}
