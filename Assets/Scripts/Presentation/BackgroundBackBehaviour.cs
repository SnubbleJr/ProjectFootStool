﻿using UnityEngine;
using System.Collections;

public class BackgroundBackBehaviour : MonoBehaviour {

    //gives nice control with the start, here we disable the object once it's hit the color of the camera's  intial bacground
    //this means we get a transition niceley

    //this script currently does noting

    private Color color;
    private Renderer renderer;

    void Start ()
    {
        renderer = GetComponent<Renderer>();

        color = Camera.main.backgroundColor;
    }

	// Update is called once per frame
	void Update ()
    {
	}
}
