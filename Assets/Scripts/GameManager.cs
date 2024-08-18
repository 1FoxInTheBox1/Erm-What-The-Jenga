using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    GameObject[] buildings;
    List<GameObject> buildingsToPlace;
    List<Layer> layers;

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
        buildingsToPlace = new List<GameObject>();
        GetPlacementList(2);
        foreach (GameObject building in buildingsToPlace)
        {
            Debug.Log(building);
        }
    }

    // Creates a list of buildings the player will need to place
    void GetPlacementList(int numTypes)
    {
        List<GameObject> possibleBuildings = new List<GameObject>(buildings);
        for (int i = 0; i < numTypes; i++)
        {
            int buildingIndex = Random.Range(0, possibleBuildings.Count);
            buildingsToPlace.Add(possibleBuildings[buildingIndex]);
            possibleBuildings.RemoveAt(buildingIndex);
        }
    }

    // Determines the score of the current layer
    uint ScoreLayer()
    {
        uint score = 0;
        foreach(var layer in layers){
            // score += layer.CalculateScore();
        }
        return score;
    }

    // Returns true if all buildings on the current layer are settled
    bool LayerFinished()
    {
        //return buildings.count() == placedBuildingsCount;
        return false;
    }

    void DeactivateLayer()
    {

    }
}

class Layer
{
    List<Building> buildings;
    uint placedBuildingsCount
    {
        get { return placedBuildingsCount; }
        set { placedBuildingsCount = value; }
    }

    bool IsSettled()
    {
        // TODO: Settled logic
        return false;
    }

    // Calculates the score for this layer
    uint CalculateScore()
    {
        uint score = 0;
        foreach(var building in buildings){
            //score += building.GetPoints();
        }
        return score;
    }

    // Disables physics for this layer
    void Deactivate()
    {

    }
}
