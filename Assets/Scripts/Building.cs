using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Building : MonoBehaviour, IPointerDownHandler
{
    public float points;
    public bool isPlaced;
    public bool selected;
    private int curCollisions;
    private bool inBuildArea;
    private float settleCounter = 0;

    public UnityEvent buildingSelect;
    public UnityEvent buildingDeselect;
    public UnityEvent buildingFall;

    public Rigidbody2D rb;
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
        buildingSelect.AddListener(GameObject.FindGameObjectWithTag("BuildArea").GetComponent<BuildArea>().Show);
        buildingDeselect.AddListener(GameObject.FindGameObjectWithTag("BuildArea").GetComponent<BuildArea>().Hide);
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            if (!isPlaced)
            {
                FollowMouse();
                sprite.color = (IsValidPlacement()) ? Color.white : Color.red;
            }

            AdjustScale();
        } else
        {
            if (IsSettled() && isPlaced)
            {
                //Debug.Log("Rigidbody is settled");
                sprite.color = Color.blue;
                DeactivatePhysics();
            } 
        }
    }

    // This is used to determine when a building has been clicked and to to perform the right actions when that happens
    public void OnPointerDown(PointerEventData eventData)
    {
        Building b = eventData.pointerCurrentRaycast.gameObject.GetComponent<Building>();
        if (!isPlaced)
        {
            if (selected)
            {
                if (IsValidPlacement())
                {
                    b.Place();
                }
            } else
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                b.selected = true;
                buildingSelect.Invoke();
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
        // give it no constraints, allowing it to move silly style
        rb.constraints = RigidbodyConstraints2D.None;
        // turn on gravity
        rb.gravityScale = 1;
        // set the color to white
        sprite.color = Color.white;
        col.isTrigger = false;
        selected = false;
        buildingDeselect.Invoke();
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
        if (IsSettled()){
            sprite.color = Color.blue;
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    // Returns true if the building is "settled"
    public bool IsSettled()
    {
        float linearspeed = rb.velocity.magnitude;
        float angularspeed = Mathf.Abs(rb.angularVelocity);
        if (isPlaced) {
            // If the block hasnt moved enough over a certain period of time it is considered settled.
            settleCounter = Mathf.Clamp(settleCounter + Time.deltaTime * ((linearspeed < 1 && angularspeed < 1) ? 1 : -1), 0, 1);
        }
        // if its not settled, it'll be <1 and be considered false, otherwise it's true.
        return settleCounter >= 1;
    }

    // Returns true if the building is currently in a position where it can be placed.
    // This means that it is within the build area and is not overlapping any other objects
    bool IsValidPlacement()
    {
        return curCollisions == 0 && inBuildArea;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "BuildingKillTrigger") {
            Destroy(gameObject);
            buildingFall.Invoke();
            return;
        }
        if (other.tag != "BuildArea")
        {
            curCollisions++;
        } else
        {
            inBuildArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "BuildArea")
        {
            curCollisions--;
        } else
        {
            inBuildArea = false;
        }
    }
}
