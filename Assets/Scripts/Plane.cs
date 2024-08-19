using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Plane : MonoBehaviour
{
    public float camOffset = 1;
    private bool firstContact;

    private void Start()
    {
        firstContact = true;
    }

    // In order to keep the camera centered on the new layer,
    // The plane will tell the camera where it is the first time it hits something
    // That something SHOULD be what the player just built
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only run this the first time a collision occurs
        if (firstContact)
        {
            Camera cam = Camera.main;
            // Set camera's target position to the plane's pos + offset
            cam.GetComponent<CameraFollow>().setTarget(transform.position + new Vector3(0, camOffset, 0));
            GameManager gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            // Display the selection bar
            gm.currentLayer.DisplaySelectionBar();
            firstContact = false;
        }
    }
}
