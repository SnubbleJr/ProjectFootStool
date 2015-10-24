using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class PlayerScoreBehaviour : MonoBehaviour {

    public UITextScript scoreText, teamText;
    public ParticleSystem particleSystem;

    private Color targetColor;
    private Color activeColor;
    private Color inactiveColor;
    private Color transparentColor;

    private bool active = false;
    private bool transparent = false;

    private float lerpSpeed = 5f;

    void Update()
    {
        Color color = Color.Lerp(scoreText.getColor(), targetColor, Time.deltaTime * lerpSpeed);

        setScoreColor(color);
        setTeamColor(color);
        setParticleColor(color);
    }

    public void setScoreText(string text)
    {
        scoreText.setText(text);
    }

    public void setTeamText(string text)
    {
        teamText.setText(text);
    }

    public void setColor(Color color)
    {
        activeColor = color;
        
        transparentColor = activeColor;
        transparentColor.a = 0.3f;

        inactiveColor = activeColor * 0.5f;
        inactiveColor.a = 0.5f;

        targetColor = activeColor;
    }
    
    private void setScoreColor(Color color)
    {
        scoreText.setColor(color);
    }

    private void setTeamColor(Color color)
    {
        teamText.setColor(color);
    }

    private void setParticleColor(Color color)
    {
        particleSystem.startColor = color;
    }

    public void playParticleSystem()
    {
        particleSystem.Play();
    }

    public void stopParticleSystem()
    {
        particleSystem.Stop();
    }

    public void setActive()
    {
        if (!active)
        {
            active = true;
            stopParticleSystem();
            targetColor = activeColor;
        }
    }

    public void setInactive()
    {
        if (active)
        {
            active = false;
            stopParticleSystem();
            targetColor = inactiveColor;
        }
    }

    public void setTransparent()
    {
        if (!transparent)
        {
            transparent = true;
            targetColor = transparentColor;
        }
    }

    public void unsetTransParent()
    {
        if (transparent)
        {
            transparent = false;
            //set active to 1active so we get color change
            if (active)
            {
                active = false;
                setActive();
            }
            else
            {
                active = true;
                setInactive();
            }
        }
    }
}
