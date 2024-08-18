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

    // Start is called before the first frame update
    void StartClicked() {
        SceneManager.LoadScene(1);
    }
    void SettingsClicked() {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
    void QuitClicked() {
        Debug.Log("Quit button clicked");
        Application.Quit();
    }
    void SpawnRandomBlock() {
        GameObject randomPrefab = allPrefabs[Random.Range(0, allPrefabs.Length)];
        var xPos = floorBlock.position.x;
        var halfSize = floorBlock.localScale.x / 2;
        var rot = Random.Range(0, 360);
        GameObject newBlock = Instantiate(randomPrefab, new Vector3(xPos + Random.Range(-halfSize, halfSize), 10, 0), Quaternion.Euler(new Vector3(0, 0, rot)));
        var building = newBlock.GetComponent<Building>();
        building.enabled = false;
        newBlock.GetComponent<Collider2D>().isTrigger = false;
        newBlock.transform.SetParent(fallingBlockContainer.transform);
        var scale = Random.Range(0.5f, 1.5f);
        newBlock.transform.localScale = new Vector3(scale, scale);
        newBlock.tag = "TitleBlock";
    }
    void Start()
    {
        allPrefabs = Resources.LoadAll<GameObject>("Buildings");
        startButton.onClick.AddListener(StartClicked);
        settingsButton.onClick.AddListener(SettingsClicked);
        quitButton.onClick.AddListener(QuitClicked);
    }

    // Update is called once per frame
    void Update()
    {
        var newTitleColor = Color.HSVToRGB(Mathf.PingPong(Time.time * 0.5f, 1), 1, 1);
        titleText.color = newTitleColor;

        var newBgColor = Color.HSVToRGB(Mathf.PingPong(Time.time * 0.1f, 1), 0.2f, 1);
        mainCamera.backgroundColor = newBgColor;

        counter += Time.deltaTime;
        if (counter > 2) {
            counter -= 2;
            SpawnRandomBlock();
        }
    }
}
