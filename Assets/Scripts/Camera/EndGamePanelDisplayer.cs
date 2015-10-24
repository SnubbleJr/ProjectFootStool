using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EndGamePanelDisplayer : MonoBehaviour
{

    //spawns panels, talks to the screen manager, set's their textures to RT's
    //and moves them when told

    public GameObject panelPrefab;

    public SFX finalPanelArrivedAtSound;

    private List<PanelSlider> panels;
    private EndGamePanelManager panelManager;

    void OnEnable()
    {
        PanelSlider.arrivedAtDestination += arrivedAtPos;
        PanelSlider.exited += exited;
    }

    void OnDisable()
    {
        PanelSlider.arrivedAtDestination -= arrivedAtPos;
        PanelSlider.exited -= exited;
    }

    public void buildPanelBoard(EndGamePanelManager manager)
    {
        panelManager = manager;
        panels = new List<PanelSlider>();

        for (int i = 0; i < EndGameScreenManager.Instance.getRTNoCount(); i++)
            panels.Add(makePanel(i));

        animatePanels();
    }

    private PanelSlider makePanel(int id)
    {
        //make panels, then make masks
        GameObject mask = Instantiate(panelPrefab) as GameObject;

        GameObject rawImagePanel = mask.transform.GetChild(0).gameObject;

        mask.transform.SetParent(transform, false);

        Color color = Camera.main.backgroundColor;
        color.a = 1;
        mask.GetComponent<UIImageScript>().setColor(color);

        //assign RT to raw image
        rawImagePanel.GetComponent<UIRawImageScript>().setTexture(EndGameScreenManager.Instance.getRT(id), false);

        //rotate and scale image based on it's id
        float magnitude = 1f / EndGameScreenManager.Instance.getRTNoCount() * (id + 1);
        rawImagePanel.transform.localScale = Vector3.one * magnitude;
        rawImagePanel.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 * (Random.Range(-magnitude, magnitude))));

        //slider values
        PanelSlider panelSlider = mask.GetComponent<PanelSlider>();

        panelSlider.setDestination(mask.transform.localPosition + Vector3.right * 240 * (id));

        mask.transform.localPosition += getDirection() * 75 * (id + 1) * 20;

        panelSlider.setTime(MusicManagerBehaviour.Instance.timeTillNextBeat() + ((60f / MusicManagerBehaviour.Instance.getBPM()) * id));

        panelSlider.setTime(0.3f * (id + 1));
        return panelSlider;
    }

    private Vector3 getDirection()
    {
        //generates a random int vec3
        float dir = Random.Range(0, 5);
        Vector3 vec;

        if (dir > 1)
            if (dir > 2)
                if (dir > 3)
                    if (dir > 4)
                        vec = Vector3.right;
                    else
                        vec = Vector3.right;
                else
                    vec = Vector3.left;
            else
            {
                vec = Vector3.down;
                vec.x = -0.3f;
            }
        else
        {
            vec = Vector3.up;
            vec.x = 0.3f;
        }

        return vec;
    }

    private void animatePanels()
    {
        foreach (PanelSlider panel in panels)
            panel.moveToDestination();
    }

    public void sendOffPanels()
    {
        panels[0].playLeavingSound();
        foreach (PanelSlider panel in panels)
            panel.depart();
    }

    private void arrivedAtPos(PanelSlider panel)
    {
        if (panels[panels.Count - 1].Equals(panel))
            panel.playSound(finalPanelArrivedAtSound);
        else
            panel.playArrivedSound();

        //if last panel is in place
        if (panel == panels[EndGameScreenManager.Instance.getRTNoCount() - 1].GetComponent<PanelSlider>())
        {
            panelManager.lastPanelDisplayed();
            Invoke("sendOffPanels", 0.8f);
        }
    }

    private void exited(PanelSlider panel)
    {
        //if last panel has gone
        if (panel == panels[EndGameScreenManager.Instance.getRTNoCount() - 1].GetComponent<PanelSlider>())
        {
            //remove our panels
            removePanelBoard();

            //tell the manager it's time to move on
            panelManager.removePanels();
        }
    }

    private void removePanelBoard()
    {
        foreach (PanelSlider panel in panels)
            Destroy(panel.gameObject);

        EndGameScreenManager.Instance.resetRTs();
    }
}