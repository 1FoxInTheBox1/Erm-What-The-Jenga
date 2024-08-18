using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float points;
    public bool isPlaced;

    private Rigidbody2D rb;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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
            Place();
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
        isPlaced = true;
        rb.gravityScale = 1;
        rb.constraints = RigidbodyConstraints2D.None;
    }

    void AdjustScale()
    {
        float scaleInput = Input.GetAxis("Mouse ScrollWheel");
        transform.localScale += new Vector3(scaleInput, scaleInput, scaleInput);
        Vector3.ClampMagnitude(transform.localScale, 10);
    }

    void GetPoints()
    {

    }

    // Disables physics for this building
    void Deactivate()
    {

    }

    // Returns true if the building is "settled"
    bool IsSettled()
    {
        return false;
    }
}
