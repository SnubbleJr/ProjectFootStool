using UnityEngine;
using System.Collections;

public class PositionLerper : MonoBehaviour {

    //will constantly attempt to remain in it's current position

    public float time = 30f;

    private Vector3 pos;

    // Use this for initialization
    void OnEnable()
    {
        pos = transform.localPosition;
    }

    void OnDisable()
    {
        transform.localPosition = pos;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, pos, time * Time.deltaTime);
    }
}
