using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public float points;
    public bool isPlaced;
    private int curCollisions;

    private Rigidbody2D rb;
    private Collider2D col;
    private Camera cam;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaced)
        {
            FollowMouse();
            if (curCollisions > 0)
            {
                sprite.color = Color.red;
            }
            else
            {
                sprite.color = Color.white;
            }
        }
        if (Input.GetAxis("Fire1") == 1 && curCollisions == 0)
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
        sprite.color = Color.white;
        col.isTrigger = false;
    }

    void AdjustScale()
    {
        float scaleInput = Input.GetAxis("Mouse ScrollWheel");
        transform.localScale += new Vector3(scaleInput, scaleInput, scaleInput);
        // if its already too big or too small, then dont make it bigger or smaller.
        /*
        if (transform.localScale < 2 || transform.localScale > 10){
        return;
        }
        */
        // otherwise, make it bigger or smaller.
        Vector3.ClampMagnitude(transform.localScale, 10);
    }

    // Each building needs its own way to get points
    // so we'll inherit from the building class and custom make a getpoints func
    //abstract uint GetPoints();

    // Disables physics for this building
    void DeactivatePhysics()
    {

    }

    // Returns true if the building is "settled"
    bool IsSettled()
    {
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "BuildingKillTrigger") {
            Destroy(gameObject);
            return;
        }
        curCollisions++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        curCollisions--;
    }
}
