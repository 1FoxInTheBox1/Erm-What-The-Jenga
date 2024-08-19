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
        Vector3 path = target - prevTarget;
        float progress = Mathf.Clamp(t / followTime, 0, 1);
        transform.position = prevTarget + (path * progress);
        t += Time.deltaTime;
    }

    public void setTarget(Vector3 newTarget)
    {
        prevTarget = target;
        target = new Vector3(newTarget.x, newTarget.y, -10);
        t = 0;
    }

    public Vector3 getSelectionBarCenter()
    {
        return target;
    }
}
