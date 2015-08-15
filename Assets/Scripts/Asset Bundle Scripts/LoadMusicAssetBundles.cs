using UnityEngine;
using System.Collections;

public class LoadMusicAssetBundles : MonoBehaviour {

    //Here is a private reference only this class can access
    private static LoadMusicAssetBundles instance;

    //This is the public reference that other classes will use
    public static LoadMusicAssetBundles Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<LoadMusicAssetBundles>();
            return instance;
        }
    }

    public IEnumerator loadMusicABs()
    {
        yield return StartCoroutine(loadAsset("Music", "Koth.mp3", MusicTrack.KothGame));
        yield return StartCoroutine(loadAsset("Music", "Race.mp3", MusicTrack.RaceGame));
        yield return StartCoroutine(loadAsset("Music", "Stock.mp3", MusicTrack.StockGame));
        yield return StartCoroutine(loadAsset("Music", "Menu Hype Layer.mp3", MusicTrack.PlayerSelectionMenu));
        yield return StartCoroutine(loadAsset("Music", "Menu Base Layer.ogg", MusicTrack.MainMenu));
    }

    private IEnumerator loadAsset(string path, string asset, MusicTrack musicTrack)
    {
        yield return StartCoroutine(LoadAssetFromAssetBundle.Instance.DownloadAssetBundle<AudioClip>(asset, path + "/", "file:///" + Application.dataPath + "AssetBundles/" + path + ".unity3d", 0));
        Debug.Log("Loaded " + asset + " in " + Application.dataPath + "AssetBundles/" + path + ".unity3d");
        yield return StartCoroutine(assignTrack(musicTrack));
    }

    private IEnumerator assignTrack(MusicTrack musicTrack)                                                           
    {
        MusicManagerBehaviour musicManager = MusicManagerBehaviour.Instance;
        GameObject GO = new GameObject(musicTrack.ToString());
        GO.transform.SetParent(musicManager.transform, false);
        GO.AddComponent<AudioSource>();
        GO.GetComponent<AudioSource>().clip = (AudioClip)LoadAssetFromAssetBundle.Instance.Obj;

        switch(musicTrack)
        {
            case MusicTrack.KothGame:
                musicManager.kothMusic = GO.GetComponent<AudioSource>();
                break;
            case MusicTrack.RaceGame:
                musicManager.raceMusic = GO.GetComponent<AudioSource>();
                break;
            case MusicTrack.StockGame:
                musicManager.stockMusic = GO.GetComponent<AudioSource>();
                break;
            case MusicTrack.PlayerSelectionMenu:
                musicManager.playerSelectionMusic = GO.GetComponent<AudioSource>();
                break;
            case MusicTrack.MainMenu:
                musicManager.menuMusic = GO.GetComponent<AudioSource>();
                break;
        }
        yield return null;
    }
}
