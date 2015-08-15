using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RawImage))]

public class UIRawImageScript : MonoBehaviour {
    
    private RawImage image;

    // Use this for initialization
    void Awake()
    {
        image = GetComponent<RawImage>();
    }

    public Texture getTexture()
    {
        return image.texture;
    }

    public void setTexture(Texture tex, bool resetNative)
    {
        image.texture = tex;
        if(resetNative)
            image.SetNativeSize();
    }

    public void setColor(Color color)
    {
        image.color = color;
    }
}
