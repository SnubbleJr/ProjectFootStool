using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelBehaviour : MonoBehaviour {

    //simple script that enables and disables the level when asked
    public string author;
    public Sprite thumbNail;
    public GameMode gameModeType;
    public MusicTrack levelTrack;
    public AudioClip customTrackIntro, customTrack;
    public int customTrackBPM;
    public bool customSpawnPoints = false;
    public float levelSize = 1;
    public bool customVisuliserColors = false;
    public Color visuliserColor1, visuliserColor2;

    private MusicComponent musicComponent;

    void Awake()
    {
        musicComponent = gameObject.AddComponent<MusicComponent>();
    }
    
    public void setLevel(bool value)
    {
        setChildren(transform, value);

        if (value)
        {
            setSpawnPoints(false);

            //play gamemode music
            musicComponent.setMusic(levelTrack, customTrackIntro, customTrack, customTrackBPM);
            musicComponent.playMusic();

            if (customVisuliserColors)
            {
                MusicVisualiserBehaviour.Instance.setCustomColors(true);
                MusicVisualiserBehaviour.Instance.setColor1(visuliserColor1);
                MusicVisualiserBehaviour.Instance.setColor2(visuliserColor2);
            }
        }
    }

    private void setChildren(Transform trans, bool value)
    {
        foreach (Transform child in trans)
        {
            child.gameObject.SetActive(value);
            setChildren(child, value);
        }
    }

    public void setSpawnPoints(bool showPoints)
    {
        if (customSpawnPoints)
        {
            List<GameObject> children = findChildWithTag(transform, "SpawnPoints");

            SpawnPointGenerator.Instance.setSpawnPoints(children, showPoints);
        }
        else
            SpawnPointGenerator.Instance.generateSpawnPoints(levelSize, showPoints);
    }

    private List<GameObject> findChildWithTag(Transform trans, string tag)
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in trans)
            if (child.CompareTag(tag))
                children.Add(child.gameObject);
            else
                children = findChildWithTag(child, tag);

        return children;
    }
}
