using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum InputType
{
    Keyboard,
    ControllerFull,
    ControllerHalfL,
    ControllerHalfR
}

public enum PlayerInput
{
    HorizontalInput,
    VerticalInput,
    UpInput,
    DownInputTrigger,
    DashInputTrigger,
    DownInputButton,
    DashInputButton,
    LeftInput,
    RightInput,
    SubmitInput,
    CancelInput,
    ChangeModeInput,
    PauseInput,
    SplitControlInput1,
    SplitControlInput2,
    WiggleHorizontalInput,
    WiggleVerticalInput,
    StartGameInput
}

public class PlayerInputScheme
{
    public int id;
    public int controller;
    public InputType inputType;
    public Dictionary<PlayerInput, InputAxisAndButtons> inputs;
}

public class InputAxisAndButtons
{
    public bool axis;

    //used for special cases where we want to detect axis being pressed down
    public string inputName;
    public string shortName;        //used when for when we want to use delegates
    public bool inUse;
}

public class InputLog
{
    public PlayerInputScheme InputtedPlayer;
    public string input;
    public float value;
}

public class InputManagerBehaviour : MonoBehaviour {

    //Here is a private reference only this class can access
    private static InputManagerBehaviour instance;

    //This is the public reference that other classes will use
    public static InputManagerBehaviour Instance
    {
        get
        {
            //If instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (instance == null)
                instance = GameObject.FindObjectOfType<InputManagerBehaviour>();
            return instance;
        }
    }

    //scans through player inputs and shoots off delegate when input is read

    public delegate void InputChangedDelegate();
    public static event InputChangedDelegate inputAdded;
    public static event InputChangedDelegate inputRemoved;

    public delegate void InputManagerDelegate(PlayerInputScheme inputtingPlayer, string input, float value);
    public static event InputManagerDelegate axisPressed;
    public static event InputManagerDelegate buttonPressed;
    public static event InputManagerDelegate buttonLetGo;

    public delegate void OnePlayerControllerDelegate(PlayerInputScheme player);
    public static event OnePlayerControllerDelegate playerRemoved;
    public static event OnePlayerControllerDelegate playerAdded;

    private List<PlayerInputScheme> players = new List<PlayerInputScheme>();

    List<InputLog> loggedButtonsDown = new List<InputLog>();
    List<InputLog> loggedButtonsUp = new List<InputLog>();
    List<string> loggedInputStrings = new List<string>();

    private List<string> currentControllers = new List<string>();
    private const int keyboardPlayers = 4;

	// Use this for initialization
	void Awake ()
    {
        buildPlayerInputKeyboards(keyboardPlayers);
        detectControllerChange();
	}

    private void buildPlayerInputKeyboards(int size)
    {
        for (int i=1; i <=size; i++)
        {
            PlayerInputScheme player = buildPlayerInputKeyboard(i);
            players.Add(player);

            if (playerAdded != null)
                playerAdded(player);
        }
    }

    private PlayerInputScheme buildPlayerInputKeyboard(int i)
    {
        string inputString = "Keyboard " + i.ToString();

        return new PlayerInputScheme
        {
            id = players.Count + 1,
            controller = i - keyboardPlayers,
            inputType = InputType.Keyboard,
            inputs = new Dictionary<PlayerInput, InputAxisAndButtons>
                {
                    {PlayerInput.HorizontalInput,   new InputAxisAndButtons {axis = true, inputName = inputString + " Horizontal", shortName = "Horizontal", inUse = false}}, 
                    {PlayerInput.VerticalInput,     new InputAxisAndButtons {axis = true, inputName = inputString + " Vertical", shortName = "Vertical", inUse = false}},
                
                    {PlayerInput.LeftInput,         new InputAxisAndButtons {axis = false, inputName = inputString + " Left", shortName = "Left", inUse = false}},
                    {PlayerInput.RightInput,        new InputAxisAndButtons {axis = false, inputName = inputString + " Right", shortName = "Right", inUse = false}}, 
                    {PlayerInput.UpInput,           new InputAxisAndButtons {axis = false, inputName = inputString + " Jump", shortName = "Jump", inUse = false}}, 
                    {PlayerInput.DownInputButton,   new InputAxisAndButtons {axis = false, inputName = inputString + " Flump", shortName = "Flump", inUse = false}},
                    {PlayerInput.DashInputButton,   new InputAxisAndButtons {axis = false, inputName = inputString + " Dash", shortName = "Dash", inUse = false}},
                    {PlayerInput.SubmitInput,       new InputAxisAndButtons {axis = false, inputName = inputString + " Submit", shortName = "Submit", inUse = false}},
                    {PlayerInput.CancelInput,       new InputAxisAndButtons {axis = false, inputName = inputString + " Cancel", shortName = "Cancel", inUse = false}},
                    {PlayerInput.ChangeModeInput,   new InputAxisAndButtons {axis = false, inputName = inputString + " ChangeMode", shortName = "ChangeMode", inUse = false}},
                    {PlayerInput.PauseInput,        new InputAxisAndButtons {axis = false, inputName = inputString + " Pause", shortName = "Pause", inUse = false}},
                    {PlayerInput.StartGameInput,    new InputAxisAndButtons {axis = false, inputName = inputString + " StartGame", shortName = "StartGame", inUse = false}}
                }
        };
    }

    private PlayerInputScheme buildPlayerInputControllerFull(int controllerNo)
    {
        //we don't need idNo as it will just be the controllerNo + number of keyboard players

        string inputString = "Controller " + controllerNo.ToString();
        
        return new PlayerInputScheme
        {
            id = (keyboardPlayers + 1) + (2 * (controllerNo - 1)),
            controller = controllerNo,
            inputType = InputType.ControllerFull,
            inputs = new Dictionary<PlayerInput, InputAxisAndButtons>
            {
                
                {PlayerInput.DownInputTrigger,      new InputAxisAndButtons {axis = true, inputName = inputString + " FlumpTrigger", shortName = "Flump", inUse = false}},
                {PlayerInput.DashInputTrigger,      new InputAxisAndButtons {axis = true, inputName = inputString + " DashTrigger", shortName = "Dash", inUse = false}},

                {PlayerInput.HorizontalInput,       new InputAxisAndButtons {axis = true,inputName = inputString + " Horizontal", shortName = "Horizontal", inUse = false}}, 
                {PlayerInput.VerticalInput,         new InputAxisAndButtons {axis = true,inputName = inputString + " Vertical", shortName = "Vertical", inUse = false}},
                {PlayerInput.WiggleHorizontalInput, new InputAxisAndButtons {axis = true,inputName = inputString + " WiggleHorizontal", shortName = "WiggleHorizontal", inUse = false}}, 
                {PlayerInput.WiggleVerticalInput,   new InputAxisAndButtons {axis = true,inputName = inputString + " WiggleVertical", shortName = "WiggleVertical", inUse = false}},
            
                {PlayerInput.UpInput,               new InputAxisAndButtons {axis = false, inputName = inputString + " Jump", shortName = "Jump", inUse = false}}, 
                {PlayerInput.DownInputButton,       new InputAxisAndButtons {axis = false, inputName = inputString + " FlumpButton", shortName = "Flump", inUse = false}},
                {PlayerInput.DashInputButton,       new InputAxisAndButtons {axis = false, inputName = inputString + " DashButton", shortName = "Dash", inUse = false}},
                {PlayerInput.SubmitInput,           new InputAxisAndButtons {axis = false, inputName = inputString + " Submit", shortName = "Submit", inUse = false}},
                {PlayerInput.CancelInput,           new InputAxisAndButtons {axis = false, inputName = inputString + " Cancel", shortName = "Cancel", inUse = false}},
                {PlayerInput.ChangeModeInput,       new InputAxisAndButtons {axis = false, inputName = inputString + " ChangeMode", shortName = "ChangeMode", inUse = false}},
                {PlayerInput.PauseInput,            new InputAxisAndButtons {axis = false, inputName = inputString + " Pause", shortName = "Pause", inUse = false}},
                {PlayerInput.SplitControlInput1,    new InputAxisAndButtons {axis = false, inputName = inputString + " SplitControl1", shortName = "SplitControl1", inUse = false}},
                {PlayerInput.SplitControlInput2,    new InputAxisAndButtons {axis = false, inputName = inputString + " SplitControl2", shortName = "SplitControl2", inUse = false}},
                {PlayerInput.StartGameInput,        new InputAxisAndButtons {axis = false, inputName = inputString + " StartGame", shortName = "StartGame", inUse = false}}
            }
        };
    }

    private PlayerInputScheme buildPlayerInputControllerHalf(int controllerNo, int idNo, bool left)
    {
        string inputString = "Controller " + controllerNo.ToString() + ((left) ? " left" : " right") + " side";

        return new PlayerInputScheme
        {
            id = idNo,
            controller = controllerNo,
            inputType = (left) ? InputType.ControllerHalfL : InputType.ControllerHalfR,
            inputs = new Dictionary<PlayerInput, InputAxisAndButtons>
            {                
                {PlayerInput.DownInputTrigger,      new InputAxisAndButtons {axis = true, inputName = inputString + " FlumpTrigger", shortName = "Flump", inUse = false}},
                {PlayerInput.DashInputTrigger,      new InputAxisAndButtons {axis = true, inputName = inputString + " DashTrigger", shortName = "Dash", inUse = false}},

                {PlayerInput.HorizontalInput,       new InputAxisAndButtons {axis = true,inputName = inputString + " Horizontal", shortName = "Horizontal", inUse = false}},
                {PlayerInput.VerticalInput,         new InputAxisAndButtons {axis = true,inputName = inputString + " Vertical", shortName = "Vertical", inUse = false}},
                {PlayerInput.CancelInput,           new InputAxisAndButtons {axis = true,inputName = inputString + " Cancel", shortName = "Cancel", inUse = false}},
            
                {PlayerInput.UpInput,               new InputAxisAndButtons {axis = false, inputName = inputString + " Jump", shortName = "Jump", inUse = false}}, 
                {PlayerInput.DownInputButton,       new InputAxisAndButtons {axis = false, inputName = inputString + " FlumpButton", shortName = "Flump", inUse = false}},
                {PlayerInput.DashInputButton,       new InputAxisAndButtons {axis = false, inputName = inputString + " DashButton", shortName = "Dash", inUse = false}},
                {PlayerInput.SubmitInput,           new InputAxisAndButtons {axis = false, inputName = inputString + " Submit", shortName = "Submit", inUse = false}},
                {PlayerInput.ChangeModeInput,       new InputAxisAndButtons {axis = false, inputName = inputString + " ChangeMode", shortName = "ChangeMode", inUse = false}},
                {PlayerInput.PauseInput,            new InputAxisAndButtons {axis = false, inputName = inputString + " Pause", shortName = "Pause", inUse = false}},
                {PlayerInput.SplitControlInput1,    new InputAxisAndButtons {axis = false, inputName = inputString + " JoinControl", shortName = "JoinControl", inUse = false}},
                {PlayerInput.StartGameInput,        new InputAxisAndButtons {axis = false, inputName = inputString + " StartGame", shortName = "StartGame", inUse = false}}
            }
        };
    }

	// Update is called once per frame
	void Update ()
    {
        detectControllerChange();
        detectInput();
	}

    private void detectControllerChange()
    {
        string[] controllers = Input.GetJoystickNames();

        for (int i = 0; i < controllers.Length; i++)
        {
            //if new controller
            if (!string.IsNullOrEmpty(controllers[i]) && !currentControllers.Contains(controllers[i]))
            {
                PlayerInputScheme player = buildPlayerInputControllerFull(i + 1);
                players.Add(player);

                if (inputAdded != null)
                    inputAdded();
                if (playerAdded != null)
                    playerAdded(player);
            }
        }

        controllerSplitCheck();

        foreach (string controller in currentControllers)
        {            
            //if controller removed
            if (!string.IsNullOrEmpty(controller) && !controllers.Contains(controller))
            {
                removeInputs(currentControllers.IndexOf(controller) + 1);

                if (inputRemoved != null)
                    inputRemoved();
            }
        }

        currentControllers = controllers.ToList();
    }

    private void controllerSplitCheck()
    {
        //do controller split
        //for each player that asked for a split, see if their dictionary values match
        //conitionals written down

        //do controllers join
        //for each player that asked for a join, see if their dictionary values match
        //then doo whats written down

        List<PlayerInputScheme> tempPlayers = new List<PlayerInputScheme>(players);

        //controller split checking
        foreach (PlayerInputScheme player in tempPlayers)
        {
            switch (player.inputType)
            {
                //if they are full controller schemes, then we can only split
                case InputType.ControllerFull:
                    //if both buttons are being held down
                    if (Input.GetButton(player.inputs[PlayerInput.SplitControlInput1].inputName) && Input.GetButton(player.inputs[PlayerInput.SplitControlInput2].inputName))
                    {
                        //check to see if we haven't split resecntly
                        if (!player.inputs[PlayerInput.SplitControlInput1].inUse && !player.inputs[PlayerInput.SplitControlInput2].inUse)
                        {
                            //remove full controller schema
                            players.Remove(player);

                            if (playerRemoved != null)
                                playerRemoved(player);

                            //add two new ones
                            PlayerInputScheme playerHL = buildPlayerInputControllerHalf(player.controller, player.id, true);
                            PlayerInputScheme playerHR = buildPlayerInputControllerHalf(player.controller, player.id + 1, false);

                            players.Add(playerHL);
                            players.Add(playerHR);

                            playerHL.inputs[PlayerInput.SplitControlInput1].inUse = true;
                            playerHR.inputs[PlayerInput.SplitControlInput1].inUse = true;

                            if (playerAdded != null)
                            {
                                playerAdded(playerHL);
                                playerAdded(playerHR);
                            }
                        }
                    }
                    else
                    {
                        //else they've just been released, set inuse to false
                        player.inputs[PlayerInput.SplitControlInput1].inUse = false;
                        player.inputs[PlayerInput.SplitControlInput2].inUse = false;
                    }
                    break;
                //if we have a half left, find the half right that has the same controller
                case InputType.ControllerHalfL:
                    foreach (PlayerInputScheme player2 in tempPlayers)
                    {
                        //if 2 players on the same on controller exist
                        if (player.controller == player2.controller && player.id != player2.id)
                        {
                            //if both buttons are being held down
                            if (Input.GetButton(player.inputs[PlayerInput.SplitControlInput1].inputName) && Input.GetButton(player2.inputs[PlayerInput.SplitControlInput1].inputName))
                            {
                                //check to see if we haven't split resecntly
                                if (!player.inputs[PlayerInput.SplitControlInput1].inUse && !player2.inputs[PlayerInput.SplitControlInput1].inUse)
                                {
                                    //remove players
                                    players.Remove(player);
                                    players.Remove(player2);

                                    if (playerRemoved != null)
                                    {
                                        playerRemoved(player);
                                        playerRemoved(player2);
                                    }

                                    //add full back in
                                    PlayerInputScheme playerF = buildPlayerInputControllerFull(player.controller);

                                    players.Add(playerF);

                                    //set buttons to inuse
                                    playerF.inputs[PlayerInput.SplitControlInput1].inUse = true;
                                    playerF.inputs[PlayerInput.SplitControlInput2].inUse = true;

                                    if (playerAdded != null)
                                        playerAdded(playerF);
                                }
                            }
                            else
                            {
                                //else they've just been released, set inuse to false
                                player.inputs[PlayerInput.SplitControlInput1].inUse = false;
                                player2.inputs[PlayerInput.SplitControlInput1].inUse = false;
                            }
                        }
                    }
                    break;
            }
        }
    }

    private void removeInputs(int controller)
    {
        //remove all the players linked to the controller
        List<PlayerInputScheme> tempPlayers = new List<PlayerInputScheme>(players);
        foreach (PlayerInputScheme player in tempPlayers)
        {
            if (player.controller == controller)
            {
                players.Remove(player);

                if (playerRemoved != null)
                    playerRemoved(player);
            }
        }
    }

    private void detectInput()
    {
        float input;
        bool noAxis = true;

        foreach (PlayerInputScheme player in players)
        {
            //enumerate through out inputs
            foreach (PlayerInput playerInput in Enum.GetValues(typeof(PlayerInput)))
            {
                if (player.inputs.ContainsKey(playerInput))
                {
                    if (player.inputs[playerInput].axis)
                    {
                        //axis
                        input = Input.GetAxis(player.inputs[playerInput].inputName);

                        if (axisPressed != null)
                            axisPressed(player, player.inputs[playerInput].shortName, input);

                        if (input != 0)
                        {
                            noAxis = false;

                            //button press emulation for axis
                            if (!player.inputs[playerInput].inUse)
                            {
                                player.inputs[playerInput].inUse = true;
                                logInputDown(player, player.inputs[playerInput].shortName, input, true);
                            }
                        }
                        else
                            player.inputs[playerInput].inUse = false;
                    }
                    else
                    {
                        //buttons
                        if (Input.GetButtonDown(player.inputs[playerInput].inputName))
                            logInputDown(player, player.inputs[playerInput].shortName, 1, true);

                        if (Input.GetButtonUp(player.inputs[playerInput].inputName))
                            logInputUp(player, player.inputs[playerInput].shortName, 1);
                    }
                }
            }
        }
        
        //menu clearer if no one has pressed on an axis
        if (noAxis && axisPressed != null)
            axisPressed(new PlayerInputScheme(), "Vertical", 10);

        if (buttonPressed != null)
            foreach (InputLog log in loggedButtonsDown)
                buttonPressed(log.InputtedPlayer, log.input, log.value);

        loggedButtonsDown.Clear();
        loggedInputStrings.Clear();

        if (buttonLetGo != null)
            foreach (InputLog log in loggedButtonsUp)
                buttonLetGo(log.InputtedPlayer, log.input, log.value);

        loggedButtonsUp.Clear();
    }
    
    private void logInputUp(PlayerInputScheme player, string input, float value)
    {
        InputLog log = new InputLog();
        log.InputtedPlayer = player;
        log.input = input;
        log.value = value;

        loggedButtonsUp.Add(log);
    }

    private void logInputDown(PlayerInputScheme player, string input, float value, bool duplicate)
    {
        InputLog log = new InputLog();
        log.InputtedPlayer = player;
        log.input = input;
        log.value = value;

        logInputDown(log, false);
    }

    private void logInputDown(PlayerInputScheme player, string input, float value)
    {
        logInputDown(player, input, value, false);
    }

    private void logInputDown(InputLog input)
    {
        logInputDown(input, false);
    }

    private void logInputDown(InputLog input, bool duplicate)
    {
        //logs input, but doesn't duplicate if set
        if ((!loggedInputStrings.Contains(input.input)) || duplicate)
        {
            loggedButtonsDown.Add(input);
            loggedInputStrings.Add(input.input);
        }
    }

    public List<PlayerInputScheme> getInputs()
    {
        return players;
    }
}
