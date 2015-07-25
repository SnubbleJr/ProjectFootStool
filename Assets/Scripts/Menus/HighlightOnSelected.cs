using UnityEngine;
using System.Collections;

public class HighlightOnSelected : MonoBehaviour {

    public Color highlightedColor = Color.red;

    private GUIText guiText;
    private Color defaultColor;

    void Start()
    {
        guiText = GetComponent<GUIText>();
        defaultColor = guiText.color;
    }

    void OnEnable()
    {
        MenuEntry.selectDelegateGUI += highlight;
    }

    void OnDisable()
    {
        MenuEntry.selectDelegateGUI -= highlight;
    }

    private void highlight(bool selected)
    {
        if (selected)
        {
            guiText.color = highlightedColor;
        }
        else
        {
            guiText.color = defaultColor;
        }
    }
}
