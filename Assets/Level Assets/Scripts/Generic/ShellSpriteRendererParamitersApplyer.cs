using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ShellSpriteRendererParamitersApplyer : MonoBehaviour
{
    //Adds shell sprite renderer paramiteers to all game objecys int he scene that don't have it already
    //again, any code written here will not run in game!

	// Use this for initialization
	void Awake ()
    {
        foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            if (spriteRenderer.gameObject.GetComponent<ShellSpriteRendererParamiters>() == null)
                spriteRenderer.gameObject.AddComponent<ShellSpriteRendererParamiters>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
