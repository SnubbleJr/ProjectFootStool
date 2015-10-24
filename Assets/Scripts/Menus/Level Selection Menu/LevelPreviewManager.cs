using UnityEngine;
using System.Collections;

public class LevelPreviewManager : MonoBehaviour {

    private UITextScript creatorText;
    private UIImageScript previewImage;
    private MenuScript menuScript;

    // Use this for initialization
    void Awake()
    {
        menuScript = GetComponentInChildren<MenuScript>();
        creatorText = GetComponentInChildren<UITextScript>();
        previewImage = GetComponentInChildren<UIImageScript>();
    }

    // Update is called once per frame
    void Update()
    {
        previewLevel(menuScript.getCurrentEntry());
    }

    private void previewLevel(int option)
    {
        GameObject level = menuScript.getEntryAt(option);
        LevelBehaviour levelBehaviour = level.GetComponent<LevelBehaviour>();
        creatorText.setText("Created By " + levelBehaviour.author);
        previewImage.setSprite(levelBehaviour.thumbNail, true, false);
    }
}
