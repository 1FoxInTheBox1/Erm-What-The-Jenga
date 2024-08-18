using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    List<Layer> layers;

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

    // Determines the score of the current layer
    void ScoreLayer()
    {

    }

    // Returns true if all buildings on the current layer are settled
    bool LayerFinished()
    {
        return false;
    }

    void DeactivateLayer()
    {

    }
}

class Layer
{
    List<Building> buildings;

    bool IsSettled()
    {
        return false;
    }

    // Calculates the score for this layer
    uint CalculateScore()
    {
        return 0;
    }

    void Deactivate()
    {

    }
}
