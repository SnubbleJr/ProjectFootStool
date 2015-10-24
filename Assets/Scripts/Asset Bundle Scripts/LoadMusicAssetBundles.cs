using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class LoadMusicAssetBundles : MonoBehaviour {

    public AudioClip stockIntro, stockMain, raceIntro, raceMain, kothIntro, kothMain, menuBase, menuHype;

    private const string kothFileName = "Koth.ogg";
    private const string raceFileName = "Race.ogg";
    private const string stockFileName = "Stock.ogg";
    private const string menuBaseFileName = "Menu Base.ogg";
    private const string menuHypeFileName = "Menu Hype Layer.ogg";

    private const string kothIntroFileName = "Koth Intro.ogg";
    private const string raceIntroFileName = "Race Intro.ogg";
    private const string stockIntroFileName = "Stock Intro.ogg";

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
        string path = "Music";

#if UNITY_EDITOR
        yield return StartCoroutine(loadAsset(path, kothFileName));
        yield return StartCoroutine(assignTrack(MusicTrack.KothGame));
        yield return StartCoroutine(loadAsset(path, raceFileName));
        yield return StartCoroutine(assignTrack(MusicTrack.RaceGame));
        yield return StartCoroutine(loadAsset(path, stockFileName));
        yield return StartCoroutine(assignTrack(MusicTrack.StockGame));
        yield return StartCoroutine(loadAsset(path, menuHypeFileName));
        yield return StartCoroutine(assignTrack(MusicTrack.PlayerSelectionMenu));
        yield return StartCoroutine(loadAsset(path, menuBaseFileName));
        yield return StartCoroutine(assignTrack(MusicTrack.MainMenu));
        yield return StartCoroutine(assignCustomTrack());

        yield return StartCoroutine(loadAsset(path, kothIntroFileName));
        yield return StartCoroutine(assignTrack(MusicTrack.KothGame, true));
        yield return StartCoroutine(loadAsset(path, raceIntroFileName));
        yield return StartCoroutine(assignTrack(MusicTrack.RaceGame, true));
        yield return StartCoroutine(loadAsset(path, stockIntroFileName));
        yield return StartCoroutine(assignTrack(MusicTrack.StockGame, true));
        yield return StartCoroutine(assignCustomTrack(true));
#else
        yield return StartCoroutine(loadAsset(path, ""));
        yield return StartCoroutine(extractTracks());
#endif
    }

    private IEnumerator loadAsset(string path, string asset)
    {
        yield return StartCoroutine(LoadAssetFromAssetBundle.Instance.DownloadAssetBundle<AudioClip>(asset, path + "/", "file:///" + Application.dataPath + "/AssetBundles/" + path, -1));
        Debug.Log("Loaded " + asset + " in " + Application.dataPath + "/AssetBundles/" + path);
    }

    private IEnumerator extractTracks()
    {
        AssetBundle assetBundle = LoadAssetFromAssetBundle.Instance.getBundle();
        
        yield return StartCoroutine(assignTrack(assetBundle.LoadAsset<AudioClip>(kothFileName), MusicTrack.KothGame));
        yield return StartCoroutine(assignTrack(assetBundle.LoadAsset<AudioClip>(raceFileName), MusicTrack.RaceGame));
        yield return StartCoroutine(assignTrack(assetBundle.LoadAsset<AudioClip>(stockFileName), MusicTrack.StockGame));
        yield return StartCoroutine(assignTrack(assetBundle.LoadAsset<AudioClip>(menuHypeFileName), MusicTrack.PlayerSelectionMenu));
        yield return StartCoroutine(assignTrack(assetBundle.LoadAsset<AudioClip>(menuBaseFileName), MusicTrack.MainMenu));
        yield return StartCoroutine(assignCustomTrack());

        yield return StartCoroutine(assignTrack(assetBundle.LoadAsset<AudioClip>(kothIntroFileName), MusicTrack.KothGame, true));
        yield return StartCoroutine(assignTrack(assetBundle.LoadAsset<AudioClip>(raceIntroFileName), MusicTrack.RaceGame, true));
        yield return StartCoroutine(assignTrack(assetBundle.LoadAsset<AudioClip>(stockIntroFileName), MusicTrack.StockGame, true));
        yield return StartCoroutine(assignCustomTrack(true));

        LoadAssetFromAssetBundle.Instance.finishedWithBundle();
    }

    private IEnumerator assignTrack(MusicTrack musicTrack)
    {
        yield return StartCoroutine(assignTrack(musicTrack, false));
    }

    private IEnumerator assignTrack(MusicTrack musicTrack, bool intro)
    {
        yield return StartCoroutine(assignTrack(LoadAssetFromAssetBundle.Instance.Obj, musicTrack, intro));
    }

    private IEnumerator assignTrack(Object obj, MusicTrack musicTrack)
    {
        yield return StartCoroutine(assignTrack(LoadAssetFromAssetBundle.Instance.Obj, musicTrack, false));
    }

    private IEnumerator assignTrack (Object obj, MusicTrack musicTrack, bool intro)
    {
        AudioClip backUpClip = new AudioClip();

        MusicManagerBehaviour musicManager = MusicManagerBehaviour.Instance;
        GameObject GO = new GameObject(musicTrack.ToString() + (intro ? "Intro" : ""));
        GO.transform.SetParent(musicManager.transform, false);
        yield return null;

        AudioSource audioSource = GO.AddComponent<AudioSource>();
        audioSource.loop = !intro;

        yield return null;

        AudioMixer musicMixer = Resources.Load("Audio Mixers/MusicMixer") as AudioMixer;

        switch (musicTrack)
        {
            case MusicTrack.KothGame:
                audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Koth")[0];
                if (intro)
                {
                    musicManager.kothMusicIntro = audioSource;
                    backUpClip = kothIntro;
                }
                else
                {
                    musicManager.kothMusic = audioSource;
                    backUpClip = kothMain;
                }
                break;
            case MusicTrack.RaceGame:
                audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Race")[0];
                if (intro)
                {
                    musicManager.raceMusicIntro = audioSource;
                    backUpClip = raceIntro;
                }
                else
                {
                    musicManager.raceMusic = audioSource;
                    backUpClip = raceMain;
                }
                break;
            case MusicTrack.StockGame:
                audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Stock")[0];
                if (intro)
                {
                    musicManager.stockMusicIntro = audioSource;
                    backUpClip = stockIntro;
                }
                else
                {
                    musicManager.stockMusic = audioSource;
                    backUpClip = stockMain;
                }
                break;
            case MusicTrack.PlayerSelectionMenu:
                audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Menu Layer")[0];
                musicManager.playerSelectionMusic = audioSource;
                backUpClip = menuHype;
                break;
            case MusicTrack.MainMenu:
                audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Menu Base")[0];
                musicManager.menuMusic = audioSource;
                backUpClip = menuBase;
                break;
        }

        if (obj != null)
            audioSource.clip = (AudioClip)obj;
        else
            audioSource.clip = backUpClip;

        yield return null;
    }

    private IEnumerator assignCustomTrack()
    {
        yield return StartCoroutine(assignCustomTrack(false));
    }

    private IEnumerator assignCustomTrack(bool intro)
    {
        MusicManagerBehaviour musicManager = MusicManagerBehaviour.Instance;
        GameObject GO = new GameObject("Custom" + (intro ? "Intro" : ""));
        GO.transform.SetParent(musicManager.transform, false);
        yield return null;

        AudioSource audioSource = GO.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = !intro;

        yield return null;

        AudioMixer musicMixer = Resources.Load("Audio Mixers/MusicMixer") as AudioMixer;

        audioSource.outputAudioMixerGroup = musicMixer.FindMatchingGroups("Custom")[0];
        if (intro)
            musicManager.customMusicIntro = audioSource;
        else
            musicManager.customMusic = audioSource;
        yield return null;
    }
}
