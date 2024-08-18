using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Starts a new game
    void StartGame()
    {

    }

    // Creates a list of buildings the player will need to place
    void GetPlacementList()
    {

    }


    // Returns true if all buildings on the current layer are settled
    bool LayerFinished()
    {
        // if no buildings have been placed, return false
        if(layers[-1].PlacedBuildingsCount == 0){
            return false;
        }

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

    public uint NumberOfBuildingsInLayer(){
        return (uint)buildings.Count;;
    }
    public uint LayerScore
    {
    // yoinks the score for the layer
        get { return LayerScore; }
    
    // Calculates the score for this layer
        set {
            uint LayerScore = 0;
            foreach(var building in buildings){
                LayerScore += building.GetPoints();
            }
        }
    }
    public uint PlacedBuildingsCount
    {
        get { return PlacedBuildingsCount; }
        set { PlacedBuildingsCount = value; }
    }

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
