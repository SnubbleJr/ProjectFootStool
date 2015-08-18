using UnityEngine;
using UnityEngine.Audio;
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
        yield return StartCoroutine(assignCustomTrack());
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
        yield return null;
        
        AudioSource audioSource = GO.AddComponent<AudioSource>();
        audioSource.clip = (AudioClip)LoadAssetFromAssetBundle.Instance.Obj;
        audioSource.loop = true;

        yield return null;

        AudioMixer musicMixer = Resources.Load("Audio Mixers/MusicMixer") as AudioMixer;
        
        switch(musicTrack)
        {
            case MusicTrack.KothGame:
                audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Koth")[0];
                musicManager.kothMusic = audioSource;
                break;
            case MusicTrack.RaceGame:
                audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Race")[0];
                musicManager.raceMusic = audioSource;
                break;
            case MusicTrack.StockGame:
                audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Stock")[0];
                musicManager.stockMusic = audioSource;
                break;
            case MusicTrack.PlayerSelectionMenu:
                audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Menu Layer")[0];
                musicManager.playerSelectionMusic = audioSource;
                break;
            case MusicTrack.MainMenu:
                audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Menu Base")[0];
                musicManager.menuMusic = audioSource;
                break;
        }
        yield return null;
    }

    private IEnumerator assignCustomTrack()
    {
        MusicManagerBehaviour musicManager = MusicManagerBehaviour.Instance;
        GameObject GO = new GameObject("Custom");
        GO.transform.SetParent(musicManager.transform, false);
        yield return null;

        AudioSource audioSource = GO.AddComponent<AudioSource>();
        audioSource.loop = true;

        yield return null;

        AudioMixer musicMixer = Resources.Load("Audio Mixers/MusicMixer") as AudioMixer;

        audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Custom")[0];
        musicManager.customMusic = audioSource;
        yield return null;
    }
}
