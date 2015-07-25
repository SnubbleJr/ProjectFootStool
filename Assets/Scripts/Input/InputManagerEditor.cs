using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections;

[ExecuteInEditMode] 
public class InputManagerEditor : MonoBehaviour {

    void Start()
    {
        ClearInputManager();
        SetupInputManager();
    }

    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
    {
        SerializedProperty child = parent.Copy();
        child.Next(true);
        do
        {
            if (child.name == name) return child;
        }
        while (child.Next(false));
        return null;
    }

    private static bool AxisDefined(string axisName)
    {
        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

        axesProperty.Next(true);
        axesProperty.Next(true);
        while (axesProperty.Next(false))
        {
            SerializedProperty axis = axesProperty.Copy();
            axis.Next(true);
            if (axis.stringValue == axisName) return true;
        }
        return false;
    }
    public enum AxisType
    {
        KeyOrMouseButton = 0,
        MouseMovement = 1,
        JoystickAxis = 2
    };

    public class InputAxis
    {
        public string name;
        public string descriptiveName;
        public string descriptiveNegativeName;
        public string negativeButton;
        public string positiveButton;
        public string altNegativeButton;
        public string altPositiveButton;

        public float gravity;
        public float dead;
        public float sensitivity;

        public bool snap = false;
        public bool invert = false;

        public AxisType type;

        public int axis;
        public int joyNum;
    }

    public enum ControllerAxisDef
    {
        Horizontal = 1,
        Vertical = 2,
        Flump = 10,
        WiggleHorizontal = 4,
        WiggleVertical = 5
    }

    public enum ControllerButtonDef
    {
        Jump =  4,
        ChangeMode = 3,
        Submit = 0,
        Cancel = 1,
        Pause = 7,
        SplitControl1 = 7,
        SplitControl2 = 6
    }

    public enum ControllerLeftHalfAxisDef
    {
        Horizontal = 1,
        Vertical = 2,
        Cancel = 9,
        Flump = 9
    }

    public enum ControllerLeftHalfButtonDef
    {
        Jump = 4,
        ChangeMode = 8,
        Submit = 4,
        Pause = 6,
        JoinControl = 6
    }

    public enum ControllerRightHalfAxisDef
    {
        Horizontal = 4,
        Vertical = 5,
        Cancel = 10,
        Flump = 10
    }

    public enum Controlle0rRightHalfButtonDef
    {
        Jump = 5,
        ChangeMode = 9,
        Submit = 5,
        Pause = 7,
        JoinControl = 7
    }

    private static void AddAxis(InputAxis axis)
    {
        if (AxisDefined(axis.name)) return;

        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

        axesProperty.arraySize++;
        serializedObject.ApplyModifiedProperties();

        SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

        GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
        GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
        GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
        GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
        GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
        GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
        GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
        GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
        GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
        GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
        GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
        GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
        GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
        GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
        GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

        serializedObject.ApplyModifiedProperties();
    }

    public static void ClearInputManager()
    {
        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
        axesProperty.ClearArray();
        serializedObject.ApplyModifiedProperties();
    }

    public static void SetupInputManager()
    {
        // Add mouse definitions
        AddAxis(new InputAxis() { name = "Mouse X", gravity = 3, dead = 0.19f, sensitivity = 3, snap = true, type = AxisType.MouseMovement, axis = 1 });
        AddAxis(new InputAxis() { name = "Mouse Y", gravity = 3, dead = 0.19f, sensitivity = 3, snap = true, type = AxisType.MouseMovement, axis = 2 });

        SetupKeyBoardControlls(1, "a", "d", "w", "s", "e", "q", "x", "escape");
        SetupKeyBoardControlls(2, "left", "right", "up", "down", "page down", "delete", "end", "escape");
        SetupKeyBoardControlls(3, "u", "o", "8", "i", "9", "7", "k", "escape");
        SetupKeyBoardControlls(4, "v", "n", "g", "b", "h", "f", "m", "escape");

        SetupNonSharedControllers();
        SetupSharedControllers();
    }

    private static void SetupKeyBoardControlls(int index, string left, string right, string up, string down, string submit, string cancel, string changeMode, string pause)
    {
        AddAxis(new InputAxis() { name = "Keyboard " + index.ToString() + " Horizontal", descriptiveName = "Menu navagation only", negativeButton = left, positiveButton = right, gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
        AddAxis(new InputAxis() { name = "Keyboard " + index.ToString() + " Vertical", descriptiveName = "Menu navagation only", negativeButton = down, positiveButton = up, gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
        AddAxis(new InputAxis() { name = "Keyboard " + index.ToString() + " Left", positiveButton = left, gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
        AddAxis(new InputAxis() { name = "Keyboard " + index.ToString() + " Right", positiveButton = right, gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
        AddAxis(new InputAxis() { name = "Keyboard " + index.ToString() + " Jump", positiveButton = up, gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
        AddAxis(new InputAxis() { name = "Keyboard " + index.ToString() + " Flump", positiveButton = down, gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
        AddAxis(new InputAxis() { name = "Keyboard " + index.ToString() + " Submit", positiveButton = submit, gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
        AddAxis(new InputAxis() { name = "Keyboard " + index.ToString() + " Cancel", positiveButton = cancel, gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
        AddAxis(new InputAxis() { name = "Keyboard " + index.ToString() + " ChangeMode", positiveButton = changeMode, gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });
        AddAxis(new InputAxis() { name = "Keyboard " + index.ToString() + " Pause", positiveButton = pause, gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton });

    }

    private static void SetupNonSharedControllers()
    {
        // Add gamepad definitions for each not shared controller
        for (int i = 1; i <= 8; i++)
        {
            //axis
            foreach (string axisName in Enum.GetNames(typeof(ControllerAxisDef)))
            {
                ControllerAxisDef axis = (ControllerAxisDef)Enum.Parse(typeof(ControllerAxisDef), axisName);

                InputAxis inputAxis = new InputAxis()
                {
                    name = "Controller " + i + " " + axisName,
                    gravity = 1000,
                    dead = 0.19f,
                    sensitivity = 1,
                    snap = true,
                    type = AxisType.JoystickAxis,
                    axis = (int)axis,
                    joyNum = i,
                };

                if ((ControllerAxisDef)Enum.Parse(typeof(ControllerAxisDef), axisName) == ControllerAxisDef.Vertical)
                    inputAxis.invert = true;

                if ((ControllerAxisDef)Enum.Parse(typeof(ControllerAxisDef), axisName) == ControllerAxisDef.Flump)
                {
                    inputAxis.dead = 0.19f;
                    inputAxis.sensitivity = 100;
                    inputAxis.snap = false;
                }
                AddAxis(inputAxis);
            }

            //buttons
            foreach (string buttonName in Enum.GetNames(typeof(ControllerButtonDef)))
            {
                ControllerButtonDef button = (ControllerButtonDef)Enum.Parse(typeof(ControllerButtonDef), buttonName);

                AddAxis(new InputAxis()
                {
                    name = "Controller " + i + " " + buttonName,
                    positiveButton = "joystick " + i + " button " + ((int)button).ToString(),
                    gravity = 1000,
                    dead = 0.001f,
                    sensitivity = 1000,
                    type = AxisType.KeyOrMouseButton,
                });
            }
        }
    }

    private static void SetupSharedControllers()
    {
        // Add gamepad definitions for each shared controller
        for (int i = 1; i <= 8; i++)
        {
            //left side
            //axis
            foreach (string axisName in Enum.GetNames(typeof(ControllerLeftHalfAxisDef)))
            {
                ControllerLeftHalfAxisDef axis = (ControllerLeftHalfAxisDef)Enum.Parse(typeof(ControllerLeftHalfAxisDef), axisName);

                InputAxis inputAxis = new InputAxis()
                {
                    name = "Controller " + i + " left side " + axisName,
                    gravity = 1000,
                    dead = 0.19f,
                    sensitivity = 1,
                    snap = true,
                    type = AxisType.JoystickAxis,
                    axis = (int)axis,
                    joyNum = i,
                };

                if ((ControllerLeftHalfAxisDef)Enum.Parse(typeof(ControllerLeftHalfAxisDef), axisName) == ControllerLeftHalfAxisDef.Vertical)
                    inputAxis.invert = true;

                if (axis == ControllerLeftHalfAxisDef.Flump)
                {
                    inputAxis.dead = 0.19f;
                    inputAxis.sensitivity = 100;
                    inputAxis.snap = false;
                }
                AddAxis(inputAxis);
            }

            //buttons
            foreach (string buttonName in Enum.GetNames(typeof(ControllerLeftHalfButtonDef)))
            {
                ControllerLeftHalfButtonDef button = (ControllerLeftHalfButtonDef)Enum.Parse(typeof(ControllerLeftHalfButtonDef), buttonName);

                AddAxis(new InputAxis()
                {
                    name = "Controller " + i + " left side " + buttonName,
                    positiveButton = "joystick " + i + " button " + ((int)button).ToString(),
                    gravity = 1000,
                    dead = 0.001f,
                    sensitivity = 1000,
                    type = AxisType.KeyOrMouseButton,
                });
            }

            //right side
            //axis
            foreach (string axisName in Enum.GetNames(typeof(ControllerRightHalfAxisDef)))
            {
                ControllerRightHalfAxisDef axis = (ControllerRightHalfAxisDef)Enum.Parse(typeof(ControllerRightHalfAxisDef), axisName);

                InputAxis inputAxis = new InputAxis()
                {
                    name = "Controller " + i + " right side " + axisName,
                    gravity = 1000,
                    dead = 0.19f,
                    sensitivity = 1,
                    snap = true,
                    type = AxisType.JoystickAxis,
                    axis = (int)axis,
                    joyNum = i,
                };

                if ((ControllerRightHalfAxisDef)Enum.Parse(typeof(ControllerRightHalfAxisDef), axisName) == ControllerRightHalfAxisDef.Vertical)
                    inputAxis.invert = true;

                if (axis == ControllerRightHalfAxisDef.Flump)
                {
                    inputAxis.dead = 0.19f;
                    inputAxis.sensitivity = 100;
                    inputAxis.snap = false;
                }
                AddAxis(inputAxis);
            }

            //buttons
            foreach (string buttonName in Enum.GetNames(typeof(Controlle0rRightHalfButtonDef)))
            {
                Controlle0rRightHalfButtonDef button = (Controlle0rRightHalfButtonDef)Enum.Parse(typeof(Controlle0rRightHalfButtonDef), buttonName);

                AddAxis(new InputAxis()
                {
                    name = "Controller " + i + " right side " +  buttonName,
                    positiveButton = "joystick " + i.ToString() + " button " + ((int)button).ToString(),
                    gravity = 1000,
                    dead = 0.001f,
                    sensitivity = 1000,
                    type = AxisType.KeyOrMouseButton,
                });
            }
        }
    }
}

#endif