using UnityEngine;
using System.Collections;
using System.IO;
public class BundleVersionReader : MonoBehaviour
{
    public static int readVersion(string assetBundle)
    {
        //if text doesn't exsit, return -1
        int version = -1;
        if (File.Exists(assetBundle + ".txt"))
            int.TryParse(File.ReadAllText(assetBundle + ".txt"), out version);
        return version;
    }
}
