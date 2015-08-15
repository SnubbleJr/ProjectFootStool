using UnityEngine;
using System.Collections;

public enum StatsPanelType
{
    Long,
    ShortHi,
    ShortLo
}

[RequireComponent(typeof(UIImageScript))]

public class PlayerStatsPanel : MonoBehaviour {

    public StatsPanelType panelType;
    public SFX readySound;
    public UITextScript playerText, KOText, TKOText, SDText;
    public UIImageScript playerImage, bulliedImage, bulliedByImage;
    public UITextScript readyText;

    private PulseInwardsScript pulseScript;

    private UIImageScript displayImageScript;
    private Sprite defaultImage;

    private PlayerControl player;
    private PlayerControl bulliedControl, bulliedByControl;
    private PlayerStats stats;

    private bool readyd = false;

    void Awake()
    {
        displayImageScript = GetComponent<UIImageScript>();
        defaultImage = displayImageScript.getSprite();
        pulseScript = GetComponent<PulseInwardsScript>();
    }

    private void displayStats()
    {
        playerText.gameObject.SetActive(true);
        KOText.gameObject.SetActive(true);
        TKOText.gameObject.SetActive(true);
        SDText.gameObject.SetActive(true);

        playerImage.gameObject.SetActive(true);

        if (panelType == StatsPanelType.Long)
        {
            bulliedImage.gameObject.SetActive(true);
            bulliedByImage.gameObject.SetActive(true);
        }
    }

    private void stopDisplayStats()
    {
        playerText.gameObject.SetActive(false);
        KOText.gameObject.SetActive(false);
        TKOText.gameObject.SetActive(false);
        SDText.gameObject.SetActive(false);

        playerImage.gameObject.SetActive(false);

        if (panelType == StatsPanelType.Long)
        {
            bulliedImage.gameObject.SetActive(false);
            bulliedByImage.gameObject.SetActive(false);
        }
    }

    private void showReady()
    {
        SFXManagerBehaviour.Instance.playSound(readySound);
        readyText.gameObject.SetActive(true);
        //pulse downwards (if we have arrived)
        if (!GetComponent<PanelSlider>().enabled)
            pulseScript.pulse();
    }

    private void hideReady()
    {
        readyText.gameObject.SetActive(false);
    }

    private void updateReady()
    {
        //show ready indicatoor is readyd
        if (readyd)
        {
            stopDisplayStats();
            showReady();
        }
        else
        {
            hideReady();
            displayStats();
        }
    }

    public void setStats(PlayerControl control, PlayerStats stat, PlayerControl bullid, PlayerControl bullidBy)
    {
        player = control;
        stats = stat;
        bulliedControl = bullid;
        bulliedByControl = bullidBy;

        playerText.setText(stats.id.ToString());
        playerText.setColor(player.getColor());
        KOText.setText(stats.getKOs().Count.ToString());
        TKOText.setText(stats.getTKOs().Count.ToString());
        SDText.setText(stats.getSDs().ToString());

        playerImage.setSprite(player.getSprite());
        playerImage.setColor(player.getColor());

        if (panelType == StatsPanelType.Long)
        {
            if (!player.Equals(bulliedControl))
            {
                bulliedImage.setSprite(bulliedControl.getSprite());
                bulliedImage.setColor(bulliedControl.getColor());
            }

            if (!player.Equals(bulliedByControl))
            {
                bulliedByImage.setSprite(bulliedByControl.getSprite());
                bulliedByImage.setColor(bulliedByControl.getColor());
            }
        }

        displayStats();
    }

    public void clearStats()
    {
        //clear the whole panel, set ready to false
        //and don't display anything
        readyd = false;
        hideReady();
        stopDisplayStats();
    }

    public void setReady(bool val)
    {
        if (readyd != val)
        {
            readyd = val;
            updateReady();
        }
    }

    public void moveToDestination()
    {
        PanelSlider panelSlider = GetComponent<PanelSlider>();
        panelSlider.moveToDestination();
    }

    public void sendOff()
    {
        pulseScript.setMovement(false);
        PanelSlider panelSlider = GetComponent<PanelSlider>();
        panelSlider.depart();
    }
}
