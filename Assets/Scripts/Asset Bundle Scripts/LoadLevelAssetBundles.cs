using UnityEngine;
using System.Collections;

public class LoadLevelAssetBundles : MonoBehaviour {

    //Here is a private reference only this class can access
    private static LoadLevelAssetBundles instance;

    //This is the public reference that other classes will use
    public static LoadLevelAssetBundles Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<LoadLevelAssetBundles>();
            return instance;
        }
    }

    public IEnumerator loadLevelABs()
    {
        yield return StartCoroutine(loadAsset("Music", "Koth.mp3", MusicTrack.KothGame));
    }

    private IEnumerator loadAsset(string path, string asset, MusicTrack musicTrack)
    {
        yield return StartCoroutine(LoadAssetFromAssetBundle.Instance.DownloadAssetBundle<AudioClip>(asset, path + "/", "file:///" + Application.dataPath + "AssetBundles/" + path + ".unity3d", 0));
        yield return StartCoroutine(assignTrack(musicTrack));
    }

    private IEnumerator assignTrack(MusicTrack musicTrack)
    {
        MusicManagerBehaviour musicManager = MusicManagerBehaviour.Instance;
        GameObject GO = new GameObject(musicTrack.ToString());
        GO.transform.SetParent(musicManager.transform, false);
        GO.AddComponent<AudioSource>();
        GO.GetComponent<AudioSource>().clip = (AudioClip)LoadAssetFromAssetBundle.Instance.Obj;

        switch (musicTrack)
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
