using UnityEngine;
using System.Collections;

public class HillActivationScript : MonoBehaviour {

    //activates and deactivates it without calling gameobject setactive
    //this means we can use the find with tag method

    private BoxCollider2D bc2D;
    private SpriteRenderer sr;

    void Awake()
    {
        bc2D = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void activate()
    {
        bc2D.enabled = true;
        sr.enabled = true;
    }

    public void deactivate()
    {
        bc2D.enabled = false;
        sr.enabled = false;
    }
}
