using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitlescreenHandler : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Camera mainCamera;

    public Button startButton;
    public Button settingsButton;
    public Button quitButton;

    public GameObject settingsPanel;

    public GameObject fallingBlockContainer;
    public Transform floorBlock;
    
    GameObject[] allPrefabs = {};
    float counter = 0;

    // Handlers for each button in the main menu
    void StartClicked() {
        // Loads the main game scene
        SceneManager.LoadScene(1);
    }
    void SettingsClicked() {
        // Toggles the settings panel
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
    void QuitClicked() {
        // Quits the game (only works in builds)
        Application.Quit();
    }
    // Grabs a random prefab from the Resources/Buildings folder and spawns it
    void SpawnRandomBlock() {
        // Get a random prefab
        GameObject randomPrefab = allPrefabs[Random.Range(0, allPrefabs.Length)];

        // Get the x position and scale of the floor block
        var xPos = floorBlock.position.x;
        var halfSize = floorBlock.localScale.x / 2;

        // Generate a random rotation
        var rot = Random.Range(0, 360);

        // Spawn the block above the camera, with a random rotation and x position relative to the floor
        GameObject newBlock = Instantiate(randomPrefab, new Vector3(xPos + Random.Range(-halfSize, halfSize), 10, 0), Quaternion.Euler(new Vector3(0, 0, rot)));
        
        // Set the scale of the block to a random value between 0.5 and 1.5
        var scale = Random.Range(0.5f, 1.5f);
        newBlock.transform.localScale = new Vector3(scale, scale);

        // Disable the building script since we don't want the game logic to be running right now
        var building = newBlock.GetComponent<Building>();
        building.enabled = false;

        // Disable the trigger property so that the block collides with the floor
        newBlock.GetComponent<Collider2D>().isTrigger = false;
        // Remove constraints on the block's motion
        Rigidbody2D rb = newBlock.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.None;
        // Enable gravity on block
        rb.gravityScale = 1;
        // Set the parent of the block to the falling block container
        newBlock.transform.SetParent(fallingBlockContainer.transform);
    }
    void Start()
    {
        // Store all prefabs from the Resources/Buildings folder
        allPrefabs = Resources.LoadAll<GameObject>("Buildings");

        // Add listeners to each button
        startButton.onClick.AddListener(StartClicked);
        settingsButton.onClick.AddListener(SettingsClicked);
        quitButton.onClick.AddListener(QuitClicked);
    }

    // Update is called once per frame
    void Update()
    {
        // Change the color of the title text and background color over time
        var newTitleColor = Color.HSVToRGB(Mathf.PingPong(Time.time * 0.5f, 1), 1, 1);
        titleText.color = newTitleColor;

        var newBgColor = Color.HSVToRGB(Mathf.PingPong(Time.time * 0.1f, 1), 0.2f, 0.8f);
        mainCamera.backgroundColor = newBgColor;

        // Spawn a new block every 2 seconds
        counter += Time.deltaTime;
        if (counter > 2) {
            counter -= 2;
            SpawnRandomBlock();
        }
    }
}
