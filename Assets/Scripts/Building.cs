using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.UIElements;
using System.Buffers.Text;

public class Building : MonoBehaviour, IPointerDownHandler
{
    public float points;
    public float settleThreshold = 1;
    public bool isPlaced;
    public bool selected;
    private int curCollisions;
    private bool inBuildArea = false;
    private bool onFloor = false;
    private float settleCounter = 0;

    public UnityEvent buildingSelect;
    public UnityEvent buildingDeselect;
    public UnityEvent buildingFall;

    public Rigidbody2D rb;
    private Collider2D col;
    private Camera cam;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.gravityScale = 0;
        selected = false;
        GameObject buildAreaObject = GameObject.FindGameObjectWithTag("BuildArea");
        if (buildAreaObject != null)
        {
            BuildArea buildArea = buildAreaObject.GetComponent<BuildArea>();
            buildingSelect.AddListener(buildArea.Show);
            buildingDeselect.AddListener(buildArea.Hide);
        }
    }

    // Update is called once per frame
    void Update()
    {
        setSpriteColor();
        if (selected)
        {
            if (!isPlaced)
            {
                FollowMouse();
            }

            AdjustScale();
        } else
        {
            ApplyDrag();
            if (IsSettled() && isPlaced)
            {
                //Debug.Log("Rigidbody is settled");
                DeactivatePhysics();
            } 
        }
    }

    private uint[] angleThreshold = {85, 95};
    void ApplyDrag()
    {
        if (!onFloor)
            return;
        //Debug.Log("Applying drag");
        var angle = Vector2.Angle(rb.velocity, Vector2.up);
        if (angle < angleThreshold[0] || angle > angleThreshold[1])
            return;
        var counterVel = -(rb.velocity * 0.2f);
        // We want the ball to not only slow down, but be capable of stopping on a sloped surface.
        // That means we can't just use a linear drag, we need to use a counter velocity.
        rb.velocity += counterVel * (Time.deltaTime * 60);
    }

    // This is used to determine when a building has been clicked and to to perform the right actions when that happens
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isPlaced)
            return;
        Building b = eventData.pointerCurrentRaycast.gameObject.GetComponent<Building>();
        if (!selected)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            b.selected = true;
            buildingSelect.Invoke();
            return;
        }
        if (IsValidPlacement())
            b.Place();
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
        rb.mass = newVector.magnitude;
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
            settleCounter = Mathf.Clamp(settleCounter + Time.deltaTime * ((linearspeed < settleThreshold && angularspeed < settleThreshold) ? 1 : -1), 0, 1);
        }
        // if its not settled, it'll be <1 and be considered false, otherwise it's true.
        return settleCounter >= 1 && isPlaced;
    }

    // Returns true if the building is currently in a position where it can be placed.
    // This means that it is within the build area and is not overlapping any other objects
    bool IsValidPlacement()
    {
        return curCollisions == 0 && inBuildArea;
    }

    // Tints the sprite a certain color depending on if it can be placed, has settled, etc.
    void setSpriteColor ()
    {
        if (IsSettled())
        {
            sprite.color = Color.blue;
        } else if (selected && !IsValidPlacement())
        {
            sprite.color = Color.red;
        } else
        {
            sprite.color = Color.white;
        }
    }

    void checkCollisionEnter(Collider2D other) {
        
        switch (other.tag) {
            case "FloorPlane":
                curCollisions++;
                onFloor = true;
                break;
            case "BuildingKillTrigger":
                Destroy(gameObject);
                buildingFall.Invoke();
                break;
            case "BuildArea":
                inBuildArea = true;
                break;
            default:
                curCollisions++;
                break;
        }
    }

    void checkCollisionExit(Collider2D other) {
        switch (other.tag) {
            case "FloorPlane":
                curCollisions--;
                onFloor = false;
                break;
            case "BuildArea":
                inBuildArea = false;
                break;
            default:
                curCollisions--;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        checkCollisionEnter(other.collider);
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        checkCollisionExit(other.collider);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        checkCollisionEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        checkCollisionExit(other);
    }

    // Checks to make sure a block didnt fall off the layer.
    public bool FellOff(Layer minHeight){
        return true;
    }
}
