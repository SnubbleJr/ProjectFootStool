using UnityEngine;
using System.Collections;

public class ControllerNumberAssigner : MonoBehaviour {

    //sets the relavent controller number and picture

    public GameObject ringSprite, controlSprite;

    public Sprite keyboard1, keyboard2, keyboard3, keyboard4;
    public Sprite controllerFull, controllerHalfL, controllerHalfR;
    public Sprite ring1, ring2, ring3, ring4;

    private SpriteRenderer ringSpriteRenderer;
    private SpriteRenderer controlSpriteRenderer;

	// Use this for initialization
	void Awake () {
        ringSpriteRenderer = ringSprite.GetComponent<SpriteRenderer>();
        controlSpriteRenderer = controlSprite.GetComponent<SpriteRenderer>();
	}
	
    public void setControl(PlayerInputScheme playerInputScheme)
    {
        switch(playerInputScheme.inputType)
        {
            case InputType.Keyboard:
                setKeyboard(playerInputScheme.id);
                break;
            case InputType.ControllerFull:
                controlSpriteRenderer.sprite = controllerFull;
                setControllerRing(playerInputScheme.controller);
                break;
            case InputType.ControllerHalfL:
                controlSpriteRenderer.sprite = controllerHalfL;
                setControllerRing(playerInputScheme.controller);
                break;
            case InputType.ControllerHalfR:
                controlSpriteRenderer.sprite = controllerHalfR;
                setControllerRing(playerInputScheme.controller);
                break;
        }
    }

    private void setKeyboard(int playerNo)
    {
        switch (playerNo)
        {
            case 1:
                controlSpriteRenderer.sprite = keyboard1;
                break;
            case 2:
                controlSpriteRenderer.sprite = keyboard2;
                break;
            case 3:
                controlSpriteRenderer.sprite = keyboard3;
                break;
            case 4:
                controlSpriteRenderer.sprite = keyboard4;
                break;
        }

        //clear controller rings sprite and text
        SendMessage("enterText", "");
        ringSpriteRenderer.sprite = null;
    }

    private void setControllerRing(int no)
    {
        //we only have 4 rings, so set 5,6,7,8 to rings 1,2,3,4
        int controllerNo = (no % 4);
        switch (controllerNo)
        {
            case 1:
                ringSpriteRenderer.sprite = ring1;
                break;
            case 2:
                ringSpriteRenderer.sprite = ring2;
                break;
            case 3:
                ringSpriteRenderer.sprite = ring3;
                break;
            case 4:
                ringSpriteRenderer.sprite = ring4;
                break;
        }

        SendMessage("enterText", no.ToString());
    }
}
