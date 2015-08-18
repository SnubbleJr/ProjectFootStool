using UnityEngine;
using System.Collections;

public class LevelBehaviour : MonoBehaviour {

    //simple script that enables and disables the level when asked
    public GameMode gameModeType;
    public MusicTrack levelTrack;
    public AudioClip customTrack;
    public float levelSize = 1;

    private MusicComponent musicComponent;

    void Awake()
    {
        musicComponent = gameObject.AddComponent<MusicComponent>();
    }
    
    public void setLevel(bool value)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(value);

        if (value)
        {
            SpawnPointGenerator.Instance.generateSpawnPoints(levelSize, false);

            //play gamemode music
            musicComponent.setMusic(levelTrack, customTrack);
            musicComponent.playMusic();
        }
    }
}
