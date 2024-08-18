using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float points;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AdjustScale();
    }

    // Places the building
    void Place()
    {
        
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
