using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicVisualiserBehaviour : MonoBehaviour {

    public GameObject visualiserBar;
    public int minQuantity;
    public int maxQuantity;

    public float spacingPercentage = 0.2f;

    private Color color1;
    private Color color2;

    private List<GameObject> bars = new List<GameObject>();
    private int quantity;
    private int prevQauntity;

    private float barHeight;
    private float barHeightPerUnit;
    private float barScale;

    private float lerpSpeed = 30;

    private AudioSource currentTrack;
    float samplesPerBar;

	// Use this for initialization
	void Awake ()
    {
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
                bars.Add(bar);
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
        barScale = (Camera.main.pixelHeight / (bars.Count + totalSpacing)) * barHeightPerUnit;

        foreach (GameObject bar in bars)
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
        float nearClipPlane = Camera.main.nearClipPlane*2;
        float x0 = Screen.width / 2;

        return Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight - (((baseSpace / 2) * (((i + 1) * 2) - 1)) + ((i + 1) * (baseSpace * spacingPercentage))), nearClipPlane));
    }

    private void beatDetected(bool offBeat)
    {
        if (!offBeat)
            updateQuantity();
    }

    private void updateQuantity()
    {
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
            bars[i].GetComponent<SpriteRenderer>().color = Color.Lerp(color1, color2, (float)i / (float)bars.Count);
    }

	// Update is called once per frame
	void Update ()
    {
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
        {
            setupTrack();
            return;
        }

        float[] spectrum = new float[1024];
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
    }

    public Color getColor2()
    {
        return color2;
    }

    public void setColor2(Color color)
    {
        color2 = color;
    }
}
