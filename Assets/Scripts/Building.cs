using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
public class Building : MonoBehaviour
{
    public float points;
    public bool isPlaced;
    private int curCollisions;

    private Rigidbody2D rb;
    private Camera cam;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
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
    }

    void AdjustScale()
    {
        float scaleInput = Input.GetAxis("Mouse ScrollWheel");
        var newVector = transform.localScale + new Vector3(scaleInput, scaleInput, scaleInput);
        newVector.Clamp(1, 10);
        // if its already too big or too small, then dont make it bigger or smaller.
        transform.localScale = newVector;
    }

    // Each building needs its own way to get points
    // so we'll inherit from the building class and custom make a getpoints func
    public virtual uint GetPoints(){
        Debug.Log("You forgot to override GetPoints dummy!");
        return 0;
    }

    // Disables physics for this building
    void DeactivatePhysics()
    {

    }

    // Returns true if the building is "settled"
    bool IsSettled()
    {
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        curCollisions++;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        curCollisions--;
    }
}
