using UnityEngine;
using System.Collections;

public class PlayerIndicatorBehaviour : MonoBehaviour {

    private TextMesh textMesh;
    private SpriteRenderer spriteRenderer;

    private int id;
    private Color playerColor;
    private Color currentColor;

    private bool fading = false;
    private float lerpSpeed = 0.25f;

	// Use this for initialization
	void Awake ()
    {
        textMesh = GetComponentInChildren<TextMesh>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (fading)
        {
            currentColor = Color.Lerp(currentColor, Color.clear, Time.deltaTime * lerpSpeed);
            setColors();
        }
        if (currentColor.a <= 0.1f)
        {
            currentColor = Color.clear;
            fading = false;
            setColors();
        }
    }

    public void startFading()
    {
        resetValues();
        fading = true;
    }

    private void resetValues()
    {
        fading = false;
        currentColor = playerColor;
    }

    public void setPlayer(int i, Color c)
    {
        id = i;
        playerColor = c;
        currentColor = playerColor;
        textMesh.text = "P" + id;
        setColors();
    }

    private void setColors()
    {
        textMesh.color = currentColor;
        spriteRenderer.color = currentColor;
    }
}
