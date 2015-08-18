using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TipBarBehaviour : MonoBehaviour {

    //doesn't do much, spawns tips and gives out tip text   

    public GameObject tipPrefab;

    private List<TipBehaviour> tips = new List<TipBehaviour>();
    private List<TipBehaviour> activeTips = new List<TipBehaviour>();
    private RectTransform rectTransform;
    private string[] tipText;

    private List<PlayerInputScheme> holdingButtonDown = new List<PlayerInputScheme>();
    private float originalTipSpeed = 0.1f;
    private float increasedTipSpeed = 0.8f;

    //Here is a private reference only this class can access
    private static TipBarBehaviour instance;

    //This is the public reference that other classes will use
    public static TipBarBehaviour Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<TipBarBehaviour>();
            return instance;
        }
    }
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        tipText = LoadTipsFile.Instance.getTips();

        tips.Add(createTip());
    }

    void OnEnable()
    {
        InputManagerBehaviour.buttonPressed += buttonDown;
        InputManagerBehaviour.buttonLetGo += buttonUp;
    }

    void OnDisable()
    {
        InputManagerBehaviour.buttonPressed -= buttonDown;
        InputManagerBehaviour.buttonLetGo -= buttonUp;
    }
    
    private void buttonDown(PlayerInputScheme player, string input, float value)
    {
        //if yellow button held down, then set speed of tips to increase speed
        if (input == player.inputs[PlayerInput.ChangeModeInput].shortName)
        {
            if (!holdingButtonDown.Contains(player))
                holdingButtonDown.Add(player);
            foreach (TipBehaviour tip in tips)
                tip.setSpeed(increasedTipSpeed);
        }
    }

    private void buttonUp(PlayerInputScheme player, string input, float value)
    {
        //if yellow button is let go, then set speed of tips to increase speed
        if (input == player.inputs[PlayerInput.ChangeModeInput].shortName)
        {
            if (holdingButtonDown.Contains(player))
                holdingButtonDown.Remove(player);
            
            //if noone is now holding down, then stop
            if (holdingButtonDown.Count <= 0)
                foreach (TipBehaviour tip in tips)
                    tip.setSpeed(originalTipSpeed);
        }
    }

    private TipBehaviour createTip()
    {
        TipBehaviour tip = (Instantiate(tipPrefab, Vector2.right * rectTransform.rect.width, Quaternion.identity) as GameObject).GetComponent<TipBehaviour>();
        tip.transform.SetParent(transform, false);
        tip.setSpeed((holdingButtonDown.Count > 0) ? increasedTipSpeed : originalTipSpeed); 
        return tip;
    }

    public string getTip()
    {
        int index = (int)Random.Range(0, tipText.Length);
        if (index == tipText.Length)
            index--;

        return tipText[index];
    }

    public void tipEntered(TipBehaviour tip)
    {
        //tip has entered, add it to active list, and make a new tip behind it
        activeTips.Add(tip);

        //if we don't ahve any inactive tips left
        tips.Add(createTip());
    }

    public void tipExited(TipBehaviour tip)
    {
        tips.Remove(tip);
        activeTips.Remove(tip);
        Destroy(tip.gameObject);
    }
}
