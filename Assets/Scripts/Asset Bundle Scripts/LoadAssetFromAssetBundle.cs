// C# Example
// Loading an Asset from disk instead of loading from an AssetBundle
// when running in the Editor
using System.Collections;
using UnityEngine;

class LoadAssetFromAssetBundle : MonoBehaviour
{
    public Object Obj;

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
        Obj = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + localURL + asset, typeof(T));
        yield return null;

#else
        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;

        // Start the download
        using(WWW www = WWW.LoadFromCacheOrDownload (url, version)){
            yield return www;
            if (www.error != null)
                Debug.LogError("WWW download:" + www.error);
            AssetBundle assetBundle = www.assetBundle;
            Obj = assetBundle.LoadAsset(asset, typeof(T));
            // Unload the AssetBundles compressed contents to conserve memory
            //bundle.Unload(false);

        } // memory is freed from the web stream (www.Dispose() gets called implicitly)
#endif
    }
}