using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;

public class LoadTipsFile : MonoBehaviour {

    public TextAsset defaultTips;

    private const string path = "/Files/tips.txt";

    private string[] tips;

    //Here is a private reference only this class can access
    private static LoadTipsFile instance;

    //This is the public reference that other classes will use
    public static LoadTipsFile Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<LoadTipsFile>();
            return instance;
        }
    }

    public IEnumerator loadTipsFile()
    {
        yield return StartCoroutine(loadTips());
    }

    private IEnumerator loadTips()
    {
        //try and read
        try
        {
            //populate settings from the file
            tips = File.ReadAllLines(Application.dataPath + path);
        }
        //assing
        catch
        {
            Debug.Log("Could not find " + Application.dataPath + path);
            tips = defaultTips.text.Split('\n');
        }
        yield return null;
    }

    public string[] getTips()
    {
        return tips;
    }
}
