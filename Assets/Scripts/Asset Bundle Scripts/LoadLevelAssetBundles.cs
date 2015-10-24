using UnityEngine;
using System.IO;
using System.Collections;

public class LoadLevelAssetBundles : MonoBehaviour {

    public GameObject stockLevel, raceLevel, kothLevel;

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
#if UNITY_EDITOR
#else
        //asign default levels
        yield return StartCoroutine(assignLevel(null, GameMode.Stock));
        yield return StartCoroutine(assignLevel(null, GameMode.Race));
        yield return StartCoroutine(assignLevel(null, GameMode.Koth));
#endif
        string path = "Levels";

        foreach (GameMode gameMode in System.Enum.GetValues(typeof(GameMode)))
            yield return StartCoroutine(loadABsInFolder(path + "/" + System.Enum.GetName(typeof(GameMode), gameMode) + "/", gameMode));
    }

    private IEnumerator loadABsInFolder(string path, GameMode gameMode)
    {
        //scan for all the asset bundles in this folder, and then assign al the elvels in each asset bundle

#if UNITY_EDITOR
#else
        path = Path.Combine("AssetBundles", path);
#endif
        if (Directory.Exists(Directory.GetParent(Path.Combine(Application.dataPath, path)).ToString()))
        {
            string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, path));
            yield return null;

            foreach (string file in files)
            {
                if (Path.GetExtension(file) != ".manifest" && Path.GetExtension(file) != ".meta" && Path.GetExtension(file) != ".txt")
                {
                    yield return StartCoroutine(loadAsset(path + Path.GetFileName(file), gameMode));
#if UNITY_EDITOR
                    if (Path.GetExtension(file) == ".prefab")
                        yield return StartCoroutine(assignLevel(gameMode));
                    else
                        yield return StartCoroutine(extractLevels(gameMode));
#else
                    yield return StartCoroutine(extractLevels(gameMode));
#endif
                }
                yield return null;
            }
        }
        yield return null;
    }

    private IEnumerator loadAsset(string path, GameMode gameMode)
    {
        int version = BundleVersionReader.readVersion(Path.Combine(Application.dataPath, path));
        yield return StartCoroutine(LoadAssetFromAssetBundle.Instance.DownloadAssetBundle<GameObject>("", path, "file:///" + Path.Combine(Application.dataPath, path), version));
    }
    
    private IEnumerator extractLevels(GameMode gameMode)
    {
        //load all the levels from the current AB

        AssetBundle assetBundle = LoadAssetFromAssetBundle.Instance.getBundle();

        yield return null;

        //foreach level in ab
        foreach (string asset in assetBundle.GetAllAssetNames())
        {
            GameObject level = assetBundle.LoadAsset<GameObject>(asset);
            yield return StartCoroutine(assignLevel(level, gameMode));
            yield return null;
        }

        LoadAssetFromAssetBundle.Instance.finishedWithBundle();    
    }

    private IEnumerator assignLevel(GameMode gameMode)
    {
        yield return StartCoroutine(assignLevel(LoadAssetFromAssetBundle.Instance.Obj, gameMode));
    }

    private IEnumerator assignLevel(Object obj, GameMode gameMode)
    {
        GameObject backUpLevel = null;

        CustomLevelParser levelParser = CustomLevelParser.Instance;

        switch (gameMode)
        {
            case GameMode.Stock:
                backUpLevel = stockLevel;
                break;
            case GameMode.Race:
                backUpLevel = raceLevel;
                break;
            case GameMode.Koth:
                backUpLevel = kothLevel;
                break;
        }

        if (obj != null)
            yield return StartCoroutine(levelParser.parseLevel((GameObject)obj));
        else if (backUpLevel != null)
            yield return StartCoroutine(levelParser.parseLevel(backUpLevel));

        yield return null;
    }
}
