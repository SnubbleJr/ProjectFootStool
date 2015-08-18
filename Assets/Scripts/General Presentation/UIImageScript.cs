using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]

public class UIImageScript : MonoBehaviour {

    private Image image;

    // Use this for initialization
    void Awake()
    {
        image = GetComponent<Image>();
    }

    public Sprite getSprite()
    {
        return image.sprite;
    }

    public void setSprite(Sprite sprite)
    {
        setSprite(sprite, false, false);
    }

    public void setSprite(Sprite sprite, bool preserveAspect, bool setNativeSize)
    {
        image.sprite = sprite;
        image.preserveAspect = preserveAspect;
        if (setNativeSize)
            image.SetNativeSize();
    }

    public Color getColor()
    {
        return image.color;
    }

    public void setColor(Color color)
    {
        image.color = color;
    }
}
