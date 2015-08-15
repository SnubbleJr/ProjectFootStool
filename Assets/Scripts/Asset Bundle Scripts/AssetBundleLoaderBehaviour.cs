using UnityEngine;
using System.Collections;

public class AssetBundleLoaderBehaviour : MonoBehaviour {

    private PanelSlider loadingloop;
    private UITextScript loadingTextScript;

    public static bool musicReady = false;
    public static bool settingReady = false;
    public static bool levelReady = false;
    public static bool statReady = false;

    public delegate void AssetBundleLoadedDelegate();
    public static event AssetBundleLoadedDelegate musicLoaded;
    public static event AssetBundleLoadedDelegate statLoaded;
    public static event AssetBundleLoadedDelegate settingsLoaded;
    public static event AssetBundleLoadedDelegate levelsLoaded;
    public static event AssetBundleLoadedDelegate allLoaded;

    //Here is a private reference only this class can access
    private static AssetBundleLoaderBehaviour instance;

    //This is the public reference that other classes will use
    public static AssetBundleLoaderBehaviour Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<AssetBundleLoaderBehaviour>();
            return instance;
        }
    }

	// Use this for initialization
	void Awake ()
    {
        StartCoroutine(startLoading());
	}

    private IEnumerator startLoading()
    {
        yield return StartCoroutine(createLoadingLoop());
        yield return StartCoroutine(loadMusic());
        yield return StartCoroutine(loadSettings());
        yield return StartCoroutine(loadStats());
        //yield return StartCoroutine(loadLevels());
    }

    private IEnumerator createLoadingLoop()
    {
        loadingloop = GetComponentInChildren<PanelSlider>();
        loadingTextScript = loadingloop.GetComponentInChildren<UITextScript>();

        loadingloop.setDestination(Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 0)));
        loadingloop.setSpeed(1000);
        loadingloop.moveToDestination();

        yield return null;
    }

    private IEnumerator setLoadingLoop()
    {
        if (!getAllLoaded())
        {
            string text = "";
            if (!musicReady)
                text = "Downloading Music...";
            else if (!settingReady)
                    text = "Downloading Settings...";
                else if (!statReady)
                        text = "Downloading Stats...";
                    else if (!levelReady)
                            text = "Downloading Levels...";

            loadingTextScript.setText(text);
            resetLoadingLoop();
        }
        else
            stopLoadingLoop();

        yield return null;
    }

    private void resetLoadingLoop()
    {
        //reset the loop pos
        if (loadingloop)
        {
            loadingloop.backToStart();
            loadingloop.moveToDestination();
        }
    }

    private void stopLoadingLoop()
    {
        //stop loop from appearing now
        if (loadingloop)
        {
            loadingloop.backToStart();
            loadingloop.gameObject.SetActive(false);
        }  
    }

    private IEnumerator loadMusic()
    {
        yield return StartCoroutine(setLoadingLoop());
        //wait for music to load up
        yield return StartCoroutine(LoadMusicAssetBundles.Instance.loadMusicABs());
        musicReady = true;
        if (musicLoaded != null)
            musicLoaded();
    }

    private IEnumerator loadSettings()
    {
        yield return StartCoroutine(setLoadingLoop());
        //wait for music to load up
        yield return StartCoroutine(LoadSettingsFile.Instance.loadSettingsFile());
        settingReady = true;
        if (settingsLoaded != null)
            settingsLoaded();
    }

    private IEnumerator loadStats()
    {
        yield return StartCoroutine(setLoadingLoop());
        //wait for music to load up
        yield return StartCoroutine(LoadStatsFile.Instance.loadStatsFile());
        statReady = true;
        if (statLoaded != null)
            statLoaded();
    }

    private IEnumerator loadLevels()
    {
        yield return StartCoroutine(setLoadingLoop());
        //wait for music to load up
        yield return StartCoroutine(LoadLevelAssetBundles.Instance.loadLevelABs());
        levelReady = true;
        if (levelsLoaded != null)
            levelsLoaded();
    }

    public bool getAllLoaded()
    {
        return (musicReady && settingReady && levelReady && statReady);
    }
}
