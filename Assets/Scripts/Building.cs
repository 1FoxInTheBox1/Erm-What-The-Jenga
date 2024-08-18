using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using System.IO;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour, IPointerDownHandler
{
    public float points;
    public bool isPlaced;
    public bool selected;
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
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.gravityScale = 0;
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
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

            AdjustScale();
        } else
        {

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Building b = eventData.pointerCurrentRaycast.gameObject.GetComponent<Building>();
        if (!isPlaced)
        {
            if (selected)
            {
                if (curCollisions == 0)
                {
                    b.Place();
                }
            } else
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                b.selected = true;
            }
        }
    }

    // Moves the building to the mouse position
    void FollowMouse()
    {
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
    }

    // Places the building
    public void Place()
    {
        isPlaced = true;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = 1;
        sprite.color = Color.white;
        col.isTrigger = false;
        selected = false;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        curCollisions++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        curCollisions--;
    }
}
