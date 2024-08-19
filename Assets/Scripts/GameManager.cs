using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class GameManager : MonoBehaviour
{
    public GameObject plane;

    GameObject[] buildings;
    List<Layer> layers;
    Layer currentLayer;
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
        cam.transform.position = cam.transform.position + new Vector3(0, 5, 0);
        Instantiate(plane, new Vector3(0, 20, 0), Quaternion.identity);
        currentLayer = CreateLayer(2, 5);
        layers.Add(currentLayer);
        currentLayer.DisplaySelectionBar();
    }

    // Creates a new layer.
    // numTypes is the number of different types of buildings.
    // totalBuildings is the total number of buildings in the layer
    // EX: If possible types are a, b, and c,
    // then CreateLayer(2, 5) will choose 2 types of buildings and then instantiate a total of 5 buildings of those types,
    // such as [a, a, b, b, b] or [c, b, b, b, b]
    Layer CreateLayer(int numTypes, int totalBuildings)
    {
        Dictionary<GameObject, int> buildingQuantities = new Dictionary<GameObject, int>();
        List<GameObject> possibleBuildings = new List<GameObject>(buildings);
        int remainingBuildings = totalBuildings;
        for (int i = numTypes; i > 0; i--)
        {
            int buildingIndex = Random.Range(0, possibleBuildings.Count);
            GameObject building = possibleBuildings[buildingIndex];
            buildingQuantities[building] = 0;
            int amount = (i == 1) ? remainingBuildings : Random.Range(1, remainingBuildings + 1);
            buildingQuantities[building] = amount;
            remainingBuildings -= amount;
            possibleBuildings.RemoveAt(buildingIndex);
        }

        Layer newLayer = new Layer();
        foreach (GameObject building in buildingQuantities.Keys)
        {
            for (int i = 0; i < buildingQuantities[building]; i++)
            {
                GameObject newBuilding = Instantiate(building, new Vector3(200, 0, 0), Quaternion.identity);
                newLayer.AddBuilding(newBuilding);
            }
        }
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
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].transform.position = Camera.main.transform.position + new Vector3(-8, 2 - (1.5f * i), 10);
        }
    }
}

