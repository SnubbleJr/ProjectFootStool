using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LoadSettingsFile: MonoBehaviour {

    private const string path = "/Files/settings.txt";

    private static Dictionary<Setting, float> settings = new Dictionary<Setting, float>();

    //Here is a private reference only this class can access
    private static LoadSettingsFile instance;

    //This is the public reference that other classes will use
    public static LoadSettingsFile Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<LoadSettingsFile>();
            return instance;
        }
    }

    void OnEnable()
    {
        MasterVolumeSliderElement.volumeChanged += masterVolChanged;
        MusicVolumeSliderElement.volumeChanged += musicVolChanged;
        SFXVolumeSliderElement.volumeChanged += sfxVolChanged;
    }

    void OnDisable()
    {
        MasterVolumeSliderElement.volumeChanged -= masterVolChanged;
        MusicVolumeSliderElement.volumeChanged -= musicVolChanged;
        SFXVolumeSliderElement.volumeChanged -= sfxVolChanged;
    }

    public IEnumerator loadSettingsFile()
    {
        yield return StartCoroutine(loadtSettings());
    }

    private IEnumerator loadtSettings()
    {
        foreach (Setting setting in Enum.GetValues(typeof(Setting)))
            settings.Add(setting, 1);

        //try and read
        try
        {
            //populate settings from the file
            string[] settingsInFile = File.ReadAllLines(Application.dataPath + path);
            float[] valuesInFile = new float[settingsInFile.Length];

            for (int i = 0; i < settingsInFile.Length; i++)
                float.TryParse(settingsInFile[i], out valuesInFile[i]);

            foreach (Setting setting in Enum.GetValues(typeof(Setting)))
                settings[setting] = valuesInFile[(int)setting];
        }
        //assing
        catch
        {
            Debug.Log("Could not find " + Application.dataPath + path);
        }

        applySettings();
        saveSettings();
        yield return null;
    }

    private void masterVolChanged()
    {
        settings[Setting.MasterVol] = MasterVolumeSliderElement.volume;
        saveSettings();
    }
    
    private void musicVolChanged()
    {
        settings[Setting.MusicVol] = MusicVolumeSliderElement.volume;
        saveSettings();
    }

    private void sfxVolChanged()
    {
        settings[Setting.SFXVol] = SFXVolumeSliderElement.volume;
        saveSettings();
    }

    private void saveSettings()
    {
        //write settings to a file,
        string[] settingsInFile = new string[Enum.GetValues(typeof(Setting)).Length];

        foreach (Setting setting in Enum.GetValues(typeof(Setting)))
            settingsInFile[(int)setting] = settings[setting].ToString();

        File.WriteAllLines(Application.dataPath + path, settingsInFile);
        Debug.Log("Wrote to " + Application.dataPath + path);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    private void applySettings()
    {
        MasterVolumeSliderElement.volume = settings[Setting.MasterVol];
        MusicVolumeSliderElement.volume = settings[Setting.MusicVol];
        SFXVolumeSliderElement.volume = settings[Setting.SFXVol];
    }

    public Dictionary<Setting, float> getSettings()
    {
        return settings;
    }
}
