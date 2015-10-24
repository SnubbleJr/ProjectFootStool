using UnityEngine;
using System.Collections;

public class IntroSceneBehaviour : MonoBehaviour {

    //shows splash screens, and loads match int he background

    public GameObject firstIntro, secondIntro;

    private GameObject intro1, intro2;

    private bool startedLoad = false;
    private bool intro1Finished = false;
    private bool intro2Finished = false;

    private AsyncOperation async;
    
    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonPressed;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonPressed;
    }

    void Awake()
    {
        loadIntro1();
    }

    private void loadIntro1()
    {
        intro1 = Instantiate(firstIntro);
        intro1.transform.SetParent(transform, false);
    }

    private void loadIntro2()
    {
        intro1Finished = true;
        Destroy(intro1);
        intro2 = Instantiate(secondIntro);
        intro2.transform.SetParent(transform, false);
    }

    private void skipIntro()
    {
        if (!intro1Finished)
            loadIntro2();
        else if (!intro2Finished && async.progress >= 0.9f)
            startLevel();
    }

    private void buttonPressed(PlayerInputScheme player, string input, float value)
    {
        if (input == player.inputs[PlayerInput.StartGameInput].shortName)
            skipIntro();
    }

    void Update()
    {
        if (!startedLoad)
            startLoading();
    }

    private void startLevel()
    {
        intro2Finished = true;
        Destroy(intro2);
        async.allowSceneActivation = true;
	}

    private void startLoading()
    {
        startedLoad = true;
        StartCoroutine(loadLevel());
    }

    private IEnumerator loadLevel()
    {
        async = Application.LoadLevelAsync(1);
        async.allowSceneActivation = false;
        yield return async;
    }

    public void introFinished()
    {
        skipIntro();
    }
}
