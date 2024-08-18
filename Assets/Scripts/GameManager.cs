using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class GameManager : MonoBehaviour
{
    GameObject[] buildings;
    List<Layer> layers;
    Layer currentLayer;
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
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Starts a new game
    void StartGame()
    {
        buildings = Resources.LoadAll<GameObject>("Buildings");
        currentLayer = GetPlacementList(2, 5);
    }

    // Creates a list of buildings the player will need to place
    Layer GetPlacementList(int numTypes, int totalBuildings)
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
                GameObject newBuilding = Instantiate(building, new Vector3(0, 0, 0), Quaternion.identity);
                newLayer.AddBuilding(newBuilding);
            }
        }
        return newLayer;
    }


    // Returns true if all buildings on the current layer are settled
    bool LayerFinished()
    {

        // if the latest layer's number of placed buildings is equal to the total number placed buldings, return true
        Layer latestLayer = layers[layers.Count - 1];
        uint latestLayerNumberOfBuildings = latestLayer.NumberOfBuildingsInLayer();
        return latestLayer.PlacedBuildingsCount == latestLayerNumberOfBuildings;
    }

    void DeactivateLayer()
    {

    }
}

class Layer
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

    public Layer (List<Building> buildings)
    {
        this.buildings = buildings;
        LayerScore = 0;
        PlacedBuildingsCount = 0;
    }

    public Layer()
    {
        this.buildings = new List<Building>();
        LayerScore = 0;
        PlacedBuildingsCount = 0;
    }

    bool IsSettled()
    {
        // TODO: Settled logic
        return false;
    }

    public void AddBuilding(GameObject building)
    {
        buildings.Add(building.GetComponent<Building>());
    }

    // Disables physics for this layer
    void Deactivate()
    {

    }
}

