using UnityEngine;
using System.Collections;

public class CustomLevelParser : MonoBehaviour {
    //this is attachedd to the level manager and generates a working level based on the shell passed to it

    public GameObject[] defaultLevels;

    //Here is a private reference only this class can access
    private static CustomLevelParser instance;

    //This is the public reference that other classes will use
    public static CustomLevelParser Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<CustomLevelParser>();
            return instance;
        }
    }

    void Awake()
    {
        foreach (GameObject defaultLevel in defaultLevels)
            parseLevel(defaultLevel);
    }

    public GameObject parseObject(GameObject shell)
    {
        //clone our game, to keep name, tag and layer
        GameObject newGO = Instantiate(shell, shell.transform.position, shell.transform.rotation) as GameObject;
        newGO.name = shell.name;

        //remove all shell components of newGO
        foreach (Component componentGO in newGO.GetComponents(typeof(Component)))
        {
            System.Type type = componentGO.GetType();
            if (!( type == typeof(Transform)
                || type == typeof(SpriteRenderer)
                || type == typeof(BoxCollider2D)
                || type == typeof(Rigidbody2D)))
                Destroy(componentGO);
        }

        //then add parsed components back
        foreach (Component shellComponent in shell.GetComponents(typeof(Component)))
            parseComponent(newGO, shellComponent);

        //remove all shell children of newGO
        foreach (Transform childGO in newGO.transform)
            Destroy(childGO.gameObject);
        
        //then add parsed children back
        foreach (Transform shellChild in shell.transform)
        {
            GameObject newChild = parseObject(shellChild.gameObject);
            if (newChild != null)
            {
                newChild.transform.SetParent(newGO.transform, true);
                newChild.transform.localScale = shellChild.transform.localScale;
            }
        }

        return newGO;
    }

    private void parseComponent(GameObject gameObject, Component shellComponent)
    {
        //adds correct component to gameobect if we can find it
        System.Type type = shellComponent.GetType();

        if (type == typeof(ShellLevelBehaviour))
        {
            LevelBehaviour component = gameObject.AddComponent<LevelBehaviour>();
            ShellLevelBehaviour shellComp = shellComponent.GetComponent<ShellLevelBehaviour>();
            component.gameModeType = shellComp.gameModeType;
            component.levelTrack = shellComp.levelTrack;
            component.customTrack = shellComp.customTrack;
            component.levelSize = shellComp.levelSize;
        }
        if (type == typeof(ShellBackgroundBackBehaviour))
        {
            BackgroundBackBehaviour component = gameObject.AddComponent<BackgroundBackBehaviour>();
        }
        if (type == typeof(ShellColorChanger))
        {
            ColorChanger component = gameObject.AddComponent<ColorChanger>();
            ShellColorChanger shellComp = shellComponent.GetComponent<ShellColorChanger>();
            component.changeColor = shellComp.changeColor;
        }
        if (type == typeof(ShellHillActivationScript))
        {
            HillActivationScript component = gameObject.AddComponent<HillActivationScript>();
        }
        if (type == typeof(ShellHillTriggerScript))
        {
            HillTriggerScript component = gameObject.AddComponent<HillTriggerScript>();
            ShellHillTriggerScript shellComp = shellComponent.GetComponent<ShellHillTriggerScript>();
            component.hillEnter = shellComp.hillEnter;
            component.hillExit = shellComp.hillExit;
            component.hillContested = shellComp.hillContested;
        }
        if (type == typeof(ShellHillPulseScript))
        {
            HillPulseScript component = gameObject.AddComponent<HillPulseScript>();
            ShellHillPulseScript shellComp = shellComponent.GetComponent<ShellHillPulseScript>();
            component.alphaThreshold = shellComp.alphaThreshold;
        }
        if (type == typeof(ShellTerrainGeneratorBehaviour))
        {
            TerrainGeneratorBehaviour component = gameObject.AddComponent<TerrainGeneratorBehaviour>();
            ShellTerrainGeneratorBehaviour shellComp = shellComponent.GetComponent<ShellTerrainGeneratorBehaviour>();
            component.preferedPlatform = shellComp.preferedPlatform;
            component.specialPlatforms = shellComp.specialPlatforms;
        }
        if (type == typeof(ShellLevelMoverScript))
        {
            LevelMoverScript component = gameObject.AddComponent<LevelMoverScript>();
        }
        if (type == typeof(ShellKillBoxBehaviour))
        {
            KillBoxBehaviour component = gameObject.AddComponent<KillBoxBehaviour>();
        }
    }

    public void parseLevel(GameObject shell)
    {
        parseObject(shell).transform.SetParent(transform, false);
        Debug.Log("Loaded level " + shell.name);

        LevelManagerBehaviour.Instance.findLevels();
    }

    public void parseLevels(GameObject[] shells)
    {
        foreach (GameObject shell in shells)
            parseLevel(shell);
    }
}
