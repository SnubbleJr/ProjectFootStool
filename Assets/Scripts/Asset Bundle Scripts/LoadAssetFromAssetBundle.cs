// C# Example
// Loading an Asset from disk instead of loading from an AssetBundle
// when running in the Editor
using System.Collections;
using UnityEngine;

class LoadAssetFromAssetBundle : MonoBehaviour
{
    public Object Obj;

    private AssetBundle assetBundle;

    //Here is a private reference only this class can access
    private static LoadAssetFromAssetBundle instance;

    //This is the public reference that other classes will use
    public static LoadAssetFromAssetBundle Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<LoadAssetFromAssetBundle>();
            return instance;
        }
    }

    public IEnumerator DownloadAssetBundle<T>(string asset, string localURL, string url, int version) where T : Object {
        Obj = null;

#if UNITY_EDITOR
        //if version is -1, then it's a default asset
        if (version < 0)
        {
            Obj = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + localURL + asset, typeof(T));
            yield return null;
        }
        else
        {
            // Wait for the Caching system to be ready
            while (!Caching.ready)
                yield return null;

            // Start the download
            using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
            {
                yield return www;
                if (www.error != null)
                    Debug.LogError("WWW download:" + www.error);
                assetBundle = www.assetBundle;

            } // memory is freed from the web stream (www.Dispose() gets called implicitly)
        }
#else
        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;

        // Start the download
        using(WWW www = WWW.LoadFromCacheOrDownload (url, version)){
            yield return www;
            if (www.error != null)
                Debug.LogError("WWW download:" + www.error);
            assetBundle = www.assetBundle;

        } // memory is freed from the web stream (www.Dispose() gets called implicitly)
#endif
    }

    public AssetBundle getBundle()
    {
        if (assetBundle != null)
            return assetBundle;
        else
            return null;
    }

    public void finishedWithBundle()
    {
        assetBundle.Unload(false);
    }
}