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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (firstContact)
        {
            Camera cam = Camera.main;
            cam.GetComponent<CameraFollow>().setTarget(transform.position + new Vector3(0, camOffset, 0));
            GameManager gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gm.currentLayer.DisplaySelectionBar();
            firstContact = false;
        }
    }
}
