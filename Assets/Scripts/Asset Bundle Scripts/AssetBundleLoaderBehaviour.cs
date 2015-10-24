using UnityEngine;
using System.Collections;

public class AssetBundleLoaderBehaviour : MonoBehaviour {

    private PanelSlider loadingloop;
    private UITextScript loadingTextScript;
    private UIImageScript loadingImage;
    private bool fadeLoop = false;

    public static bool musicReady = false;
    public static bool settingReady = false;
    public static bool levelsReady = false;
    public static bool statsReady = false;
    public static bool tipsReady = false;

    public delegate void AssetBundleLoadedDelegate();
    public static event AssetBundleLoadedDelegate musicLoaded;
    public static event AssetBundleLoadedDelegate statsLoaded;
    public static event AssetBundleLoadedDelegate settingsLoaded;
    public static event AssetBundleLoadedDelegate tipsLoaded;
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
	void OnEnable ()
    {
        StartCoroutine(startLoading());
	}

    void OnDisable ()
    {
        musicReady = false;
        settingReady = false;
        levelsReady = false;
        statsReady = false;
        tipsReady = false;

        loadingloop.gameObject.SetActive(true);
        loadingImage.setColor(Color.white);
    }

    private IEnumerator startLoading()
    {
        yield return StartCoroutine(createLoadingLoop());

        yield return StartCoroutine(loadAsset(LoadMusicAssetBundles.Instance.loadMusicABs(), musicLoaded));
        musicReady = true;
        yield return StartCoroutine(loadAsset(LoadSettingsFile.Instance.loadSettingsFile(), settingsLoaded));
        settingReady = true;
        yield return StartCoroutine(loadAsset(LoadSettingsFile.Instance.loadSettingsFile(), statsLoaded));
        statsReady = true;
        yield return StartCoroutine(loadAsset(LoadTipsFile.Instance.loadTipsFile(), tipsLoaded));
        tipsReady = true;
        yield return StartCoroutine(loadAsset(LoadLevelAssetBundles.Instance.loadLevelABs(), levelsLoaded));
        levelsReady = true;

        yield return StartCoroutine(setLoadingLoop());

        if (allLoaded != null)
            allLoaded();
    }

    private IEnumerator createLoadingLoop()
    {
        loadingloop = GetComponentInChildren<PanelSlider>();
        loadingTextScript = loadingloop.GetComponentInChildren<UITextScript>();
        loadingImage = transform.GetComponentInChildren<UIImageScript>();

        loadingloop.setDestination(new Vector2(Camera.main.pixelWidth / 5, -Camera.main.pixelHeight / 11));
        loadingloop.setSpeed(1000);
        loadingloop.moveToDestination();

        yield return null;
    }

    private IEnumerator setLoadingLoop()
    {
        string text = "";
        if (!getAllLoaded())
        {
            if (!musicReady)
                text = "Loading Music...";
            else if (!settingReady)
                text = "Loading Settings...";
            else if (!statsReady)
                text = "Loading Stats...";
            else if (!tipsReady)
                text = "Loading tips...";
            else if (!levelsReady)
                text = "Loading Levels...";
        }
        else
        {
            text = "Have fun...";
            stopLoadingLoop();
        }

        loadingTextScript.setText(text);

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
            fadeLoop = true;
    }

    void Update()
    {
        if (loadingloop && fadeLoop)
            fadeLoadingLoop();
    }

    private void fadeLoadingLoop()
    {
        Color color = loadingImage.getColor();
        color = Color.Lerp(color, Color.clear, 5 * Time.deltaTime);
        if (color.a < 0.1f)
        {
            loadingloop.gameObject.SetActive(false);
            fadeLoop = false;
        }
        else
            loadingImage.setColor(color);
    }

    private IEnumerator loadAsset(IEnumerator coroutine, AssetBundleLoadedDelegate loadedDelegate)
    {
        yield return StartCoroutine(setLoadingLoop());
        yield return StartCoroutine(coroutine);
        if (loadedDelegate != null)
            loadedDelegate();
    }

    public bool getAllLoaded()
    {
        return (musicReady && settingReady && levelsReady && statsReady && tipsReady);
    }
}
