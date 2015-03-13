using UnityEngine;
using System.Collections;

public class InvertCylinderBehaviour : MonoBehaviour {

    //expands unitl it fills the screen

    public float speed;

    Vector3 newTrans;

    void Start()
    {
        newTrans = transform.localScale;
    }
	
    void Update()
    {
        if (transform.localScale.x > 80)
            this.enabled = false;

        newTrans.x += 1.5f;
        newTrans.y += 1.5f;

        transform.localScale = Vector3.Lerp(transform.localScale, newTrans, speed * Time.deltaTime);
    }
}
