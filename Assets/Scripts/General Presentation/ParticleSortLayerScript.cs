using UnityEngine;
using System.Collections;

public class ParticleSortLayerScript : MonoBehaviour {

    public int sortingOrder;

    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<ParticleSystem>().GetComponent<Renderer>();

        //Change Foreground to the layer you want it to display on 
        //You could prob. make a public variable for this
        renderer.sortingLayerName = "Foreground";
        renderer.sortingOrder = sortingOrder;
    }
}
