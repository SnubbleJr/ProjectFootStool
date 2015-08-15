using UnityEngine;
using System.Collections;

public class DrunkRotation : MonoBehaviour {

    public Vector3 minRotation;
    public Vector3 maxRotation;
    public float speed = 0.1F;  

	// Update is called once per frame
    void Update()
    {
        Vector3 rotationVec = new Vector3(pingPong(minRotation.x, maxRotation.x), pingPong(minRotation.y, maxRotation.y),pingPong(minRotation.z, maxRotation.z));

        transform.rotation = Quaternion.Euler(rotationVec);
	}

    private float pingPong(float min, float max)
    {
        float value = max - Mathf.PingPong(Time.time * speed, (max - min));
        if (float.IsNaN(value))
            value = 0;
        return value;
    }
}
