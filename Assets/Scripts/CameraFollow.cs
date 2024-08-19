using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    Vector3 target;
    // Follow time is the time in seconds it will take for the camera to pan to its target position
    public float followTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        target = new Vector3(0, 0, -10);
    }

    // Update is called once per frame
    void Update()
    {
        // Smooth camera :)
        float delta = Mathf.Clamp(6 * Time.deltaTime, 0, 1);
        transform.position = Vector3.Lerp(transform.position, target, delta);
    }

    // Sets the position the camera should pan over to
    public void setTarget(Vector3 newTarget)
    {
        target = new Vector3(newTarget.x, newTarget.y, -10);
    }

    // This is just used to display the new layer's selection bar in the correct position
    // We need it to be to the left of where the camera is going to be, so we return the target position
    public Vector3 getSelectionBarCenter()
    {
        return target;
    }
}
