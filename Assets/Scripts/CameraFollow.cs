using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class CameraFollow : MonoBehaviour
{
    Camera cam;
    Vector3 target;
    float targetSize = 5;
    [SerializeField]
    float maxSize = 10;
    bool followingGameOver;
    // Follow time is the time in seconds it will take for the camera to pan to its target position
    public float followTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        target = new Vector3(0, 0, -10);
    }

    // Update is called once per frame
    void Update()
    {
        // Smooth camera :)
        float delta = Mathf.Clamp(6 * Time.deltaTime, 0, 1);
        transform.position = Vector3.Lerp(transform.position, target, delta);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, delta);
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 5, maxSize);
    }

    // Sets the position the camera should pan over to
    public void setTarget(Vector3 newTarget)
    {
        target = new Vector3(newTarget.x, newTarget.y, -10);
    }

    public void moveToNewLayer(Vector3 newCenter)
    {
        setTarget(newCenter);
        targetSize = cam.orthographicSize + .5f;
    }

    public void followGameOver(Vector3 target)
    {
        if (!followingGameOver)
        {
            setTarget(target);
            targetSize = 10;
            followingGameOver = true;
        }
    }

    // This is just used to display the new layer's selection bar in the correct position
    // We need it to be to the left of where the camera is going to be, so we return the target position
    public Vector3 getSelectionBarCenter()
    {
        return target;
    }
}
