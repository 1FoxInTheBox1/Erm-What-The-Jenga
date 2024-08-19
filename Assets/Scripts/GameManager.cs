using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

// TODO: Why am i getting:
// "UnassignedReferenceException: The variable planePrefab of GameManager has not been assigned.
// You probably need to assign the planePrefab variable of the GameManager script in the inspector."

public class GameManager : MonoBehaviour
{
    public GameObject planePrefab;

    GameObject[] buildings;
    GameObject[] planes;
    public List<Layer> layers;
    public Layer currentLayer;
    Camera cam;
    public uint TotalScore
    {
        get { return TotalScore; }
        set { TotalScore = value; }
    }
    
    public void CalculateTotalScore(){
        uint score = 0;
        foreach(var layer in layers){
            score += layer.LayerScore;
        }
        TotalScore = score;
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            cam.gameObject.AddComponent<Physics2DRaycaster>();
        }
        layers = new List<Layer>();
        StartGame();
    }

    // Update is called once per frame
    // TODO: Fix this function
    void Update()
    {
        if (currentLayer.IsSettled())
        {
            NewLayer();
        }
    }

    // Starts a new game
    void StartGame()
    {
        buildings = Resources.LoadAll<GameObject>("Buildings");
        currentLayer = CreateLayer(2, 5);
        layers.Add(currentLayer);
        currentLayer.DisplaySelectionBar();
    }

    // Drops a new plane and creates a new layer
    void NewLayer()
    {

        // TODO: the following commented out line is generating a big bug.
        // Instantiate(plane, new Vector3(0, 20, 0), Quaternion.identity);

        //Vector3 spawnPosition = cam.transform.position + new Vector3(0, 15, 0);
        //GameObject planeInstance = Instantiate(plane, spawnPosition, Quaternion.identity);
        //Debug.Log("Created new plane");
        //currentLayer = CreateLayer(2, 6);
        //Debug.Log("Created new layer");

        // Create a new layer object, set it as the current layer
        currentLayer = CreateLayer(2, 5);
        layers.Add(currentLayer);

        // The plane's spawn point is 10 units above the camera's center
        Vector3 planeSpawnpoint = new Vector3(cam.transform.position.x, cam.transform.position.y + 10, 0);
        Instantiate(planePrefab, planeSpawnpoint, Quaternion.identity);
    }

    // Creates a new layer.
    // numTypes is the number of different types of buildings.
    // totalBuildings is the total number of buildings in the layer
    // EX: If possible types are a, b, and c,
    // then CreateLayer(2, 5) will choose 2 types of buildings and then instantiate a total of 5 buildings of those types,
    // such as [a, a, b, b, b] or [c, b, b, b, b]
    Layer CreateLayer(int numTypes, int totalBuildings)
    {
        // make a hashmap that holds how many of each building there are
        Dictionary<GameObject, int> buildingQuantities = new Dictionary<GameObject, int>();
        // make a list of all the possible buildings
        List<GameObject> possibleBuildings = new List<GameObject>(buildings);
        // the buildings that
        int remainingBuildings = totalBuildings;
        for (int i = numTypes; i > 0; i--)
        {
            int buildingIndex = Random.Range(0, possibleBuildings.Count);
            GameObject building = possibleBuildings[buildingIndex];
            buildingQuantities[building] = 0;
            // if theres only one kind of building left, make it the rest of all the buildings
            // otherwise, make a random amount of a type of buildings
            int amount = (i == 1) ? remainingBuildings : Random.Range(1, remainingBuildings + 1);
            // make the quantity of a given buildings equal to the pseudorandom amount
            buildingQuantities[building] = amount;
            // update the amount of remaining buildings
            remainingBuildings -= amount;
            // now that we used that building, remove it from the possibly chosen buildings list.
            possibleBuildings.RemoveAt(buildingIndex);
        }

        Layer newLayer = new Layer();
        foreach (GameObject building in buildingQuantities.Keys)
        {
            // for each buildings, iterate through the given quantity we previously generated
            for (int i = 0; i < buildingQuantities[building]; i++)
            {
                // and place it down on the scene.
                GameObject newBuilding = Instantiate(building, new Vector3(200, 0, 0), Quaternion.identity);
                newLayer.AddBuilding(newBuilding);
            }
        }
        // return the new layer
        return newLayer;
    }

    // Disables physics for a layer
    // TODO: remove this maybe? layer already has a DeactivatePhysics() method
    void DeactivateLayer()
    {

    }
}

public class Layer
{
    List<Building> buildings;

    public uint NumberOfBuildingsInLayer()
    {
        return (uint)buildings.Count; ;
    }
    public uint LayerScore
    {
        // yoinks the score for the layer
        get { return LayerScore; }

        // Calculates the score for this layer
        set
        {
            uint LayerScore = 0;
            foreach (var building in buildings)
            {
                LayerScore += building.GetPoints();
            }
        }
    }
    public uint PlacedBuildingsCount;

    public Layer()
    {
        this.buildings = new List<Building>();
        LayerScore = 0;
        PlacedBuildingsCount = 0;
    }

    // Checks if all buildings in a layer are settled
    public bool IsSettled()
    {
        // iterate through the list.
        foreach(var building in buildings){
            // if any of them are not settled,
            if(!building.IsSettled()){
                // return false
                return false;
            }
        }
        // if you didn't return false, then they must all be settled
        // so return true
        return true;
    }

    // Adds a building to the list of buildings in a layer
    public void AddBuilding(GameObject building)
    {
        buildings.Add(building.GetComponent<Building>());
    }

    // Disables physics for this layer
    void DeactivatePhysics()
    {
        if(IsSettled() && buildings.Count > 3){
            buildings[buildings.Count-3].rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    // Displays all the blocks to be placed in this layer at the bottom of the screen
    // TODO: Replace this with some sort of UI element
    public void DisplaySelectionBar()
    {
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].transform.position = cam.getSelectionBarCenter() + new Vector3(-8, 3 - (1.5f * i), 10);
        }
    }
}

