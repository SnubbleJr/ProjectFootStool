using UnityEngine;
using System.Collections;

public class TitleScreentoMenu : MonoBehaviour {

    public GameObject mainMenu;
    public float time = 1f;

    private MusicComponent musicComponent;

    void OnEnable()
    {
        //stop music
        musicComponent = GetComponent<MusicComponent>();
        musicComponent.stopMusic();

        InputManagerBehaviour.buttonPressed += buttonDetected;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonDetected;
    }

    void buttonDetected(PlayerInputScheme player, string input, float value)
    {
        //cheeky overide for this specail case
        if (input == player.inputs[PlayerInput.StartGameInput].shortName)
            goTime();
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        if (mainMenu != null)
        {
            //check to see if we have loaded everything in
            if (AssetBundleLoaderBehaviour.Instance.getAllLoaded())
            {

                Invoke("activateMenu", time);

                //disable us
                this.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    private void activateMenu()
    {
        //find and enable mainmenu
        mainMenu.SetActive(true);

        //play menu music
        musicComponent.playMusic(MusicTrack.MainMenu);
    }
}
