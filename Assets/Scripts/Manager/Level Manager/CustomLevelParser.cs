using UnityEngine;
using System.Collections;

public class CustomLevelParser : MonoBehaviour {
    //this is attachedd to the level manager and generates a working level based on the shell passed to it

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

    private GameObject parsedObject;

    public IEnumerator parseObject(GameObject shell)
    {
        //make a new object, and one by one fill it out with the shell blueprint
        GameObject newGO = new GameObject();
        newGO.SetActive(false);
        newGO.name = shell.name;
        newGO.tag = shell.tag;
        newGO.layer = shell.layer;
        newGO.transform.position = shell.transform.position;
        newGO.transform.rotation = shell.transform.rotation;
        newGO.transform.localScale = shell.transform.localScale;
        
        //then add parsed components in
        foreach (Component shellComponent in shell.GetComponents(typeof(Component)))
            StartCoroutine(parseComponent(newGO, shellComponent));

        newGO.SetActive(shell.activeSelf);

        //then add parsed children back
        foreach (Transform shellChild in shell.transform)
        {
            StartCoroutine(parseObject(shellChild.gameObject));

            if (parsedObject != null)
            {
                parsedObject.transform.SetParent(newGO.transform, true);
                parsedObject.SetActive(false);
                parsedObject.transform.localScale = shellChild.transform.localScale;
                parsedObject.transform.localPosition = shellChild.transform.localPosition;
            }
        }

        parsedObject = newGO;
        yield return null;
    }

    private IEnumerator parseComponent(GameObject gameObject, Component shellComponent)
    {
        //adds correct component to gameobect if we can find it
        System.Type type = shellComponent.GetType();

        if (type == typeof(SpriteRenderer))
        {
            SpriteRenderer component = gameObject.AddComponent<SpriteRenderer>();
            SpriteRenderer  shellComp = shellComponent.GetComponent<SpriteRenderer>();
            component.enabled = shellComp.enabled;
            component.sprite = shellComp.sprite;
            component.color = shellComp.color;
        }
        if (type == typeof(BoxCollider2D))
        {
            BoxCollider2D component = gameObject.AddComponent<BoxCollider2D>();
            BoxCollider2D shellComp = shellComponent.GetComponent<BoxCollider2D>();
            component.enabled = shellComp.enabled;
            component.sharedMaterial = shellComp.sharedMaterial;
            component.isTrigger = shellComp.isTrigger;
            component.usedByEffector = shellComp.usedByEffector;
            component.offset = shellComp.offset;
            component.size = shellComp.size;
        }
        if (type == typeof(Rigidbody2D))
        {
            Rigidbody2D component = gameObject.AddComponent<Rigidbody2D>();
            Rigidbody2D shellComp = shellComponent.GetComponent<Rigidbody2D>();
            component.mass = shellComp.mass;
            component.drag = shellComp.drag;
            component.angularDrag = shellComp.angularDrag;
            component.gravityScale = shellComp.gravityScale;
            component.isKinematic = shellComp.isKinematic;
            component.interpolation = shellComp.interpolation;
            component.sleepMode = shellComp.sleepMode;
            component.collisionDetectionMode = shellComp.collisionDetectionMode;
            component.constraints = shellComp.constraints;
        }
        if (type == typeof(ShellLevelBehaviour))
        {
            LevelBehaviour component = gameObject.AddComponent<LevelBehaviour>();
            ShellLevelBehaviour shellComp = shellComponent.GetComponent<ShellLevelBehaviour>();
            component.enabled = shellComp.enabled;
            component.author = shellComp.author;
            component.thumbNail = shellComp.thumbNail;
            component.gameModeType = shellComp.gameModeType;
            component.levelTrack = shellComp.levelTrack;
            component.customTrackIntro = shellComp.customTrackIntro;
            component.customTrack = shellComp.customTrack;
            component.customTrackBPM = shellComp.customTrackBPM;
            component.customSpawnPoints = shellComp.customSpawnPoints;
            component.levelSize = shellComp.levelSize;
            component.customVisuliserColors = shellComp.customVisuliserColors;
            component.visuliserColor1 = shellComp.visuliserColor1;
            component.visuliserColor2 = shellComp.visuliserColor2;
        }
        if (type == typeof(ShellBackgroundBackBehaviour))
        {
            BackgroundBackBehaviour component = gameObject.AddComponent<BackgroundBackBehaviour>();
            ShellBackgroundBackBehaviour shellComp = shellComponent.GetComponent<ShellBackgroundBackBehaviour>();
            component.enabled = shellComp.enabled;
        }
        if (type == typeof(ShellColorChanger))
        {
            ColorChanger component = gameObject.AddComponent<ColorChanger>();
            ShellColorChanger shellComp = shellComponent.GetComponent<ShellColorChanger>();
            component.enabled = shellComp.enabled;
            component.changeColor = shellComp.changeColor;
        }
        if (type == typeof(ShellHillActivationScript))
        {
            HillActivationScript component = gameObject.AddComponent<HillActivationScript>();
            ShellHillActivationScript shellComp = shellComponent.GetComponent<ShellHillActivationScript>();
            component.enabled = shellComp.enabled;
        }
        if (type == typeof(ShellHillTriggerScript))
        {
            HillTriggerScript component = gameObject.AddComponent<HillTriggerScript>();
            ShellHillTriggerScript shellComp = shellComponent.GetComponent<ShellHillTriggerScript>();
            component.enabled = shellComp.enabled;
            component.hillEnter = shellComp.hillEnter;
            component.hillExit = shellComp.hillExit;
            component.hillContested = shellComp.hillContested;
        }
        if (type == typeof(ShellHillPulseScript))
        {
            HillPulseScript component = gameObject.AddComponent<HillPulseScript>();
            ShellHillPulseScript shellComp = shellComponent.GetComponent<ShellHillPulseScript>();
            component.enabled = shellComp.enabled;
            component.alphaThreshold = shellComp.alphaThreshold;
        }
        if (type == typeof(ShellTerrainGeneratorBehaviour))
        {
            TerrainGeneratorBehaviour component = gameObject.AddComponent<TerrainGeneratorBehaviour>();
            ShellTerrainGeneratorBehaviour shellComp = shellComponent.GetComponent<ShellTerrainGeneratorBehaviour>();
            component.enabled = shellComp.enabled;
            component.preferedPlatform = shellComp.preferedPlatform;
            component.specialPlatforms = shellComp.specialPlatforms;
        }
        if (type == typeof(ShellLevelMoverScript))
        {
            LevelMoverScript component = gameObject.AddComponent<LevelMoverScript>();
            ShellLevelMoverScript shellComp = shellComponent.GetComponent<ShellLevelMoverScript>();
            component.enabled = shellComp.enabled;
        }
        if (type == typeof(ShellKillBoxBehaviour))
        {
            KillBoxBehaviour component = gameObject.AddComponent<KillBoxBehaviour>();
            ShellKillBoxBehaviour shellComp = shellComponent.GetComponent<ShellKillBoxBehaviour>();
            component.enabled = shellComp.enabled;
        }
        if (type == typeof(ShellSpriteRendererParamiters))
        {
            SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
            ShellSpriteRendererParamiters shellComp = shellComponent.GetComponent<ShellSpriteRendererParamiters>();
            component.enabled = shellComp.enabled;
            component.sortingLayerName = shellComp.sortingLayerName;
            component.sortingOrder = shellComp.sortingOrder;
        }
        yield return null;
    }

    public IEnumerator parseLevel(GameObject shell)
    {
        yield return StartCoroutine(parseObject(shell));
        parsedObject.transform.SetParent(transform, false);
        
        parsedObject.SetActive(true);
        
        LevelManagerBehaviour.Instance.findLevels();

        yield return null;
    }

    public IEnumerator parseLevels(GameObject[] shells)
    {
        foreach (GameObject shell in shells)
            yield return StartCoroutine(parseLevel(shell));
    }

    public GameObject getParsedObject()
    {
        return parsedObject;
    }
}
