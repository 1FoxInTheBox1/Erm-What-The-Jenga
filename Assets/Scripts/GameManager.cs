using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    GameObject[] buildings;
    Dictionary<GameObject, int> buildingsToPlace;
    List<Layer> layers;
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
        buildingsToPlace = new Dictionary<GameObject, int>();
    }

    // Creates a list of buildings the player will need to place
    void GetPlacementList(int numTypes, int totalBuildings)
    {
        List<GameObject> possibleBuildings = new List<GameObject>(buildings);
        int remainingBuildings = totalBuildings;
        for (int i = numTypes; i > 0; i--)
        {
            int buildingIndex = Random.Range(0, possibleBuildings.Count);
            GameObject building = possibleBuildings[buildingIndex];
            buildingsToPlace[building] = 0;
            int amount = (i == 1) ? remainingBuildings : Random.Range(1, remainingBuildings + 1);
            buildingsToPlace[building] = amount;
            remainingBuildings -= amount;
            possibleBuildings.RemoveAt(buildingIndex);
        }
    }


    // Returns true if all buildings on the current layer are settled
    bool LayerFinished()
    {
        // if the latest layer's number of placed buildings is equal to the total number placed buldings, return true
        Layer latestLayer = layers[layers.Count - 1];
        uint latestLayerNumberOfBuildings = latestLayer.NumberOfBuildingsInLayer();
        return latestLayer.PlacedBuildingsCount == latestLayerNumberOfBuildings;
    }

    // what is this?
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

    bool IsSettled()
    {
        // TODO: Settled logic
        return false;
    }

    // Disables physics for this layer
    void DeactivatePhysics()
    {

    }
}

