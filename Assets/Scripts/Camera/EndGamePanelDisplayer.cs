using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EndGamePanelDisplayer : MonoBehaviour {

    //spawns panels, talks to the screen manager, set's their textures to RT's
    //and moves them when told

    public GameObject panelPrefab;

    private List<GameObject> panels;
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
        panels = new List<GameObject>();

        for (int i = 0; i < EndGameScreenManager.Instance.getRTNoCount(); i++)
        {
            panels.Add(makePanel(i));
        }

        animatePanels();
    }

    private GameObject makePanel(int id)
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

        panelSlider.setDestination(mask.transform.localPosition + Vector3.right * 240 * (id % 3));

        mask.transform.localPosition += getDirection() * 75 * (id + 1 % 3) * 20;

        return mask;
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
                vec = Vector3.down;
        else
            vec = Vector3.up;

        return vec;
    }

    private void animatePanels()
    {
        foreach (GameObject panel in panels)
            panel.GetComponent<PanelSlider>().moveToDestination();
    }

    public void sendOffPanels()
    {
        foreach (GameObject panel in panels)
            panel.GetComponent<PanelSlider>().depart();
    }

    private void arrivedAtPos(PanelSlider panel)
    {
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
        if (panel == panels[EndGameScreenManager.Instance.getRTNoCount()-1].GetComponent<PanelSlider>())
        {
            //remove our panels
            removePanelBoard();

            //tell the manager it's time to move on
            panelManager.removePanels();
        }
    }

    private void removePanelBoard()
    {
        foreach(GameObject panel in panels)
            Destroy(panel);

        EndGameScreenManager.Instance.resetRTs();
    }
}
