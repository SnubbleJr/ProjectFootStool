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
    }

    //when the gameobject this is attached to is activated, we do this 
    public void goTime()
    {
        if (mainMenu != null)
        {
            Invoke("activateMenu", time);

            //disable us
            this.transform.parent.gameObject.SetActive(false);
        }
    }

    private void activateMenu()
    {
        //find and enable mainmenu
        mainMenu.SetActive(true);

        //play menu music
        musicComponent.playMusic();
    }
}
