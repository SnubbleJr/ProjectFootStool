using UnityEngine;
using System.Collections;

public class PlayerSelectionTileBehaviour : MonoBehaviour {

    private bool selected = true;

    private PlayerColor pColor;
    private PlayerSprite pSprite;

    private float alpha;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
    void OnGUI()
    {
        
    }

	// Update is called once per frame
	void Update ()
    {
        if (pColor != null)
            spriteRenderer.color = pColor.color;

        if (pSprite != null)
            spriteRenderer.sprite = pSprite.sprite;

        Color aColor = spriteRenderer.color;
        aColor.a = alpha;
        spriteRenderer.color = aColor;
        /*
        if (selected)
        {
        }
        */
	}

    public void setSelected(bool value)
    {
        selected = value;
    }

    public bool getSelected()
    {
        return selected;
    }

    public void setColor(PlayerColor color)
    {
        pColor = color;
    }

    public PlayerColor getColor()
    {
        return pColor;
    }

    public void setAlpha(float talpha)
    {
        alpha = talpha;
    }

    public void setSprite(PlayerSprite sprite)
    {
        pSprite = sprite;
    }

    public PlayerSprite getSprite()
    {
        return pSprite;
    }
}
