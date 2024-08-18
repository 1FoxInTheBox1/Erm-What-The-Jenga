using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float points;
    public bool isPlaced;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaced)
        {
            FollowMouse();
        }
        Debug.Log(Input.GetAxis("Fire1"));
        if (Input.GetAxis("Fire1") == 1)
        {

        }
        AdjustScale();
    }

    // Moves the building to the mouse position
    void FollowMouse()
    {
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
    }

    // Places the building
    void Place()
    {
        
    }

    void AdjustScale()
    {
        float scaleInput = Input.GetAxis("Mouse ScrollWheel");
        transform.localScale += new Vector3(scaleInput, scaleInput, scaleInput);
        // if its already too big or too small, then dont make it bigger or smaller.
        if (transform.localScale < 2 || transform.localScale > 10){
            return
        }
        // otherwise, make it bigger or smaller.
        Vector3.ClampMagnitude(transform.localScale, 10);
    }

    // Each building needs its own way to get points
    // so we'll inherit from the building class and custom make a getpoints func
    abstract uint GetPoints();

    // Disables physics for this building
    void DeactivatePhysics()
    {

    }

    // Returns true if the building is "settled"
    bool IsSettled()
    {
        return false;
    }
}
