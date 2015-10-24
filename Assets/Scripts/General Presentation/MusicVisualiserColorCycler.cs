using UnityEngine;
using System.Collections;

[System.Serializable]
public class MusicVisualiserColorSchemes
{
    public GameMode gameMode;
    public Color topColor, bottomColor;
}

public class MusicVisualiserColorCycler : MonoBehaviour {

    public MusicVisualiserColorSchemes[] colorSchemes;
    public float speed;

    private MusicVisualiserBehaviour musicVisualiser;

    private MusicVisualiserColorSchemes currentScheme;

	// Use this for initialization
	void Awake ()
    {
        musicVisualiser = GetComponent<MusicVisualiserBehaviour>();
        currentScheme = colorSchemes[0];
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!musicVisualiser.getCustomColors())
        {
            musicVisualiser.setColor1(Color.Lerp(musicVisualiser.getColor1(), currentScheme.topColor, Time.deltaTime * speed));
            musicVisualiser.setColor2(Color.Lerp(musicVisualiser.getColor2(), currentScheme.bottomColor, Time.deltaTime * speed));
        }
    }

    public void setGameMode(GameMode gameMode)
    {
        musicVisualiser.setCustomColors(false);
        for(int i = 0; i < colorSchemes.Length; i++)
            if (colorSchemes[i].gameMode == gameMode)
                currentScheme = colorSchemes[i];
    }
}
