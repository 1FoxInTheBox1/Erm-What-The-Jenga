using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    Vector3 target;
    Vector3 prevTarget;
    // Follow time is the time in seconds it will take for the camera to pan to its target position
    public float followTime = 1f;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        target = new Vector3(0, 0, -10);
        prevTarget = new Vector3(0, 0, -10);
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // This is meant to be a simple linear interpolation
        // Might be a better way of doing it but this seems to work just fine
        Vector3 path = target - prevTarget;
        float progress = Mathf.Clamp(t / followTime, 0, 1);
        transform.position = prevTarget + (path * progress);
        t += Time.deltaTime;
    }

    // Sets the position the camera should pan over to
    public void setTarget(Vector3 newTarget)
    {
        prevTarget = target;
        target = new Vector3(newTarget.x, newTarget.y, -10);
        t = 0;
    }

    // This is just used to display the new layer's selection bar in the correct position
    // We need it to be to the left of where the camera is going to be, so we return the target position
    public Vector3 getSelectionBarCenter()
    {
        return target;
    }
}
