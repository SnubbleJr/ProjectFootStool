using UnityEngine;
using System.Collections;

public class MenuOptionSelectionDisplay : MonoBehaviour {

    public Texture2D texture;

    void OnGUI()
    {
        GUIUtility.RotateAroundPivot(90, new Vector2(30, 30));
        GUI.Label(new Rect(140, -300, 150, 20), texture);
    }
}
