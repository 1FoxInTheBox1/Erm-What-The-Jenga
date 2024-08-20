using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

// thank you for playing my game! (mario reference)

public class GameManager : MonoBehaviour
{
    public GameObject planePrefab;
    [SerializeField]
    TMP_Text scoreText;

    GameObject[] buildings;
    GameObject[] planes;
    public List<Layer> layers;
    public Layer currentLayer;
    Camera cam;

    [SerializeField]
    float gameOverTimer = 5;
    bool isInGameOver = false;
    public uint TotalScore;
    
    public void CalculateTotalScore(){
        uint score = 0;
        foreach(var layer in layers){
            // score += layer.LayerScore;
            score += 1;
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
            CalculateTotalScore();
            NewLayer();
        }
        // If the game is over then decrease the time until reset
        if (isInGameOver) gameOverTimer -= Time.deltaTime;
        // If the game over timer is less than 0 then load the title screen
        if (gameOverTimer <= 0) SceneManager.LoadScene(0);

        scoreText.SetText(TotalScore.ToString());
        
        CheckGameOver();
    }

    // Starts a new game
    void StartGame()
    {
        buildings = Resources.LoadAll<GameObject>("Buildings");
        currentLayer = CreateLayer(2, 5);
        layers.Add(currentLayer);
        currentLayer.plane = GameObject.FindGameObjectWithTag("Starting Plane");
        currentLayer.DisplaySelectionBar();
    }

    // Drops a new plane and creates a new layer
    void NewLayer()
    {
        // Create a new layer object, set it as the current layer
        currentLayer = CreateLayer(2, 5);
        layers.Add(currentLayer);

        // Spawn a new plane to fall on top of the stage
        // The plane's spawn point is 10 units above the camera's center
        Vector3 planeSpawnpoint = new Vector3(cam.transform.position.x, cam.transform.position.y + 10, 0);
        currentLayer.plane = Instantiate(planePrefab, planeSpawnpoint, Quaternion.identity);
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

    // Checks if a plane fell below the previous plane
    private bool DidAPlaneFallOff(out GameObject plane){
        if (layers.Count <= 1)
        {
            plane = GameObject.FindGameObjectWithTag("Starting Plane");
            return false;
        }

        for(int i = 1; i < layers.Count; i++){
            if(layers[i].plane.transform.position.y < layers[i-1].plane.transform.position.y){
                plane = layers[i].plane;
                return true;
            }
        }
        plane = null;
        return false;
    }
    
    // Checks if the game should end
    void CheckGameOver()
    {
        // Check if there are more than three layers
        if (layers.Count > 3)
        {
            // Loop through the subset of layers
            for (int i = layers.Count - 3; i < layers.Count; i++)
            {
                // Access each layer in the subset
                var layer = layers[i];
                if (layer.DidABuildingFallOff(layer.plane, out Building fallenBuilding)){
                    ShowGameOver(fallenBuilding.transform.position);
                }
            }
        }
        else{
            foreach(var layer in layers){
                if (layer.DidABuildingFallOff(layer.plane, out Building fallenBuilding)){
                    ShowGameOver(fallenBuilding.transform.position);
                }
            }
        }
        foreach(var layer in layers){
            if(layer.IsABuildingBelowTheStartingPlane(out Building fallenBuilding)){
                ShowGameOver(fallenBuilding.transform.position);
            }
        }
        if(DidAPlaneFallOff(out GameObject plane)){
            ShowGameOver(plane.transform.position);
        }
    }

    // Zooms out camera and centers it on the fallen object
    // Also starts a timer that will send the player back to title screen when it ends
    void ShowGameOver(Vector3 endPosition)
    {
        GameObject.FindGameObjectWithTag("BuildArea").GetComponent<SpriteRenderer>().enabled = false;
        Camera.main.GetComponent<CameraFollow>().followGameOver(endPosition);
        isInGameOver = true;
    }
}

public class Layer
{
    List<Building> buildings;
    public GameObject plane;

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

    // Checks if a building is below the starting plane
    public bool IsABuildingBelowTheStartingPlane(out Building fallenBuilding){
        foreach(var building in buildings){
            if(building.transform.position.y < -20 && building.isPlaced){
                fallenBuilding = building;
                return true;
            }
        }
        fallenBuilding = null;
        return false;
    }

    // Checks if a building fell below its plane
    public bool DidABuildingFallOff(GameObject plane, out Building fallenBuilding){
        foreach(var building in buildings){
            if(building.FellOff(plane.transform.position)){
                fallenBuilding = building;
                return true;
            }
        }
        fallenBuilding = null;
        return false;
    }
}

