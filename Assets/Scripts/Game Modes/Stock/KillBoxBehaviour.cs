using UnityEngine;
using System.Collections;

public class KillBoxBehaviour : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        //kill any player that comes here
        if (other.CompareTag("Player"))
            other.SendMessage("setHit", true);
    }
}
