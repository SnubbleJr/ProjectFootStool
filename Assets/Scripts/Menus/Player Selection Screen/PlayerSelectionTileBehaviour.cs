using UnityEngine;
using System.Collections;

public class PlayerSelectionTileBehaviour : MonoBehaviour {

    private bool selected = true;

    private PlayerColor pColor;
    private PlayerSprite pSprite;

    //private float alpha;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        if (pColor != null)
            spriteRenderer.color = pColor.color;
    }

    public PlayerColor getColor()
    {
        return pColor;
    }

    public void setAlpha(float alpha)
    {
        if (spriteRenderer.color.a == alpha)
            return;

        if (alpha == 0)
            spriteRenderer.enabled = false;
        else
        {
            spriteRenderer.enabled = true;
            Color aColor = spriteRenderer.color;
            aColor.a = alpha;
            spriteRenderer.color = aColor;
        }
    }

    public void setSprite(PlayerSprite sprite)
    {
        pSprite = sprite;
        spriteRenderer.sprite = pSprite.sprite;
    }

    public PlayerSprite getSprite()
    {
        return pSprite;
    }
}
