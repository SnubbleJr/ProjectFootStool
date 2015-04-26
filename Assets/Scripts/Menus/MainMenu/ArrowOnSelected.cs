using UnityEngine;
using System.Collections;

public class ArrowOnSelected : MonoBehaviour {

    public Color highlightedColor;
    public Texture selectedTexture;

    void OnEnable()
    {
        MenuEntry.selectDelegateGUI += drawArrow;
    }

    void OnDisable()
    {
        MenuEntry.selectDelegateGUI -= drawArrow;
    }

    private void drawArrow(bool selected)
    {
        Vector3 coOrds = Camera.main.ViewportToScreenPoint(transform.position);

        //because we rotated it, we have to use y,x not x,y, make sure anchor is top left
        GUIUtility.RotateAroundPivot(90, new Vector2(30, 30));
        GUI.contentColor = highlightedColor;
        GUI.Label(new Rect((Screen.height - coOrds.y), (coOrds.x - Screen.width) * 0.8f, 50, 50), selectedTexture);
    }
}
