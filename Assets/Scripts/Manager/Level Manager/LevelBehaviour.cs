using UnityEngine;
using System.Collections;

public class LevelBehaviour : MonoBehaviour {

    //simple script that enables and disables the level when asked

    public void setLevel(bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }
}
